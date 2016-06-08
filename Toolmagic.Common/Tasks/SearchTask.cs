using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
			var notNullAction = Argument.IsNotNull(action, "action");
			var notNullInitialItems = Argument.IsNotNull(initialItems, "initialItems");

			var items = notNullInitialItems.Value.ToArray();

			if (items.Length == 0)
			{
				throw new ArgumentException(@"Initial items collection cannot be empty");
			}

			var queueOptions = NotNull<SearchTaskOptions>.Create(options ?? new SearchTaskOptions());
			var queue = NotNull<IHierarchicalQueue<T>>.Create
				(
					new LimitedHierarchicalQueue<T>(items, queueOptions.Value.MaxHierarchyDepth, queueOptions.Value.MaxQueueLength)
				);

			await Task.Run
				(
					() => { Process(queue, notNullAction, cancellationToken, queueOptions); },
					cancellationToken
				);
		}

		private static void Process<T>
			(
			NotNull<IHierarchicalQueue<T>> queue,
			NotNull<Func<T, CancellationToken, IEnumerable<T>>> action,
			CancellationToken cancellationToken,
			NotNull<SearchTaskOptions> options
			)
		{
			var runningTasksCounter = new InterlockedCounter(0);

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
								runningTasksCounter,
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

			//var parallelOptions = new ParallelOptions
			//{
			//	MaxDegreeOfParallelism = options.Value.MaxDegreeOfParallelism
			//};

			ParallelTask
				.StartNew(actions)
				.Wait(CancellationToken.None);

			//Parallel.ForEach(actions, parallelOptions, a => a.Invoke());
		}

		private static async Task ExecuteTask<T>
			(
			NotNull<IHierarchicalQueue<T>> queue,
			int dequeueWaitTimeout,
			InterlockedCounter runningTasksCounter,
			NotNull<Func<T, CancellationToken, IEnumerable<T>>> action,
			CancellationToken cancellationToken
			)
		{
			while (true)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}

				T item;
				if (!queue.Value.TryDequeue(out item))
				{
					if (cancellationToken.IsCancellationRequested)
					{
						break;
					}

					if (runningTasksCounter.IsZero())
					{
						break;
					}

					if (dequeueWaitTimeout > 0)
					{
						await Task.Delay(dequeueWaitTimeout, cancellationToken);
					}

					continue;
				}

				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}

				try
				{
					runningTasksCounter.Increment();

					foreach (var childItem in action.Value.Invoke(item, cancellationToken))
					{
						if (cancellationToken.IsCancellationRequested)
						{
							break;
						}

						queue.Value.TryEnqueue(item, childItem);
					}
				}
				finally
				{
					runningTasksCounter.Decrement();
				}
			}
		}
	}
}