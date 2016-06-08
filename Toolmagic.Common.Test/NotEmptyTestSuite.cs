using System;
using NUnit.Framework;

namespace Toolmagic.Common.Test
{
	[TestFixture]
	public sealed class NotEmptyTestSuite
	{
		[Test]
		public void NotEmptyStringCastsImplicitlyToStringTest()
		{
			const string expectedValue = "test";

			var notEmptyValue = NotEmpty.Wrap(expectedValue);
			string actualValue = notEmptyValue;

			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void NotEmptyStringConvertsToStringTest()
		{
			const string expectedValue = "test";

			var notEmptyValue = NotEmpty.Wrap(expectedValue);

			Assert.AreEqual(expectedValue, notEmptyValue.ToString());
		}

		[Test]
		public void NotEmptyStringFailsOnEmptyStringTest()
		{
			var exception = Assert.Throws<ArgumentException>
				(
					() => { NotEmpty.Wrap(string.Empty); }
				);
			Assert.AreEqual(@"value", exception.ParamName);
		}

		[Test]
		public void NotEmptyStringFailsOnNotStringGenericTypeTest()
		{
			// This code will not be compiled because NotEmpty<T> supports T == string only!
			// NotEmpty<object>.Create(new object());
		}

		[Test]
		public void NotEmptyStringFailsOnNullStringTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() => { NotEmpty.Wrap(null); }
				);
			Assert.AreEqual(@"value", exception.ParamName);
		}

		[Test]
		public void NotEmptyStringFailsOnWhitespaceStringTest()
		{
			var exception = Assert.Throws<ArgumentException>
				(
					() => { NotEmpty.Wrap(" "); }
				);
			Assert.AreEqual(@"value", exception.ParamName);
		}

		[Test]
		public void NotEmptyStringTest()
		{
			var expected = "Test";

			var notEmptyString = NotEmpty<string>.Create(expected);

			Assert.IsNotNull(notEmptyString);
			Assert.AreEqual(expected, notEmptyString.Value);
		}
	}
}