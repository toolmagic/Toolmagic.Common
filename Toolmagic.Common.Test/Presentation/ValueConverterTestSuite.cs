using System;
using System.Globalization;
using NUnit.Framework;
using Toolmagic.Common.Presentation;

namespace Toolmagic.Common.Test.Presentation
{
	[TestFixture]
	public sealed class ValueConverterTestSuite
	{
		internal sealed class StringAndDateTimeValueConverter : ValueConverter<string, DateTime>
		{
			protected override DateTime Convert(string value, object parameter, CultureInfo culture)
			{
				Argument.IsNotEmpty(value, nameof(value));
				Argument.IsNotNull(parameter, nameof(parameter));
				Argument.IsNotNull(culture, nameof(culture));

				return DateTime.Parse(value, culture);
			}

			protected override string ConvertBack(DateTime value, object parameter, CultureInfo culture)
			{
				Argument.IsNotNull(parameter, nameof(parameter));
				Argument.IsNotNull(culture, nameof(culture));

				return value.ToString(culture);
			}
		}

		internal sealed class TestExceptionValueConverter : ValueConverter<TestException, Test2Exception>
		{
			protected override Test2Exception Convert(TestException value, object parameter, CultureInfo culture)
			{
				Argument.IsNotNull(parameter, nameof(parameter));
				Argument.IsNotNull(culture, nameof(culture));

				return new Test2Exception(value.Message);
			}

			protected override TestException ConvertBack(Test2Exception value, object parameter, CultureInfo culture)
			{
				Argument.IsNotNull(parameter, nameof(parameter));
				Argument.IsNotNull(culture, nameof(culture));

				return new TestException(value.Message);
			}
		}

		[Test]
		public void ValueConverterConvertsBackDateTimeToStringTest()
		{
			var converter = new StringAndDateTimeValueConverter();
			var dateTime = new DateTime(2012, 09, 28);
			var parameter = new object();

			var result = converter.ConvertBack(dateTime, typeof(string), parameter, CultureInfo.InvariantCulture);

			Assert.IsNotNull(result);
			Assert.IsInstanceOf<string>(result);
			Assert.AreEqual("09/28/2012 00:00:00", (string) result);
		}

		[Test]
		public void ValueConverterConvertsBackSourceToSubclassOfTest2ExceptionTest()
		{
			var converter = new TestExceptionValueConverter();
			var sourceValue = new Test2Exception("test message");
			var parameter = new object();

			var result = converter.ConvertBack(sourceValue, typeof(Exception), parameter, CultureInfo.InvariantCulture);

			Assert.IsNotNull(result);
			Assert.IsInstanceOf<TestException>(result);
			Assert.AreEqual(sourceValue.Message, ((TestException) result).Message);
		}

		[Test]
		public void ValueConverterConvertsSourceToSubclassOfTestExceptionTest()
		{
			var converter = new TestExceptionValueConverter();
			var sourceValue = new TestException("test message");
			var parameter = new object();

			var result = converter.Convert(sourceValue, typeof(Exception), parameter, CultureInfo.InvariantCulture);

			Assert.IsNotNull(result);
			Assert.IsInstanceOf<Test2Exception>(result);
			Assert.AreEqual(sourceValue.Message, ((Test2Exception) result).Message);
		}

		[Test]
		public void ValueConverterConvertsStringToDateTimeTest()
		{
			var converter = new StringAndDateTimeValueConverter();
			const string text = "2012-09-28";
			var parameter = new object();

			var result = converter.Convert(text, typeof(DateTime), parameter, CultureInfo.InvariantCulture);

			Assert.IsNotNull(result);
			Assert.IsInstanceOf<DateTime>(result);
			Assert.AreEqual(DateTime.Parse(text), (DateTime) result);
		}

		[Test]
		public void ValueConverterReturnsNullOnConvertingBackToUnexpectedTargetTypeTest()
		{
			var converter = new StringAndDateTimeValueConverter();
			var dateTime = new DateTime(2012, 09, 28);
			var parameter = new object();

			var result = converter.ConvertBack(dateTime, typeof(TestException), parameter, CultureInfo.InvariantCulture);

			Assert.IsNull(result);
		}

		[Test]
		public void ValueConverterReturnsNullOnConvertingToUnexpectedTargetTypeTest()
		{
			var converter = new StringAndDateTimeValueConverter();
			const string text = "2012-09-28";
			var parameter = new object();

			var result = converter.Convert(text, typeof(TestException), parameter, CultureInfo.InvariantCulture);

			Assert.IsNull(result);
		}
	}
}