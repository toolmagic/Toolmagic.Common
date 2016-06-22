using System;
using System.Runtime.Serialization;

namespace Toolmagic.Common.Test
{
	[Serializable]
	public sealed class TestException : Exception
	{
		public TestException()
		{
		}

		public TestException(string message)
			: base(message)
		{
		}

		public TestException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

#pragma warning disable CS0628 // New protected member declared in sealed class
		protected TestException(SerializationInfo info, StreamingContext context)
#pragma warning restore CS0628 // New protected member declared in sealed class
			: base(info, context)
		{
		}
	}
}