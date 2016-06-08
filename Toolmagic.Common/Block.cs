using System;

namespace Toolmagic.Common
{
	public static class Block
	{
		public static FinallyBlock<T> Try<T>(Func<T> tryFunc)
		{
			var notNullTryFunc = Argument.IsNotNull(tryFunc, "tryFunc");

			return new FinallyBlock<T>(notNullTryFunc);
		}

		public static FinallyBlock Try(Action tryAction)
		{
			var notNullTryAction = Argument.IsNotNull(tryAction, "tryAction");

			return new FinallyBlock(notNullTryAction);
		}
	}
}