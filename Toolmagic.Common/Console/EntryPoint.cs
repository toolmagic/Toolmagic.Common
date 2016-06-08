using System;
using System.Linq;
using System.Text;
using CommandLine;
using Common.Logging;

namespace Toolmagic.Common.Console
{
	public sealed class EntryPoint
	{
		private static readonly CommandList Commands = CommandList.Create();
		private static readonly ILog Log = LogManager.GetLogger<EntryPoint>();

		public static Action<string> InitializeTrace { get; set; }

		public static int Execute(string[] args)
		{
			try
			{
				Log.Info(AppInfo.ProductNameAndVersion);
				Log.InfoFormat(
					"Command-line arguments:\n  {0}",
					args.Length == 0
						? "(none)"
						: string.Join("\n  ", args.Select((a, i) => $"args[{i}]: {a}")));

				if (args.Length == 0 || args[0] == "--help" || args[0] == "/?")
				{
					LogUsage();
					return ReturnCode.Ok;
				}

				var parsedArguments = 0;

				if (args[0] == "--trace")
				{
					if (args.Length < 2)
					{
						throw new IncorrectUsageException(@"The '--trace' option must be followed by uri.");
					}

					InitializeTrace?.Invoke(args[1]);

					parsedArguments += 2;
				}

				if (parsedArguments == args.Length)
				{
					throw new IncorrectUsageException(@"Command specifier is missed.");
				}

				var commandSwitch = args[parsedArguments];

				ICommand command;
				if (!Commands.TryGetCommand(commandSwitch, out command))
				{
					throw new IncorrectUsageException($@"Unknown command: {commandSwitch}.");
				}

				parsedArguments++; /* skip command switch */

				if (!Parser.Default.ParseArguments(args.Skip(parsedArguments).ToArray(), command))
				{
					throw new IncorrectUsageException("Can't parse command-line arguments");
				}

				command.Execute();
				return 0;
			}
			catch (IncorrectUsageException e)
			{
				Log.ErrorFormat("ERROR: {0}", e.Message);
				LogUsage();

				return ReturnCode.IncorrectUsage;
			}
			catch (Exception e)
			{
				Log.Error(e);

				return ReturnCode.Unknown;
			}
		}

		private static void LogUsage()
		{
			Log.Info(@"Usage:");
			foreach (var command in Commands.Commands)
			{
				var builder = new StringBuilder();

				builder
					.Append(@"> ")
					.Append(AppInfo.ProductName)
					.Append(@".exe ");

				if (command.Switches.Count() == 1)
				{
					builder.Append(command.Switches.Single());
				}
				else
				{
					builder
						.Append('(')
						.Append(string.Join(" | ", command.Switches))
						.Append(')');
				}

				builder.AppendLine().Append(command.GetUsage());

				Log.Info(builder.ToString());
			}
		}
	}
}