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
			const Action<string, CancellationToken> nullAction = null;

			var exception = Assert.Throws<ArgumentNullException>
				(
					() => ProducerConsumerQueue<string>.Start(nullAction, Environment.ProcessorCount)
				);

			Assert.AreEqual("consumerAction", exception.ParamName);
		}

		[Test]
		public void ProducerConsumerQueueMultipleConsumersTest()
		{
			const int initialItemCount = 100;
			const int producerDelay = 10;
			const int consumerDelay = producerDelay/5;

			var processedItems = new ConcurrentBag<int>();

			using (var queue = ProducerConsumerQueue<int>.Start((value, cancellationToken) =>
			{
				Thread.Sleep(producerDelay);
				processedItems.Add(value);
				System.Console.WriteLine(value);
			}, Environment.ProcessorCount))
			{
				Parallel.ForEach
					(
						Enumerable.Range(0, initialItemCount),
						new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
						value =>
						{
							// ReSharper disable once AccessToDisposedClosure
							queue.Enqueue(value);
							Thread.Sleep(consumerDelay);
						}
					);

				Thread.Sleep(2*initialItemCount*producerDelay/Environment.ProcessorCount);
				queue.Shutdown();
			}

			Assert.AreEqual(initialItemCount, processedItems.Count);
		}

		[Test]
		public void ProducerConsumerQueueOneConsumerTest()
		{
			const int initialItemCount = 100;
			const int producerDelay = 10;
			const int consumerDelay = producerDelay/5;

			var processedItems = new ConcurrentBag<int>();

			using (var queue = ProducerConsumerQueue<int>.Start((value, cancellationToken) =>
			{
				Thread.Sleep(producerDelay);
				processedItems.Add(value);
				System.Console.WriteLine(value);
			}, 1))
			{
				Parallel.ForEach
					(
						Enumerable.Range(0, initialItemCount),
						new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
						value =>
						{
							// ReSharper disable once AccessToDisposedClosure
							queue.Enqueue(value);
							Thread.Sleep(consumerDelay);
						}
					);

				Thread.Sleep(initialItemCount*consumerDelay);
				queue.Shutdown();
			}

			Assert.Greater(initialItemCount, processedItems.Count);
		}


		[Test]
		public void ProducerConsumerStartsTest()
		{
			Action<string, CancellationToken> consumerAction = (value, cancellationToken) => { };
			var consumerCount = Environment.ProcessorCount;

			using (var queue = ProducerConsumerQueue<string>.Start(consumerAction, consumerCount))
			{
				Assert.IsNotNull(queue);
			}
		}
	}
}