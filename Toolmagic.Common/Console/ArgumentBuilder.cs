using System;
using System.Text;

namespace Toolmagic.Common.Console
{
	public sealed class ArgumentBuilder
	{
		private readonly string _optionNameValueSeparator;
		private readonly StringBuilder _stringBuilder = new StringBuilder();

		public ArgumentBuilder(string sourceValue, string optionNameValueSeparator = @" ")
		{
			Argument.IsNotNull(sourceValue, nameof(sourceValue));
			Argument.IsNotNull(optionNameValueSeparator, nameof(optionNameValueSeparator));

			_stringBuilder.Append(sourceValue);
			_optionNameValueSeparator = optionNameValueSeparator;
		}

		public ArgumentBuilder()
			: this(string.Empty)
		{
		}

		public ArgumentBuilder AddSwitch(string value)
		{
			var notEmptyValue = Argument.IsNotEmpty(value, nameof(value));
			CheckArgumentNameOrSwitch(notEmptyValue, nameof(value));

			_stringBuilder.AppendFormat(" {0}", notEmptyValue.Value);

			return this;
		}

		public ArgumentBuilder AddOption(string name, string value)
		{
			var notEmptyName = Argument.IsNotEmpty(name, nameof(name));
			CheckArgumentNameOrSwitch(notEmptyName, nameof(name));

			var notNullValue = Argument.IsNotNull(value, nameof(value));

			_stringBuilder.AppendFormat(" {0}{1}{2}", notEmptyName.Value, _optionNameValueSeparator, EscapeString(notNullValue));

			return this;
		}

		public ArgumentBuilder AddArgument(string arg)
		{
			Argument.IsNotEmpty(arg, @"arg");

			_stringBuilder.AppendFormat(" {0}", EscapeString(NotNull.Wrap(arg)));

			return this;
		}

		private static void CheckArgumentNameOrSwitch(NotEmpty<string> value, string argumentName)
		{
			if (value.Value.Contains(" ") || value.Value.Contains("\""))
			{
				throw new ArgumentException(
					$"Switch or argument name '{value.Value}' shouldn't contain spaces or quotes.", argumentName);
			}
		}

		private static string EscapeString(NotNull<string> value)
		{
			if (value.Value.Contains(" ") || value.Value.Contains("\""))
			{
				return $"\"{value.Value.Replace("\"", "\"\"")}\"";
			}

			return value.Value;
		}

		public override string ToString()
		{
			return _stringBuilder.ToString().TrimStart(' ');
		}
	}
}