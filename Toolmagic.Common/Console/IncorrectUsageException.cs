using System;

namespace Toolmagic.Common.Console
{
	[Serializable]
	internal sealed class IncorrectUsageException : Exception
	{
		public IncorrectUsageException(string message)
			: base(message)
		{
		}
	}
}