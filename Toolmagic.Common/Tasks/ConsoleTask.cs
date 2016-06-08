using System;
using System.Threading;
using System.Threading.Tasks;
using Toolmagic.Common.IO;

namespace Toolmagic.Common.Tasks
{
	public static class ConsoleTask
	{
		public static async Task StartNew
			(
			Action<CancellationToken> executeAction,
			ConsoleKeyInfo cancelKey
			)
		{
			await StartNew(executeAction, cancelKey, new SystemConsole());
		}

		internal static async Task StartNew
			(
			Action<CancellationToken> executeAction,
			ConsoleKeyInfo cancelKey,
			IConsole console
			)
		{
			Argument.IsNotNull(executeAction, @"executeAction");
			Argument.IsNotNull(console, @"console");

			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

#pragma warning disable 4014
			Task.Factory.StartNew
				(
					() => { WaitForCancelKey(cancelKey, console, cancellationTokenSource); }
					, cancellationToken
				);
#pragma warning restore 4014

			await Task.Run
				(
					() => executeAction.Invoke(cancellationToken),
					cancellationToken
				);
		}

		private static void WaitForCancelKey
			(
			ConsoleKeyInfo cancelKey,
			IConsole console,
			CancellationTokenSource cancellationTokenSource
			)
		{
			try
			{
				while (!cancellationTokenSource.IsCancellationRequested)
				{
					if (console.KeyAvailable)
					{
						var key = console.ReadKey(false);

						if (key == cancelKey)
						{
							cancellationTokenSource.Cancel();
							break;
						}
					}

					Task
						.Delay(100, cancellationTokenSource.Token)
						.Wait();
				}
			}
			catch (OperationCanceledException)
			{
				// Skip exception
			}
		}
	}
}