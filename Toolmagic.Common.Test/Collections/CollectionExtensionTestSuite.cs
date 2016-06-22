using System;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Toolmagic.Common.Collections;

namespace Toolmagic.Common.Test.Collections
{
	[TestFixture]
	public sealed class CollectionExtensionTestSuite
	{
		[TestCase(0, ExpectedResult = typeof(ArgumentOutOfRangeException))]
		[TestCase(-1, ExpectedResult = typeof(ArgumentOutOfRangeException))]
		public object CollectionSplitFailsWithIncorrectArgumentsTest(int chunkSize)
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
		public void CollectionAddsEmptyRangeTest()
		{
			var source = new Collection<string> {"1", "2"};
			var collection = new string[] {};

			var target = source.AddRange(collection);

			Assert.IsNotNull(target);
			CollectionAssert.AreEqual(source, target);
		}

		[Test]
		public void CollectionAddsRangeTest()
		{
			var source = new Collection<string> {"1", "2"};
			var collection = new[] {"3", "4", "5"};

			const int skipElements = 1;
			var takeElements = collection.Length - 2;

			var target = source.AddRange(collection, skipElements, takeElements);

			Assert.IsNotNull(target);
			CollectionAssert.AreEqual(source.Union(collection.Skip(skipElements).Take(takeElements)), target);
		}

		[Test]
		public void CollectionAddsTotalRangeTest()
		{
			var source = new Collection<string> {"1", "2"};
			var collection = new[] {"3", "4", "5"};

			var target = source.AddRange(collection);

			Assert.IsNotNull(target);
			CollectionAssert.AreEqual(source.Union(collection), target);
		}

		[Test]
		public void CollectionFailsOnAddingRangeToFixedSizeCollectionTest()
		{
			var source = new[] {"1", "2"};
			var collection = new[] {"3", "4", "5"};

			var exception = Assert.Throws<ArgumentException>(() => { source.AddRange(collection, 0, collection.Length); });
			Assert.AreEqual(@"source", exception.ParamName);
		}

		[Test]
		public void CollectionFailsOnAddingTotalRangeToFixedSizeCollectionTest()
		{
			var source = new[] {"1", "2"};
			var collection = new[] {"3", "4", "5"};

			var exception = Assert.Throws<ArgumentException>(() => { source.AddRange(collection); });
			Assert.AreEqual(@"source", exception.ParamName);
		}

		[Test]
		public void CollectionSplitTest()
		{
			var collection = new[] {"1", "2", "3", "4", "5", "6", "7", "8"};

			var chunks = collection.Split(3).ToList();

			CollectionAssert.AreEquivalent(new[] {"1", "2", "3"}, chunks.First());
			CollectionAssert.AreEquivalent(new[] {"4", "5", "6"}, chunks.ElementAt(1));
			CollectionAssert.AreEquivalent(new[] {"7", "8"}, chunks.Last());
		}

		[Test]
		public void CollectionSplitToEachElementTest()
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
		public void CollectionSplitWithTooLargeChunkSizeTest()
		{
			var collection = new[] {"1", "2", "3", "4", "5", "6", "7", "8"};

			var chunks = collection.Split(int.MaxValue).ToList();

			Assert.AreEqual(1, chunks.Count);
			CollectionAssert.AreEquivalent(collection, chunks.First());
		}
	}
}