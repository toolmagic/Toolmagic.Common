using System;
using System.Linq;
using NUnit.Framework;
using Toolmagic.Common.Collections;

namespace Toolmagic.Common.Test.Collections
{
	[TestFixture]
	public sealed class CollectionExtensionTestSuite
	{
		[TestCase(0, ExpectedResult = typeof (ArgumentOutOfRangeException))]
		[TestCase(-1, ExpectedResult = typeof (ArgumentOutOfRangeException))]
		public object SplitFailedWithIncorrectArgumentsTest(int chunkSize)
		{
			var collection = new[] {"1", "2", "3", "4", "5", "6", "7", "8"};

			try
			{
				return collection.Split(chunkSize);
			}
			catch (Exception t)
			{
				return t.GetType();
			}
		}

		[Test]
		public void SplitTest()
		{
			var collection = new[] {"1", "2", "3", "4", "5", "6", "7", "8"};

			var chunks = collection.Split(3).ToList();

			CollectionAssert.AreEquivalent(new[] {"1", "2", "3"}, chunks.First());
			CollectionAssert.AreEquivalent(new[] {"4", "5", "6"}, chunks.ElementAt(1));
			CollectionAssert.AreEquivalent(new[] {"7", "8"}, chunks.Last());
		}

		[Test]
		public void SplitToEachElementTest()
		{
			var collection = new[] {"1", "2", "3", "4", "5", "6", "7", "8"};

			var chunks = collection.Split(1).ToList();

			Assert.AreEqual(collection.Length, chunks.Count);
			for (var i = 0; i < collection.Length; i++)
			{
				CollectionAssert.AreEquivalent(new[] {collection[i]}, chunks[i]);
			}
		}

		[Test]
		public void SplitWithTooLargeChunkSizeTest()
		{
			var collection = new[] {"1", "2", "3", "4", "5", "6", "7", "8"};

			var chunks = collection.Split(int.MaxValue).ToList();

			Assert.AreEqual(1, chunks.Count);
			CollectionAssert.AreEquivalent(collection, chunks.First());
		}
	}
}