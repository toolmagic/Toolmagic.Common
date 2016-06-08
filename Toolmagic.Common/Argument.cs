using System;

namespace Toolmagic.Common
{
	public static class Argument
	{
		public static NotNull<T> IsNotNull<T>(T argumentValue, string argumentName) where T : class
		{
			return NotNull<T>.Create(argumentValue, argumentName);
		}

		public static NotEmpty<string> IsNotEmpty(string argumentValue, string argumentName)
		{
			return NotEmpty<string>.Create(argumentValue, argumentName);
		}

		public static void IsInRange<T>(T argumentValue, string argumentName, T minValue, T maxValue) where T : IComparable<T>
		{
			IsNotNull<IComparable<T>>(argumentValue, argumentName);

			if (argumentValue.CompareTo(minValue) < 0 || argumentValue.CompareTo(maxValue) > 0)
			{
				throw new ArgumentOutOfRangeException(GetArgumentName(argumentName, "argumentValue"));
			}
		}

		public static void AreEqual<T>(T expectedValue, T actualValue, string argumentName) where T : IComparable<T>
		{
			if (expectedValue == null && actualValue == null)
			{
				return;
			}

			if (expectedValue != null)
			{
				if (expectedValue.CompareTo(actualValue) != 0)
				{
					throw new ArgumentException(GetArgumentName(argumentName, "argumentValue"));
				}
				return;
			}

			if (actualValue != null && actualValue.CompareTo(expectedValue) != 0)
			{
				throw new ArgumentException(GetArgumentName(argumentName, "argumentValue"));
			}
		}

		internal static string GetArgumentName(string customName, string defaultName)
		{
			return
				string.IsNullOrWhiteSpace(customName)
					? defaultName
					: customName
				;
		}
	}
}