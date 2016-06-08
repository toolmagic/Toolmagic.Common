using System;
using NUnit.Framework;

namespace Toolmagic.Common.Test
{
	[TestFixture]
	public sealed class NotNullTestSuite
	{
		[Test]
		public void NotEmptyStringConvertsToStringTest()
		{
			var expectedValue = new Version(1, 2, 3, 4);

			var notNullValue = NotNull.Wrap(expectedValue);

			Assert.AreEqual(expectedValue.ToString(), notNullValue.ToString());
		}

		[Test]
		public void NotNullFailsOnNullValueArgumentTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() => { NotNull<object>.Create(null); }
				);
			Assert.AreEqual(@"value", exception.ParamName);
		}

		[Test]
		public void NotNullTest()
		{
			var obj = new object();

			var notNullObj = NotNull<object>.Create(obj);

			Assert.IsNotNull(notNullObj);
			Assert.IsNotNull(notNullObj.Value);
		}
	}
}