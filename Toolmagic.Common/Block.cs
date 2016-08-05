using System;

namespace Toolmagic.Common
{
	public static class Block
	{
		public static FinallyBlock<T> Try<T>(Func<T> tryFunc)
		{
			var notNullTryFunc = Argument.IsNotNull(tryFunc, nameof(tryFunc));

			return new FinallyBlock<T>(notNullTryFunc);
		}

		public static FinallyBlock Try(Action tryAction)
		{
			var notNullTryAction = Argument.IsNotNull(tryAction, nameof(tryAction));

			return new FinallyBlock(notNullTryAction);
		}
	}
}