using System;

namespace Toolmagic.Common.IO
{
	public sealed class SystemConsole : IConsole
	{
		private readonly object _lockObject = new object();

		public bool KeyAvailable => System.Console.KeyAvailable;

		public ConsoleKeyInfo ReadKey(bool intercept)
		{
			return System.Console.ReadKey(intercept);
		}

		public void WriteLine(string format, params object[] args)
		{
			lock (_lockObject)
			{
				System.Console.WriteLine(format, args);
			}
		}

		public void Write(string format, params object[] args)
		{
			System.Console.Write(format, args);
		}

		public void WriteLine()
		{
			System.Console.WriteLine();
		}
	}
}