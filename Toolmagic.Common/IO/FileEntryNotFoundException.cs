using System;
using System.IO;
using System.Runtime.Serialization;

namespace Toolmagic.FileIndexer.Core
{
	[Serializable]
	public sealed class FileEntryNotFoundException : IOException
	{
		public FileEntryNotFoundException()
		{
		}

		public FileEntryNotFoundException(string message)
			: base(message)
		{
		}

		public FileEntryNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public FileEntryNotFoundException(string message, int hresult)
			: base(message, hresult)
		{
		}

		private FileEntryNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}