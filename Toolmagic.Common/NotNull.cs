using System;

namespace Toolmagic.Common
{
	public static class NotNull
	{
		public static NotNull<T> Wrap<T>(T value) where T : class
		{
			return NotNull<T>.Create(value);
		}
	}

	public struct NotNull<T> where T : class
	{
		private NotNull(T value)
			: this()
		{
			Value = value;
		}

		public T Value { get; }

		public static NotNull<T> Create(T value, string argumentName = null)
		{
			if (value == null)
			{
				throw new ArgumentNullException(Argument.GetArgumentName(argumentName, "value"));
			}

			return new NotNull<T>(value);
		}

		public static explicit operator NotNull<T>(T value)
		{
			return Create(value);
		}

		public static implicit operator T(NotNull<T> notNullValue)
		{
			return notNullValue.Value;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}