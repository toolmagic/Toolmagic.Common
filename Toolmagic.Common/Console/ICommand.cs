using System.Collections.Generic;

namespace Toolmagic.Common.Console
{
	public interface ICommand
	{
		IEnumerable<string> Switches { get; }

		string GetUsage();

		void Execute();
	}
}