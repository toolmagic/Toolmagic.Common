using System;
using System.Collections.Generic;
using static System.FormattableString;

namespace Toolmagic.Common
{
	public static class NotEmpty
	{
		public static NotEmpty<string> Wrap(string value)
		{
			return NotEmpty<string>.Create(value);
		}
	}

	public struct NotEmpty<T> where T : /* string */
		IComparable<string>,
		IEquatable<string>,
		IEnumerable<char>
	{
		private NotEmpty(string value)
			: this()
		{
			Value = value;
		}

		public string Value { get; }

		public static NotEmpty<string> Create(string value, string argumentName = null)
		{
			var realArgumentName = Argument.GetArgumentName(argumentName, nameof(value));

			Argument.IsNotNull(value, realArgumentName);

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException(Invariant($"Invalid '{realArgumentName}' argument value: {value}"),
					realArgumentName);
			}

			return new NotEmpty<string>(value);
		}

		public static implicit operator string(NotEmpty<T> notNullValue)
		{
			return notNullValue.Value;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}