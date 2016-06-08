using System;

namespace Toolmagic.Common.Console
{
	internal sealed class IncorrectUsageException : Exception
	{
		public IncorrectUsageException(string message)
			: base(message)
		{
		}
	}
}