using System;
using NUnit.Framework;

namespace Toolmagic.Common.Test
{
	[TestFixture]
	public sealed class ArgumentTestSuite
	{
		[TestCase(100, 100, ExpectedResult = true)]
		[TestCase(0, 0, ExpectedResult = true)]
		[TestCase(-1, 0, ExpectedResult = typeof (ArgumentException))]
		[TestCase(99, 100, ExpectedResult = typeof (ArgumentException))]
		public object ArgumentAreEqualInt64Test(long a, long b)
		{
			try
			{
				Argument.AreEqual(a, b, "b");
				return true;
			}
			catch (Exception t)
			{
				return t.GetType();
			}
		}

		[TestCase("100", "100", ExpectedResult = true)]
		[TestCase("", "", ExpectedResult = true)]
		[TestCase(null, null, ExpectedResult = true)]
		[TestCase(null, "", ExpectedResult = typeof (ArgumentException))]
		[TestCase("", "111", ExpectedResult = typeof (ArgumentException))]
		[TestCase("100", "99", ExpectedResult = typeof (ArgumentException))]
		public object ArgumentAreEqualStringTest(string a, string b)
		{
			try
			{
				Argument.AreEqual(a, b, "b");
				return true;
			}
			catch (Exception t)
			{
				return t.GetType();
			}
		}

		[Test]
		public void ArgumentIsInRangeFailsOnTooLargeValueTest()
		{
			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					() =>
					{
						var value = int.MaxValue;

						Argument.IsInRange(value, "argument", 0, 1000);
					}
				);
			Assert.AreEqual(@"argument", exception.ParamName);
		}

		[Test]
		public void ArgumentIsInRangeFailsOnTooSmallValueTest()
		{
			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					() =>
					{
						var value = -10;

						Argument.IsInRange(value, "argument", 0, 1000);
					}
				);
			Assert.AreEqual(@"argument", exception.ParamName);
		}

		[Test]
		public void ArgumentIsInRangeTest()
		{
			var value = 100;

			Argument.IsInRange(value, "value", 0, 1000);
		}

		[Test]
		public void ArgumentIsNotEmptyFailsOnEmptyArgumentTest()
		{
			var exception = Assert.Throws<ArgumentException>
				(
					() => { Argument.IsNotEmpty(string.Empty, "argument"); }
				);
			Assert.AreEqual(@"argument", exception.ParamName);
		}

		[Test]
		public void ArgumentIsNotEmptyFailsOnNullArgumentAndArgumentNameTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() => { Argument.IsNotEmpty(null, null); }
				);
			Assert.AreEqual(@"value", exception.ParamName);
		}

		[Test]
		public void ArgumentIsNotEmptyFailsOnNullArgumentTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() => { Argument.IsNotEmpty(null, "argument"); }
				);
			Assert.AreEqual(@"argument", exception.ParamName);
		}

		[Test]
		public void ArgumentIsNotEmptyFailsOnWhitespaceStringArgumentTest()
		{
			var exception = Assert.Throws<ArgumentException>
				(
					() => { Argument.IsNotEmpty("   ", "argument"); }
				);
			Assert.AreEqual(@"argument", exception.ParamName);
		}

		[Test]
		public void ArgumentIsNotEmptyOnNullArgumentNameTest()
		{
			var argumentValue = "Test";

			var notNullArgument = Argument.IsNotEmpty(argumentValue, null);

			Assert.AreEqual(argumentValue, notNullArgument.Value);
		}

		[Test]
		public void ArgumentIsNotEmptyTest()
		{
			var argument = "Test";

			var notNullArgument = Argument.IsNotEmpty(argument, @"argument");

			Assert.AreEqual(argument, notNullArgument.Value);
		}

		[Test]
		public void ArgumentIsNotNullFailsOnNullArgumentAndArgumentNameTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() => { Argument.IsNotNull((object) null, null); }
				);

			Assert.AreEqual(@"value", exception.ParamName);
		}

		[Test]
		public void ArgumentIsNotNullFailsOnNullArgumentTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() => { Argument.IsNotNull((object) null, @"argument"); }
				);
			Assert.AreEqual(@"argument", exception.ParamName);
		}

		[Test]
		public void ArgumentIsNotNullOnNullArgumentNameTest()
		{
			var argument = new object();

			var notNullArgument = Argument.IsNotNull(argument, null);

			Assert.AreEqual(argument, notNullArgument.Value);
		}

		[Test]
		public void ArgumentIsNotNullTest()
		{
			var argument = new object();

			var notNullArgument = Argument.IsNotNull(argument, "argument");

			Assert.AreEqual(argument, notNullArgument.Value);
		}
	}
}