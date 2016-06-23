using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Toolmagic.Common.Collections;

namespace Toolmagic.Common.Test.Collections
{
	[TestFixture]
	public sealed class BlockingQueueTestSuite
	{
		[Test]
		public void BlockingQueueCreatesWithInitialItemsTest()
		{
			var initialItems = new[] {"1"};

			var queue = new BlockingQueue<string>(initialItems);

			Assert.IsNotNull(queue);
		}

		[Test]
		public void BlockingQueueDequeuesFromEmptyQueueItemsTest()
		{
			var initialItems = new[] {"1", "2", "3"};

			var queue = new BlockingQueue<string>(initialItems);

			while (!queue.IsCompleted)
			{
				string item;
				Assert.IsTrue(queue.TryDequeue(out item));

				queue.ReleaseItem(item);
			}

			string item2;
			Assert.IsFalse(queue.TryDequeue(out item2));
		}

		[Test]
		public void BlockingQueueDequeuesItemsTest()
		{
			var initialItems = new[] {"1", "2", "3"};

			var queue = new BlockingQueue<string>(initialItems);

			var dequeuedItems = new Collection<string>();
			while (!queue.IsCompleted)
			{
				string item;
				Assert.IsTrue(queue.TryDequeue(out item));

				dequeuedItems.Add(item);

				queue.ReleaseItem(item);
			}

			CollectionAssert.AreEqual(initialItems, dequeuedItems);
		}

		[Test]
		public void BlockingQueueEnqueuesAndDequeuesItemsTest()
		{
			var initialItems = new[] {"1", "2", "3"};
			var newItems = new[] {"4", "5", "6"};

			var queue = new BlockingQueue<string>(initialItems);

			newItems.ToList().ForEach(item => { Assert.IsTrue(queue.TryEnqueue(item)); });

			var dequeuedItems = new Collection<string>();
			while (!queue.IsCompleted)
			{
				string item;
				Assert.IsTrue(queue.TryDequeue(out item));

				dequeuedItems.Add(item);

				queue.ReleaseItem(item);
			}

			CollectionAssert.AreEqual(initialItems.Union(newItems), dequeuedItems);
		}

		[Test]
		public void BlockingQueueFailsOnCreatingWithEmptyInitialItemsTest()
		{
			var initialItems = new string[] {};

			var exception = Assert.Throws<ArgumentException>
				(
					// ReSharper disable once ObjectCreationAsStatement
					() => { new BlockingQueue<string>(initialItems); }
				);
			Assert.AreEqual(@"initialItems", exception.ParamName);
		}

		[Test]
		public void BlockingQueueFailsOnCreatingWithNullInitialItemsTest()
		{
			const IEnumerable<string> initialItems = null;

			var exception = Assert.Throws<ArgumentNullException>
				(
					// ReSharper disable once ObjectCreationAsStatement
					() => { new BlockingQueue<string>(initialItems); }
				);
			Assert.AreEqual(@"initialItems", exception.ParamName);
		}
	}
}