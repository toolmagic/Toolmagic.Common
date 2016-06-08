using System;
using System.Collections.Generic;
using NUnit.Framework;
using Toolmagic.Common.Collections;

namespace Toolmagic.Common.Test.Collections
{
	[TestFixture]
	public sealed class MultiKeyDictionaryTestSuite
	{
		private static MultiKeyDictionary<int, string, string> CreateSampleDictionary()
		{
			var dictionary = new MultiKeyDictionary<int, string, string>();

			dictionary.Add(1, "1", "aaa");
			dictionary.Add(2, "2", "bbb");
			dictionary.Add(3, "3", "ccc");

			return dictionary;
		}

		[Test]
		public void MultiKeyDictionaryAddsItemsTest()
		{
			var dictionary = CreateSampleDictionary();

			dictionary.Add(4, string.Empty, "eee");
		}

		[Test]
		[TestCase(1, ExpectedResult = true)]
		[TestCase(2, ExpectedResult = true)]
		[TestCase(3, ExpectedResult = true)]
		[TestCase(1000, ExpectedResult = false)]
		public bool MultiKeyDictionaryContainsKeys1Test(int key1)
		{
			var dictionary = CreateSampleDictionary();

			return dictionary.ContainsKey1(key1);
		}

		[Test]
		[TestCase("1", ExpectedResult = true)]
		[TestCase("2", ExpectedResult = true)]
		[TestCase("3", ExpectedResult = true)]
		[TestCase("1000", ExpectedResult = false)]
		public bool MultiKeyDictionaryContainsKeys2Test(string key2)
		{
			var dictionary = CreateSampleDictionary();

			return dictionary.ContainsKey2(key2);
		}

		[Test]
		public void MultiKeyDictionaryCreatesWithIntegersTest()
		{
			Assert.IsNotNull(new MultiKeyDictionary<int, int, int>());
		}

		[Test]
		public void MultiKeyDictionaryCreatesWithObjectsTest()
		{
			Assert.IsNotNull(new MultiKeyDictionary<object, object, object>());
		}

		[Test]
		[TestCase(1, "4", ExpectedResult = typeof (ArgumentException))] // duplicate key1
		[TestCase(5, "1", ExpectedResult = typeof (ArgumentException))] // duplicate key2
		[TestCase(6, null, ExpectedResult = typeof (ArgumentNullException))] // key2 == null
		public Type MultiKeyDictionaryFailsAddingOnInvalidArgumentsTest(int key1, string key2)
		{
			var dictionary = CreateSampleDictionary();

			try
			{
				dictionary.Add(key1, key2, "sample");
				throw new Exception("Add() must fail but it's successfully executed!");
			}
			catch (Exception t)
			{
				return t.GetType();
			}
		}

		[Test]
		public void MultiKeyDictionaryFailsOnRemovesByUnknownKey1Test()
		{
			var dictionary = CreateSampleDictionary();
			var initialCount = dictionary.Count;

			Assert.Throws<KeyNotFoundException>
				(
					() => { dictionary.RemoveByKey1(1000); }
				);

			Assert.AreEqual(initialCount, dictionary.Count);
		}

		[Test]
		public void MultiKeyDictionaryFailsOnRemovesByUnknownKey2Test()
		{
			var dictionary = CreateSampleDictionary();
			var initialCount = dictionary.Count;

			Assert.Throws<KeyNotFoundException>
				(
					() => { dictionary.RemoveByKey2("1000"); }
				);

			Assert.AreEqual(initialCount, dictionary.Count);
		}

		[Test]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		public void MultiKeyDictionaryRemovesByKey1Test(int key1)
		{
			var dictionary = CreateSampleDictionary();
			var initialCount = dictionary.Count;

			dictionary.RemoveByKey1(key1);

			Assert.AreEqual(initialCount - 1, dictionary.Count);

			string value;
			Assert.IsFalse(dictionary.TryGetValueByKey1(key1, out value));
		}

