using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using Toolmagic.Common.Tasks;
using Toolmagic.Common.Threading;

namespace Toolmagic.Common.Test.Threading
{
	[TestFixture]
	public sealed class PriorityReaderWriterLockTestSuite
	{
		private enum ResourceTaskOperation
		{
			Read,
			Write
		}

		private struct ResourceTaskInfo
		{
			public readonly ResourceTaskOperation Operation;
			public readonly int StartDelay;
			public readonly int TaskTime;
			public readonly long ExpectedMinTime;
			public readonly long ExpectedMaxTime;

			public ResourceTaskInfo(ResourceTaskOperation operation, int startDelay, int taskTime,
				long expectedMinTime, long expectedMaxTime)
			{
				Operation = operation;
				StartDelay = startDelay;
				TaskTime = taskTime;
				ExpectedMinTime = expectedMinTime;
				ExpectedMaxTime = expectedMaxTime;
			}
		}

		private void ExecuteLockingStrategyTest(ReadWritePriority priority, IDictionary<int, ResourceTaskInfo> tasks)
		{
			using (var resourceTasks = new ResourceTasks(priority))
			{
				foreach (var taskInfo in tasks)
				{
					switch (taskInfo.Value.Operation)
					{
						case ResourceTaskOperation.Read:
							resourceTasks.AddReadTask(taskInfo.Key, taskInfo.Value.StartDelay, taskInfo.Value.TaskTime);
							break;

						case ResourceTaskOperation.Write:
							resourceTasks.AddWriteTask(taskInfo.Key, taskInfo.Value.StartDelay, taskInfo.Value.TaskTime);
							break;

						default:
							throw new NotSupportedException($"Unknown operation: {taskInfo.Value.Operation}");
					}
				}

				resourceTasks.Invoke();

				foreach (var lockTime in resourceTasks.LockTimes)
				{
					System.Console.WriteLine($"TaskId: {lockTime.Key}, LockTime: {lockTime.Value}");
				}

				foreach (var lockTime in resourceTasks.LockTimes)
				{
					var expectedMinTime = tasks[lockTime.Key].ExpectedMinTime;
					var expectedMaxTime = tasks[lockTime.Key].ExpectedMaxTime;

					Assert.IsTrue
						(
							lockTime.Value >= expectedMinTime && lockTime.Value < expectedMaxTime,
							$"TaskId: {lockTime.Key}. Expected [{expectedMinTime}..{expectedMaxTime}] lock time but actual time is {lockTime.Value} ms."
						);
				}
			}
		}

		[Test]
		public void DefaultLockingStrategyTest()
		{
			IDictionary<int, ResourceTaskInfo> tasks = new Dictionary<int, ResourceTaskInfo>
			{
				{0, new ResourceTaskInfo(ResourceTaskOperation.Read, 0, 100, 0, 10)},
				{1, new ResourceTaskInfo(ResourceTaskOperation.Read, 0, 200, 0, 10)},
				{2, new ResourceTaskInfo(ResourceTaskOperation.Read, 0, 300, 0, 10)},
				{3, new ResourceTaskInfo(ResourceTaskOperation.Write, 5, 2000, 290, 5000)},
				{4, new ResourceTaskInfo(ResourceTaskOperation.Write, 10, 2000, 285, 5000)},
				{5, new ResourceTaskInfo(ResourceTaskOperation.Write, 15, 2000, 280, 5000)},
				{6, new ResourceTaskInfo(ResourceTaskOperation.Read, 20, 100, 8000, 9000)},
				{7, new ResourceTaskInfo(ResourceTaskOperation.Read, 20, 200, 8000, 9000)},
				{8, new ResourceTaskInfo(ResourceTaskOperation.Read, 20, 300, 8000, 9000)},
				{9, new ResourceTaskInfo(ResourceTaskOperation.Write, 30, 2000, 6000, 7000)},
				{10, new ResourceTaskInfo(ResourceTaskOperation.Read, 3000, 400, 5000, 6000)},
				{11, new ResourceTaskInfo(ResourceTaskOperation.Read, 3000, 100, 5000, 6000)}
			};

			ExecuteLockingStrategyTest(ReadWritePriority.Default, tasks);
		}


