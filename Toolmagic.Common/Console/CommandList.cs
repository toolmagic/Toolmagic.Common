using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Toolmagic.Common.Console
{
	internal sealed class CommandList
	{
		private readonly IDictionary<string, ICommand> _commandMap = new Dictionary<string, ICommand>();

		private CommandList(IEnumerable<ICommand> commands)
		{
			Commands = commands;

			foreach (var command in Commands)
			{
				foreach (var c in command.Switches)
				{
					_commandMap.Add(c, command);
				}
			}
		}

		public IEnumerable<ICommand> Commands { get; }

		public bool TryGetCommand(string command, out ICommand commandInstance)
		{
			Argument.IsNotEmpty(command, nameof(command));

			return _commandMap.TryGetValue(command, out commandInstance);
		}

		public static CommandList Create()
		{
			var commands =
				Assembly
					.GetEntryAssembly()
					.GetTypes()
					.Where(t => t.IsClass && !t.IsAbstract && t.GetInterface(typeof(ICommand).FullName) != null)
					.Select(t => (ICommand) Activator.CreateInstance(t))
				;

			return new CommandList(commands);
		}
	}
}