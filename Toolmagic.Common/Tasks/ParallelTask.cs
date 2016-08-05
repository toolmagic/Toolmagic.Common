using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Toolmagic.Common.Tasks
{
	public static class ParallelTask
	{
		public static async Task StartNew(IEnumerable<Action> actions)
		{
			var notNullActions = Argument.IsNotNull(actions, nameof(actions));
			Argument.IsInRange(notNullActions.Value.Count(), nameof(actions), 2, int.MaxValue);

			await StartCore(notNullActions);
		}

		public static async Task StartNew(Action action, int parallelCount)
		{
			Argument.IsNotNull(action, nameof(action));
			Argument.IsInRange(parallelCount, nameof(parallelCount), 2, int.MaxValue);

			var actions = NotNull<IEnumerable<Action>>.Create(Enumerable.Repeat(action, parallelCount));

			await StartCore(actions);
		}

		private static async Task StartCore(NotNull<IEnumerable<Action>> actions)
		{
			await Task.Run(() => { ExecuteAction(actions.Value.ToArray()); });
		}

		private static void ExecuteAction(Action[] actions)
		{
			var threads = new List<Thread>(actions.Select(action => new Thread(InvokeAction)));

			for (var i = 0; i < actions.Length; i++)
			{
				threads[i].Start(actions[i]);
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}
		}

		private static void InvokeAction(object action)
		{
			((Action) action).Invoke();
		}
	}
}