		[Test]
		[TestCase("1")]
		[TestCase("2")]
		[TestCase("3")]
		public void MultiKeyDictionaryRemovesByKey2Test(string key2)
		{
			var dictionary = CreateSampleDictionary();
			var initialCount = dictionary.Count;

			dictionary.RemoveByKey2(key2);

			Assert.AreEqual(initialCount - 1, dictionary.Count);

			string value;
			Assert.IsFalse(dictionary.TryGetValueByKey2(key2, out value));
		}

		[Test]
		[TestCase("1", 1, ExpectedResult = true)]
		[TestCase("2", 2, ExpectedResult = true)]
		[TestCase("3", 3, ExpectedResult = true)]
		[TestCase("1000", default(int), ExpectedResult = false)]
		public bool MultiKeyDictionaryReturnsKey1Test(string key2, int expectedKey1)
		{
			var dictionary = CreateSampleDictionary();

			int actualKey1;
			var result = dictionary.TryGetKey1(key2, out actualKey1);
			Assert.AreEqual(expectedKey1, actualKey1);

			return result;
		}

		[Test]
		[TestCase(1, "1", ExpectedResult = true)]
		[TestCase(2, "2", ExpectedResult = true)]
		[TestCase(3, "3", ExpectedResult = true)]
		[TestCase(1000, default(string), ExpectedResult = false)]
		public bool MultiKeyDictionaryReturnsKey2Test(int key1, string expectedKey2)
		{
			var dictionary = CreateSampleDictionary();

			string actualKey2;
			var result = dictionary.TryGetKey2(key1, out actualKey2);
			Assert.AreEqual(expectedKey2, actualKey2);

			return result;
		}

		[Test]
		public void MultiKeyDictionaryReturnsKeys1Test()
		{
			var dictionary = CreateSampleDictionary();

			CollectionAssert.AreEquivalent(new[] {1, 2, 3}, dictionary.Keys1);
		}

		[Test]
		public void MultiKeyDictionaryReturnsKeys2Test()
		{
			var dictionary = CreateSampleDictionary();

			CollectionAssert.AreEquivalent(new[] {"1", "2", "3"}, dictionary.Keys2);
		}

		[Test]
		[TestCase(1, "aaa", ExpectedResult = true)]
		[TestCase(2, "bbb", ExpectedResult = true)]
		[TestCase(3, "ccc", ExpectedResult = true)]
		[TestCase(1000, default(string), ExpectedResult = false)]
		public bool MultiKeyDictionaryReturnsValueByKey1Test(int key1, string expectedValue)
		{
			var dictionary = CreateSampleDictionary();

			string actualValue;
			var result = dictionary.TryGetValueByKey1(key1, out actualValue);
			Assert.AreEqual(expectedValue, actualValue);

			return result;
		}

		[Test]
		[TestCase("1", "aaa", ExpectedResult = true)]
		[TestCase("2", "bbb", ExpectedResult = true)]
		[TestCase("3", "ccc", ExpectedResult = true)]
		[TestCase("1000", default(string), ExpectedResult = false)]
		public bool MultiKeyDictionaryReturnsValueByKey2Test(string key2, string expectedValue)
		{
			var dictionary = CreateSampleDictionary();

			string actualValue;
			var result = dictionary.TryGetValueByKey2(key2, out actualValue);
			Assert.AreEqual(expectedValue, actualValue);

			return result;
		}

		[Test]
		public void MultiKeyDictionaryReturnsValuesTest()
		{
			var dictionary = CreateSampleDictionary();

			CollectionAssert.AreEquivalent(new[] {"aaa", "bbb", "ccc"}, dictionary.Values);
		}

		[Test]
		public void MultiyKeyDictionaryCreatesWithStringsTest()
		{
			Assert.IsNotNull(new MultiKeyDictionary<string, string, string>());
		}
	}
}