using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Toolmagic.Common.Tasks;

namespace Toolmagic.Common.Test.Tasks
{
	[TestFixture]
	public sealed class ParallelTaskTestSuite
	{
		internal sealed class Timing
		{
			public DateTime StartTime { get; set; }
			public DateTime FinishTime { get; set; }
		}

		[Test]
		public void ParallelTaskFailsOnEmptyActionsArgumentTest()
		{
			var task = ParallelTask.StartNew(Enumerable.Empty<Action>());

			Assert.IsNotNull(task);
			Assert.IsNotNull(task.Exception);
			Assert.IsInstanceOf<AggregateException>(task.Exception);

			var aggregateException = task.Exception;
			Assert.AreEqual(1, aggregateException.InnerExceptions.Count);
			Assert.IsInstanceOf<ArgumentException>(aggregateException.InnerExceptions.First());
		}

		[Test]
		public void ParallelTaskFailsOnNegativeParallelCountArgumentTest()
		{
			var task = ParallelTask.StartNew(() => { }, -1);

			Assert.IsNotNull(task);
			Assert.IsNotNull(task.Exception);
			Assert.IsInstanceOf<AggregateException>(task.Exception);

			var aggregateException = task.Exception;
			Assert.AreEqual(1, aggregateException.InnerExceptions.Count);
			Assert.IsInstanceOf<ArgumentOutOfRangeException>(aggregateException.InnerExceptions.First());
		}

		[Test]
		public void ParallelTaskFailsOnNullActionArgumentTest()
		{
			var task = ParallelTask.StartNew(null, 10);

			Assert.IsNotNull(task);
			Assert.IsNotNull(task.Exception);
			Assert.IsInstanceOf<AggregateException>(task.Exception);

			var aggregateException = task.Exception;
			Assert.AreEqual(1, aggregateException.InnerExceptions.Count);
			Assert.IsInstanceOf<ArgumentNullException>(aggregateException.InnerExceptions.First());
		}

		[Test]
		public void ParallelTaskFailsOnNullActionsArgumentTest()
		{
			var task = ParallelTask.StartNew(null);

			Assert.IsNotNull(task);
			Assert.IsNotNull(task.Exception);
			Assert.IsInstanceOf<AggregateException>(task.Exception);

			var aggregateException = task.Exception;
			Assert.AreEqual(1, aggregateException.InnerExceptions.Count);
			Assert.IsInstanceOf<ArgumentNullException>(aggregateException.InnerExceptions.First());
		}

		[Test]
		public void ParallelTaskFailsOnTooShortActionsArgumentTest()
		{
			var task = ParallelTask.StartNew(new Action[] {() => { }});

			Assert.IsNotNull(task);
			Assert.IsNotNull(task.Exception);
			Assert.IsInstanceOf<AggregateException>(task.Exception);

			var aggregateException = task.Exception;
			Assert.AreEqual(1, aggregateException.InnerExceptions.Count);
			Assert.IsInstanceOf<ArgumentOutOfRangeException>(aggregateException.InnerExceptions.First());
		}

		[Test]
		public void ParallelTaskFailsOnTooShortParallelCountArgumentTest()
		{
			var task = ParallelTask.StartNew(() => { }, 1);

			Assert.IsNotNull(task);
			Assert.IsNotNull(task.Exception);
			Assert.IsInstanceOf<AggregateException>(task.Exception);

			var aggregateException = task.Exception;
			Assert.AreEqual(1, aggregateException.InnerExceptions.Count);
			Assert.IsInstanceOf<ArgumentOutOfRangeException>(aggregateException.InnerExceptions.First());
		}

		[Test]
		public void ParallelTaskFailsOnZeroParallelCountArgumentTest()
		{
			var task = ParallelTask.StartNew(() => { }, 0);

			Assert.IsNotNull(task);
			Assert.IsNotNull(task.Exception);
			Assert.IsInstanceOf<AggregateException>(task.Exception);

			var aggregateException = task.Exception;
			Assert.AreEqual(1, aggregateException.InnerExceptions.Count);
			Assert.IsInstanceOf<ArgumentOutOfRangeException>(aggregateException.InnerExceptions.First());
		}

		[Test]
		public void ParallelTaskWithMultipleActionsTest()
		{
			var actions = new Action[10];
			var timings = new ConcurrentBag<Timing>();

			for (var i = 0; i < actions.Length; i++)
			{
				actions[i] = () =>
				{
					var timing = new Timing {StartTime = DateTime.Now};
					Thread.Sleep(500);
					timing.FinishTime = DateTime.Now;

					timings.Add(timing);
				};
			}

			var task = ParallelTask.StartNew(actions);
			task.Wait();

			var maxStartTime = timings.Select(t => t.StartTime).Max();
			var maxFinishTime = timings.Select(t => t.FinishTime).Min();

			foreach (var timing in timings)
			{
				System.Console.WriteLine("{0} - {1}", timing.StartTime, timing.FinishTime);
			}

			Assert.IsTrue(maxStartTime < maxFinishTime);
		}

		[Test]
		public void ParallelTaskWithOneActionTest()
		{
			var timings = new ConcurrentBag<Timing>();

			Action action = () =>
			{
				var timing = new Timing {StartTime = DateTime.Now};
				Thread.Sleep(500);
				timing.FinishTime = DateTime.Now;

				timings.Add(timing);
			};

			var task = ParallelTask.StartNew(action, 10);
			task.Wait();

			var maxStartTime = timings.Select(t => t.StartTime).Max();
			var maxFinishTime = timings.Select(t => t.FinishTime).Min();

			foreach (var timing in timings)
			{
				System.Console.WriteLine("{0} - {1}", timing.StartTime, timing.FinishTime);
			}

			Assert.IsTrue(maxStartTime < maxFinishTime);
		}
	}
}