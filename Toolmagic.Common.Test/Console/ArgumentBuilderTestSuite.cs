using System;
using NUnit.Framework;
using Toolmagic.Common.Console;

namespace Toolmagic.Common.Test.Console
{
	[TestFixture]
	public sealed class ArgumentBuilderTestSuite
	{
		[Test]
		[TestCase("/bbb-ccc", ExpectedResult = "/bbb-ccc")]
		[TestCase("bbb ccc", ExpectedResult = "\"bbb ccc\"")]
		[TestCase("bbb\"ccc", ExpectedResult = "\"bbb\"\"ccc\"")]
		[TestCase(null, ExpectedResult = typeof (ArgumentNullException))]
		[TestCase("", ExpectedResult = typeof (ArgumentException))]
		[TestCase("   ", ExpectedResult = typeof (ArgumentException))]
		public object ArgumentBuilderAddsArgumentTest(string argumentValue)
		{
			var argumentBuilder = new ArgumentBuilder();
			Assert.IsNotNull(argumentBuilder);

			try
			{
				argumentBuilder.AddArgument(argumentValue);
			}
			catch (Exception t)
			{
				return t.GetType();
			}

			return argumentBuilder.ToString();
		}

		[Test]
		[TestCase("--bbb", "cccddd", ExpectedResult = "--bbb cccddd")]
		[TestCase("--bbb", "ccc ddd", ExpectedResult = "--bbb \"ccc ddd\"")]
		[TestCase("--bbb", "ccc\"ddd", ExpectedResult = "--bbb \"ccc\"\"ddd\"")]
		[TestCase(null, "cccddd", ExpectedResult = typeof (ArgumentNullException))] // null name
		[TestCase("", "cccddd", ExpectedResult = typeof (ArgumentException))] // empty name
		[TestCase("aaa aaa", "cccddd", ExpectedResult = typeof (ArgumentException))] // space in name
		[TestCase("aaa\"aaa", "cccddd", ExpectedResult = typeof (ArgumentException))] // quote in name
		public object ArgumentBuilderAddsOptionTest(string name, string value)
		{
			var argumentBuilder = new ArgumentBuilder();
			Assert.IsNotNull(argumentBuilder);

			try
			{
				argumentBuilder.AddOption(name, value);
			}
			catch (Exception t)
			{
				return t.GetType();
			}
			return argumentBuilder.ToString();
		}

		[Test]
		[TestCase("/bbb-ccc", ExpectedResult = "/bbb-ccc")]
		[TestCase("bbb ccc", ExpectedResult = typeof (ArgumentException))] // space in name
		[TestCase("bbb\"ccc", ExpectedResult = typeof (ArgumentException))] // quote is name
		public object ArgumentBuilderAddsSwitchTest(string switchValue)
		{
			var argumentBuilder = new ArgumentBuilder();
			Assert.IsNotNull(argumentBuilder);

			try
			{
				argumentBuilder.AddSwitch(switchValue);
			}
			catch (Exception t)
			{
				return t.GetType();
			}

			return argumentBuilder.ToString();
		}

		[Test]
		public void ArgumentBuilderCreatesEmptyTest()
		{
			var argumentBuilder = new ArgumentBuilder();

			Assert.IsNotNull(argumentBuilder);
			Assert.AreEqual(string.Empty, argumentBuilder.ToString());
		}

		[Test]
		public void ArgumentBuilderCreatesWithSourceAndOptionDelimiterTest()
		{
			var argumentBuilder = new ArgumentBuilder("aaa", "=");

			Assert.IsNotNull(argumentBuilder);

			argumentBuilder.AddOption("--bbb", "ccc");
			Assert.AreEqual("aaa --bbb=ccc", argumentBuilder.ToString());
		}

		[Test]
		public void ArgumentBuilderCreatesWithSourceTest()
		{
			const string sourceValue = "aaa";
			var argumentBuilder = new ArgumentBuilder(sourceValue);

			Assert.IsNotNull(argumentBuilder);
			Assert.AreEqual(sourceValue, argumentBuilder.ToString());
		}
	}
}