		[Test]
		public void PriorityReaderWriterLockSupportsAllPriorityStrategiesTest()
		{
			foreach (var value in typeof(ReadWritePriority).GetEnumValues())
			{
				var priority = (ReadWritePriority) value;
				using (var rwLock = Disposable.Wrap(new PriorityReaderWriterLock(priority)))
				{
					Assert.AreEqual(priority, rwLock.Value.Priority);
				}
			}
		}

		[Test]
		public void ReadFirstLockingStrategyTest()
		{
			IDictionary<int, ResourceTaskInfo> tasks = new Dictionary<int, ResourceTaskInfo>
			{
				{0, new ResourceTaskInfo(ResourceTaskOperation.Read, 0, 100, 0, 10)},
				{1, new ResourceTaskInfo(ResourceTaskOperation.Read, 0, 200, 0, 10)},
				{2, new ResourceTaskInfo(ResourceTaskOperation.Read, 0, 300, 0, 10)},
				{3, new ResourceTaskInfo(ResourceTaskOperation.Write, 5, 2000, 295, 500)},
				{4, new ResourceTaskInfo(ResourceTaskOperation.Write, 10, 2000, 1690, 2500)},
				{5, new ResourceTaskInfo(ResourceTaskOperation.Write, 15, 2000, 3685, 5000)},
				{6, new ResourceTaskInfo(ResourceTaskOperation.Read, 20, 100, 0, 10)},
				{7, new ResourceTaskInfo(ResourceTaskOperation.Read, 20, 200, 0, 10)},
				{8, new ResourceTaskInfo(ResourceTaskOperation.Read, 20, 300, 0, 10)},
				{9, new ResourceTaskInfo(ResourceTaskOperation.Write, 30, 2000, 6000, 7000)},
				{10, new ResourceTaskInfo(ResourceTaskOperation.Read, 3000, 400, 1000, 2000)},
				{11, new ResourceTaskInfo(ResourceTaskOperation.Read, 3000, 100, 1000, 2000)}
			};

			ExecuteLockingStrategyTest(ReadWritePriority.ReadFirst, tasks);
		}
	}

	public sealed class ResourceTasks : IDisposable
	{
		private readonly ICollection<Action> _actions = new Collection<Action>();
		private readonly CommonResource _resource;
		public readonly ConcurrentDictionary<int, long> LockTimes = new ConcurrentDictionary<int, long>();

		public ResourceTasks(ReadWritePriority priority)
		{
			_resource = new CommonResource(priority);
		}

		public void Dispose()
		{
			_resource.Dispose();
		}

		public void AddReadTask(int taskId, int startDelay, int readTime)
		{
			_actions.Add(() =>
			{
				Thread.Sleep(startDelay);
				var lockTime = _resource.GetReadLockTime(readTime);
				Assert.IsTrue(LockTimes.TryAdd(taskId, lockTime));
			});
		}

		public void AddWriteTask(int taskId, int startDelay, int writeTime)
		{
			_actions.Add(() =>
			{
				Thread.Sleep(startDelay);
				var lockTime = _resource.GetWriteLockTime(writeTime);
				Assert.IsTrue(LockTimes.TryAdd(taskId, lockTime));
			});
		}

		public void Invoke()
		{
			ParallelTask
				.StartNew(_actions)
				.Wait();
		}
	}

	public sealed class CommonResource : IDisposable
	{
		private readonly PriorityReaderWriterLock _lockObject;

		public CommonResource(ReadWritePriority priority)
		{
			_lockObject = new PriorityReaderWriterLock(priority);
		}

		public void Dispose()
		{
			_lockObject.Dispose();
		}

		public long GetWriteLockTime(int delay)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			_lockObject.EnterWriteLock();
			try
			{
				stopwatch.Stop();

				Thread.Sleep(delay);

				return stopwatch.ElapsedMilliseconds;
			}
			finally
			{
				_lockObject.ExitWriteLock();
			}
		}

		public long GetReadLockTime(int delay)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			_lockObject.EnterReadLock();
			try
			{
				stopwatch.Stop();

				Thread.Sleep(delay);

				return stopwatch.ElapsedMilliseconds;
			}
			finally
			{
				_lockObject.ExitReadLock();
			}
		}
	}
}