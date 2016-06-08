using System;

namespace Toolmagic.Common.IO
{
	public interface IConsole
	{
		bool KeyAvailable { get; }

		ConsoleKeyInfo ReadKey(bool intercept);

		void Write(string format, params object[] args);
		void WriteLine(string format, params object[] args);
		void WriteLine();
	}
}