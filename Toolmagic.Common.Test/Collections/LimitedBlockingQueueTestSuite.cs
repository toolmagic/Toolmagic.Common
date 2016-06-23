using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Toolmagic.Common.Collections;

namespace Toolmagic.Common.Test.Collections
{
	[TestFixture]
	public sealed class LimitedBlockingQueueTestSuite
	{
		[Test]
		public void LimitedBlockingQueueCreatesTest()
		{
			var initialItems = new[] {"1", "2", "3"};

			var queue = new LimitedBlockingQueue<string>(initialItems, int.MaxValue);

			Assert.IsNotNull(queue);
		}

		[Test]
		public void LimitedBlockingQueueCreationFailsOnEmptyInitialItemsTest()
		{
			var initialItems = new string[] {};

			var exception = Assert.Throws<ArgumentException>(() =>
			{
				// ReSharper disable once ObjectCreationAsStatement
				new LimitedBlockingQueue<string>(initialItems, int.MaxValue);
			});

			Assert.AreEqual(@"initialItems", exception.ParamName);
		}

		[Test]
		[TestCase(-1)]
		[TestCase(0)]
		public void LimitedBlockingQueueCreationFailsOnIncorrectMaxItemCountTest(int maxItemCount)
		{
			var initialItems = new[] {"1"};

			var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				// ReSharper disable once ObjectCreationAsStatement
				new LimitedBlockingQueue<string>(initialItems, -1);
			});

			Assert.AreEqual(@"maxItemCount", exception.ParamName);
		}

		[Test]
		public void LimitedBlockingQueueCreationFailsOnNullInitialItemsTest()
		{
			const IEnumerable<string> initialItems = null;

			var exception = Assert.Throws<ArgumentNullException>(() =>
			{
				// ReSharper disable once ObjectCreationAsStatement
				new LimitedBlockingQueue<string>(initialItems, int.MaxValue);
			});

			Assert.AreEqual(@"initialItems", exception.ParamName);
		}

		[Test]
		public void LimitedBlockingQueueSkipsSomeEnqueuedItemsTest()
		{
			var initialItems = new[] {"1", "2", "3"};
			var takeCount = initialItems.Length + 1;

			const string addedItem = "4";
			const string missedItem = "5";

			var queue = new LimitedBlockingQueue<string>(initialItems, takeCount);

			Assert.IsTrue(queue.TryEnqueue(addedItem));
			Assert.IsFalse(queue.TryEnqueue(missedItem));

			var dequeuedItems = new Collection<string>();
			while (!queue.IsCompleted)
			{
				string item;
				Assert.IsTrue(queue.TryDequeue(out item));

				dequeuedItems.Add(item);

				queue.ReleaseItem(item);
			}

			CollectionAssert.AreEqual(initialItems.Union(new[] {addedItem}), dequeuedItems);
		}

		[Test]
		public void LimitedBlockingQueueSkipsSomeInitialItemsTest()
		{
			var initialItems = new[] {"1", "2", "3"};
			var takeCount = initialItems.Length - 1;

			var queue = new LimitedBlockingQueue<string>(initialItems, takeCount);

			var dequeuedItems = new Collection<string>();
			while (!queue.IsCompleted)
			{
				string item;
				Assert.IsTrue(queue.TryDequeue(out item), "Can't' get next after '{0}'", dequeuedItems.LastOrDefault());

				dequeuedItems.Add(item);

				queue.ReleaseItem(item);
			}

			CollectionAssert.AreEqual(initialItems.Take(takeCount), dequeuedItems);
		}

		[Test]
		public void LimitedBlockingQueueTakesAllInitialItemsTest()
		{
			var initialItems = new[] {"1", "2", "3"};

			var queue = new LimitedBlockingQueue<string>(initialItems, int.MaxValue);

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
	}
}