using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Toolmagic.Common.Collections;

namespace Toolmagic.Common.Test.Collections
{
	[TestFixture]
	public sealed class ProducerConsumerQueueTestSuite
	{
		[Test]
		public void ProducerConsumerFailsOnNullConsumerActiontest()
		{
			const Action<string> nullAction = null;

			var exception = Assert.Throws<ArgumentNullException>
				(
					() => ProducerConsumerQueue<string>.Start(nullAction)
				);

			Assert.AreEqual("consumerAction", exception.ParamName);
		}

		[Test]
		public void ProducerConsumerQueueTest()
		{
			const int initialItemCount = 100;
			const int producerDelay = 10;
			const int consumerDelay = producerDelay/5;

			var processedItems = new ConcurrentBag<int>();

			using (var queue = ProducerConsumerQueue<int>.Start(value =>
			{
				processedItems.Add(value);
				System.Console.WriteLine(value);
				Thread.Sleep(producerDelay);
			}))
			{
				Parallel.ForEach
					(
						Enumerable.Range(0, initialItemCount - 1),
						new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
						value =>
						{
							// ReSharper disable once AccessToDisposedClosure
							queue.Enqueue(value);
							Thread.Sleep(consumerDelay);
						}
					);

				Thread.Sleep(500);
				queue.Shutdown();
			}

			Assert.Greater(initialItemCount, processedItems.Count);
		}

		[Test]
		public void ProducerConsumerStartsTest()
		{
			using (var queue = ProducerConsumerQueue<string>.Start(value => { }))
			{
				Assert.IsNotNull(queue);
			}
		}
	}
}