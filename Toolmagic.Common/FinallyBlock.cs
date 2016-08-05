using System;

namespace Toolmagic.Common
{
	public sealed class FinallyBlock
	{
		private readonly Func<bool> _tryFunc;

		internal FinallyBlock(NotNull<Action> tryAction)
		{
			_tryFunc =
				() =>
				{
					tryAction.Value.Invoke();
					return true;
				};
		}

		public void Finally(Action finallyAction)
		{
			Argument.IsNotNull(finallyAction, nameof(finallyAction));

			Block
				.Try(_tryFunc)
				.Finally(finallyAction);
		}
	}
}