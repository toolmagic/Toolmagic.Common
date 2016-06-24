using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolmagic.Common.Collections;

namespace Toolmagic.Common.Tasks
{
	public static class SearchTask
	{
		public static async Task StartNew<T>
			(
			IEnumerable<T> initialItems,
			Func<T, CancellationToken, IEnumerable<T>> action,
			CancellationToken cancellationToken,
			SearchTaskOptions options = null
			)
		{
			var notNullAction = Argument.IsNotNull(action, nameof(action));
			var notNullInitialItems = Argument.IsNotNull(initialItems, nameof(initialItems));

			var items = notNullInitialItems.Value.ToArray();
			if (items.Length == 0)
			{
				throw new ArgumentException(@"Initial items collection cannot be empty", nameof(initialItems));
			}

			var queueOptions = NotNull<SearchTaskOptions>.Create(options ?? new SearchTaskOptions());

			var queue = NotNull<IBlockingQueue<HierarchyElement<T>>>.Create
				(
					new HierarchicalLimitedBlockingQueue<T>(items, queueOptions.Value.MaxQueueLength,
						queueOptions.Value.MaxHierarchyDepth)
				);

			await Task.Run
				(
					() => { Process(queue, notNullAction, cancellationToken, queueOptions); },
					cancellationToken
				);
		}

		private static void Process<T>
			(
			NotNull<IBlockingQueue<HierarchyElement<T>>> queue,
			NotNull<Func<T, CancellationToken, IEnumerable<T>>> action,
			CancellationToken cancellationToken,
			NotNull<SearchTaskOptions> options
			)
		{
			var actions = new Action[options.Value.MaxDegreeOfParallelism];

			for (var i = 0; i < actions.Length; i++)
			{
				actions[i] = async () =>
				{
					try
					{
						await ExecuteTask
							(
								queue,
								options.Value.DequeueWaitTimeout,
								action,
								cancellationToken
							);
					}
					catch (OperationCanceledException)
					{
						// It's just cancelling, no errors.
					}
				};
			}

			ParallelTask
				.StartNew(actions)
				.Wait(CancellationToken.None);
		}


		private static async Task ExecuteTask<T>
			(
			NotNull<IBlockingQueue<HierarchyElement<T>>> queue,
			int dequeueWaitTimeout,
			NotNull<Func<T, CancellationToken, IEnumerable<T>>> action,
			CancellationToken cancellationToken
			)
		{
			while (!queue.Value.IsCompleted)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}

				var processItem = false;
				var item = default(HierarchyElement<T>);
				try
				{
					processItem = queue.Value.TryDequeue(out item);

					if (cancellationToken.IsCancellationRequested)
					{
						break;
					}

					if (processItem)
					{
						foreach (var childItem in action.Value.Invoke(item.Child, cancellationToken))
						{
							if (cancellationToken.IsCancellationRequested)
							{
								break;
							}
							queue.Value.TryEnqueue(new HierarchyElement<T>(item.Child, childItem));
						}
					}
					else
					{
						if (dequeueWaitTimeout > 0)
						{
							await Task.Delay(dequeueWaitTimeout, cancellationToken);
						}
					}
				}
				finally
				{
					if (processItem)
					{
						queue.Value.ReleaseItem(item);
					}
				}
			}
		}
	}
}