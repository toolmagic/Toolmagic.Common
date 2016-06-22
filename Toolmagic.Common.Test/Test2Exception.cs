using System;
using System.Runtime.Serialization;

namespace Toolmagic.Common.Test
{
	[Serializable]
	public sealed class Test2Exception : Exception
	{
		public Test2Exception()
		{
		}

		public Test2Exception(string message) : base(message)
		{
		}

		public Test2Exception(string message, Exception innerException) : base(message, innerException)
		{
		}

#pragma warning disable CS0628 // New protected member declared in sealed class
		protected Test2Exception(SerializationInfo info, StreamingContext context) : base(info, context)
#pragma warning restore CS0628 // New protected member declared in sealed class
		{
		}
	}
}