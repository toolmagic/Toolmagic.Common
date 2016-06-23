using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolmagic.Common.IO;

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
			//var runningTasksCounter = new InterlockedCounter(0);

			IConsole console = new SystemConsole();

			var actions = new Action[options.Value.MaxDegreeOfParallelism];
			for (var i = 0; i < actions.Length; i++)
			{
				var taskId = i;

				actions[i] = async () =>
				{
					try
					{
						await ExecuteTask
							(
								taskId,
								queue,
								options.Value.DequeueWaitTimeout,
								//runningTasksCounter,
								action,
								cancellationToken,
								console
							);
					}
					catch (OperationCanceledException)
					{
						// It's just cancelling, no errors.
						console.WriteLine("OperationCanceledException occured.");
					}
				};
			}

			//ParallelTask
			//	.StartNew(actions)
			//	.Wait(CancellationToken.None);

			var parallelOptions = new ParallelOptions
			{
				MaxDegreeOfParallelism = options.Value.MaxDegreeOfParallelism
			};

			Parallel.ForEach(actions, parallelOptions, a => a.Invoke());
		}

		private static async Task ExecuteTask<T>
			(
			int taskId,
			NotNull<IHierarchicalQueue<T>> queue,
			int dequeueWaitTimeout,
			//InterlockedCounter runningTasksCounter,
			NotNull<Func<T, CancellationToken, IEnumerable<T>>> action,
			CancellationToken cancellationToken,
			IConsole console
			)
		{
			while (true)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					console.WriteLine("{0}. Completed on cancellation - 1", taskId);
					break;
				}

				if (queue.Value.IsCompleted)
				{
					console.WriteLine("{0}. Completed on IsCompleted = true", taskId);
					break;
				}

				var processItem = false;
				var item = default(T);
				try
				{
					processItem = queue.Value.TryDequeue(out item);

					if (cancellationToken.IsCancellationRequested)
					{
						console.WriteLine("{0}. Completed on cancellation - 2", taskId);
						break;
					}

					if (processItem)
					{
						foreach (var childItem in action.Value.Invoke(item, cancellationToken))
						{
							queue.Value.TryEnqueue(item, childItem);
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
						queue.Value.CompleteItem(item);
					}
				}
			}
			console.WriteLine("{0}. Completed while(true) {}", taskId);
		}
	}
}