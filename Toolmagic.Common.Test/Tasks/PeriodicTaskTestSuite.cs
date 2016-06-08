using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Toolmagic.Common.Tasks;

namespace Toolmagic.Common.Test.Tasks
{
	[TestFixture]
	public sealed class PeriodicTaskTestSuite
	{
		[Test]
		public void PeriodicTaskBasicScenarioTest()
		{
			var cancellationTokenSource = new CancellationTokenSource();

			var counter = 0;

			var task = PeriodicTask
				.StartNew
				(
					() => { counter++; },
					TimeSpan.FromMilliseconds(10),
					cancellationTokenSource.Token
				);

			task.Wait(TimeSpan.FromMilliseconds(100));

			cancellationTokenSource.Cancel();
			try
			{
				task.Wait(cancellationTokenSource.Token);
			}
			catch (OperationCanceledException)
			{
				// do nothing
			}

			Assert.IsTrue(counter > 2);
		}

		[Test]
		public void PeriodicTaskFailsOnNullActionArgumentTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() =>
					{
						PeriodicTask
							.StartNew
							(
								null,
								TimeSpan.FromMilliseconds(10),
								CancellationToken.None
							)
							.Wait(CancellationToken.None);
					}
				);
			Assert.AreEqual("action", exception.ParamName);
		}

		[Test]
		public void PeriodicTaskFailsOnZeroPeriodArgumentTest()
		{
			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					() =>
					{
						PeriodicTask
							.StartNew
							(
								() => { },
								TimeSpan.Zero,
								CancellationToken.None
							)
							.Wait(CancellationToken.None);
					}
				);
			Assert.AreEqual("period", exception.ParamName);
		}

		[Test]
		public void PeriodicTaskFailsTest()
		{
			var cancellationTokenSource = new CancellationTokenSource();

			var aggregateException = Assert.Throws<AggregateException>
				(
					() =>
					{
						PeriodicTask
							.StartNew
							(
								() => { throw new DivideByZeroException(); },
								TimeSpan.FromMilliseconds(10),
								cancellationTokenSource.Token
							)
							.Wait(cancellationTokenSource.Token);
					}
				);

			var exception = aggregateException.InnerExceptions.FirstOrDefault();
			Assert.IsNotNull(exception);
			Assert.IsInstanceOf<DivideByZeroException>(exception);
		}
	}
}