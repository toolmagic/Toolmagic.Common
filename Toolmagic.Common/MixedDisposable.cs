using System;

namespace Toolmagic.Common
{
	public abstract class MixedDisposable : IDisposable
	{
		private bool _disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~MixedDisposable()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				DisposeManagedResources();
			}

			DisposeUnmanagedResources();
			_disposed = true;
		}

		protected abstract void DisposeUnmanagedResources();

		protected virtual void DisposeManagedResources()
		{
		}
	}
}