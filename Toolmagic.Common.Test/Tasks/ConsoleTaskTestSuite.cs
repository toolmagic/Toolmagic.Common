using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using Toolmagic.Common.IO;
using Toolmagic.Common.Tasks;

namespace Toolmagic.Common.Test.Tasks
{
	[TestFixture]
	public sealed class ConsoleTaskTestSuite
	{
		private void LongRunningAction(CancellationToken cancellationToken)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var i = 0;
			while (i++ < 100)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					Task
						.Delay(5, cancellationToken)
						.Wait(CancellationToken.None);
				}
				catch (AggregateException t)
				{
					if (!(t.InnerException is OperationCanceledException))
					{
						throw;
					}
					break;
				}
				catch (OperationCanceledException)
				{
					break;
				}
			}
		}

		[Test]
		public void ConsoleTaskCompletesItselfTest()
		{
			var cancelKey = new ConsoleKeyInfo('\u0011', ConsoleKey.Q, false, false, true);

			var consoleMock = MockRepository.GenerateStrictMock<IConsole>();
			consoleMock.Stub(x => x.KeyAvailable).Return(true);
			consoleMock
				.Stub(x => x.ReadKey(Arg<bool>.Is.Anything))
				.Return(new ConsoleKeyInfo('A', ConsoleKey.A, false, false, true));

			var task = ConsoleTask.StartNew
				(
					LongRunningAction,
					cancelKey,
					consoleMock
				);
			task.Wait(CancellationToken.None);

			Assert.IsFalse(task.IsCanceled);
		}

		[Test]
		public void ConsoleTaskFailsOnNullActionTest()
		{
			const Action<CancellationToken> nullExecuteAction = null;
			var cancelKey = new ConsoleKeyInfo();

			var aggregateException = Assert.Throws<AggregateException>
				(
					() =>
					{
						ConsoleTask
							.StartNew(nullExecuteAction, cancelKey)
							.Wait(CancellationToken.None);
					}
				);

			var exception = aggregateException.InnerExceptions.FirstOrDefault();
			Assert.IsNotNull(exception);
			Assert.IsInstanceOf<ArgumentNullException>(exception);
			Assert.AreEqual("executeAction", ((ArgumentNullException) exception).ParamName);
		}

		[Test]
		public void ConsoleTaskFailsOnNullConsoleTest()
		{
			Action<CancellationToken> executeAction = cancellationToken => { };
			var cancelKey = new ConsoleKeyInfo();
			const IConsole nullConsole = null;

			var aggregateException = Assert.Throws<AggregateException>
				(
					() =>
					{
						ConsoleTask
							.StartNew(executeAction, cancelKey, nullConsole)
							.Wait(CancellationToken.None);
					}
				);

			var exception = aggregateException.InnerExceptions.FirstOrDefault();
			Assert.IsNotNull(exception);
			Assert.IsInstanceOf<ArgumentNullException>(exception);
			Assert.AreEqual("console", ((ArgumentNullException) exception).ParamName);
		}

		[Test]
		public void ConsoleTaskIsCancelledTest()
		{
			var cancelKey = new ConsoleKeyInfo('\u0011', ConsoleKey.Q, false, false, true);
			var notCancelKey = new ConsoleKeyInfo('A', ConsoleKey.A, false, false, true);

			var consoleMock = MockRepository.GenerateStrictMock<IConsole>();
			consoleMock
				.Expect(x => x.KeyAvailable)
				.Return(false)
				.Return(true)
				.Return(true);

			consoleMock
				.Expect(x => x.ReadKey(Arg<bool>.Is.Anything))
				.Return(notCancelKey)
				.Return(cancelKey);

			var task = ConsoleTask.StartNew
				(
					LongRunningAction,
					cancelKey,
					consoleMock
				);
			try
			{
				task.Wait();
			}
			catch (Exception)
			{
				Assert.IsTrue(task.IsCanceled);
			}
			consoleMock.VerifyAllExpectations();
		}
	}
}