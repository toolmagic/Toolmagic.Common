using System;

namespace Toolmagic.Common
{
	public sealed class FinallyBlock<T>
	{
		private readonly Func<T> _tryFunc;

		internal FinallyBlock(NotNull<Func<T>> tryFunc)
		{
			_tryFunc = tryFunc.Value;
		}

		public T Finally(Action finallyAction)
		{
			Argument.IsNotNull(finallyAction, nameof(finallyAction));

			T result;
			try
			{
				result = _tryFunc.Invoke();
			}
			catch (Exception tryBlockError)
			{
				try
				{
					finallyAction.Invoke();
				}
				catch (Exception finallyBlockError)
				{
					throw new AggregateException(tryBlockError.Message, tryBlockError, finallyBlockError);
				}
				throw;
			}
			finallyAction.Invoke();

			return result;
		}
	}
}