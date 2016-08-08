using System;
using System.Threading;

namespace Toolmagic.Common.Threading
{
	public sealed class DefaultReadWriteLockingStrategy : IReaderWriterLockingStrategy, IDisposable
	{
		private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

		public void Dispose()
		{
			_rwLock.Dispose();
		}

		public ReadWritePriority Priority => ReadWritePriority.Default;

		public void EnterReadLock()
		{
			_rwLock.EnterReadLock();
		}

		public void EnterUpgradeableReadLock()
		{
			_rwLock.EnterUpgradeableReadLock();
		}

		public void EnterWriteLock()
		{
			_rwLock.EnterWriteLock();
		}

		public void ExitReadLock()
		{
			_rwLock.ExitReadLock();
		}

		public void ExitUpgradeableReadLock()
		{
			_rwLock.ExitUpgradeableReadLock();
		}

		public void ExitWriteLock()
		{
			_rwLock.ExitWriteLock();
		}
	}
}