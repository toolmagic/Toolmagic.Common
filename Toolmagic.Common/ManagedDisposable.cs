using System;
using static System.FormattableString;

namespace Toolmagic.Common
{
	public abstract class ManagedDisposable : IDisposable
	{
		private bool _disposed;

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			DisposeManagedResources();
			_disposed = true;
		}

		protected abstract void DisposeManagedResources();

		protected void CheckNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(Invariant($"Object {GetType()} already disposed."));
			}
		}
	}
}