using System;
using System.Collections.Generic;
using System.Threading;

namespace Toolmagic.Common.Threading
{
	public sealed class ReadFirstLockingStrategy : IReaderWriterLockingStrategy, IDisposable
	{
		private readonly object _lockObject = new object();
		private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
		private readonly Queue<long> _writersQueue = new Queue<long>();
		private bool _inUpgradeableReadLock;
		private int _readerCount;
		private long _writerIdGenerator = long.MinValue;

		public void Dispose()
		{
			_rwLock.Dispose();
		}

		public ReadWritePriority Priority => ReadWritePriority.ReadFirst;

		public void EnterReadLock()
		{
			RegisterReader();

			_rwLock.EnterReadLock();
		}

		public void ExitReadLock()
		{
			UnregisterReader();

			_rwLock.ExitReadLock();
		}

		public void EnterUpgradeableReadLock()
		{
			RegisterReader();

			_rwLock.EnterUpgradeableReadLock();

			_inUpgradeableReadLock = true;
		}

		public void ExitUpgradeableReadLock()
		{
			UnregisterReader();

			_inUpgradeableReadLock = false;

			_rwLock.ExitUpgradeableReadLock();
		}

		public void EnterWriteLock()
		{
			if (!_inUpgradeableReadLock)
			{
				var writerId = RegisterWriter();
				WaitForWriter(writerId);
			}

			_rwLock.EnterWriteLock();
		}

		public void ExitWriteLock()
		{
			if (!_inUpgradeableReadLock)
			{
				UnregisterLastWriter();
			}

			_rwLock.ExitWriteLock();
		}

		private void RegisterReader()
		{
			lock (_lockObject)
			{
				_readerCount++;
			}
		}

		private void UnregisterReader()
		{
			lock (_lockObject)
			{
				_readerCount--;
			}
		}

		private void UnregisterLastWriter()
		{
			lock (_lockObject)
			{
				_writersQueue.Dequeue();
			}
		}

		private long RegisterWriter()
		{
			lock (_lockObject)
			{
				if (_writerIdGenerator == long.MaxValue)
				{
					_writerIdGenerator = long.MinValue;
				}

				var writerId = _writerIdGenerator++;
				_writersQueue.Enqueue(writerId);

				return writerId;
			}
		}

		private void WaitForWriter(long writerId)
		{
			while (true)
			{
				lock (_lockObject)
				{
					if (_readerCount == 0 && _writersQueue.Peek() == writerId)
					{
						break;
					}
				}
				Thread.Sleep(1);
			}
		}
	}
}