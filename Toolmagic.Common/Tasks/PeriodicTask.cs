using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toolmagic.Common.Tasks
{
	public static class PeriodicTask
	{
		public static Task StartNew(Action action, TimeSpan period, CancellationToken cancellationToken)
		{
			Argument.IsNotNull(action, @"action");
			Argument.IsInRange(period, @"period", TimeSpan.FromMilliseconds(1), TimeSpan.MaxValue);

			return Task.Run
				(
					async () => { await Execute(action, period, cancellationToken); },
					cancellationToken
				);
		}

		private static async Task Execute(Action action, TimeSpan period, CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				await Task
					.Delay(period, cancellationToken)
					.ContinueWith
					(
						task =>
						{
							try
							{
								action.Invoke();
							}
							catch (OperationCanceledException)
							{
								// do nothing
							}
						},
						cancellationToken
					);
			}
		}
	}
}