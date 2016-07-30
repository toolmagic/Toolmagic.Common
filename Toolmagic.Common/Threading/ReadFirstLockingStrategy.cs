using System;
using System.Collections.Generic;
using System.Threading;

namespace Toolmagic.Common.Threading
{
	public sealed class ReadFirstLockingStrategy : IReaderWriterLockingStrategy, IDisposable
	{
		private readonly object _lockObject = new object();
		private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
		private int _readerCount;
		private readonly Queue<Guid> _writersQueue = new Queue<Guid>();

		public void Dispose()
		{
			_rwLock.Dispose();
		}

		public ReadWritePriority Priority => ReadWritePriority.ReadFirst;

		public void EnterReadLock()
		{
			lock (_lockObject)
			{
				_readerCount++;
			}

			_rwLock.EnterReadLock();
		}

		public void ExitReadLock()
		{
			_rwLock.ExitReadLock();

			lock (_lockObject)
			{
				_readerCount--;
			}
		}

		public void EnterWriteLock()
		{
			var writerId = Guid.NewGuid();

			lock (_lockObject)
			{
				_writersQueue.Enqueue(writerId);
			}

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

			_rwLock.EnterWriteLock();
		}

		public void ExitWriteLock()
		{
			_rwLock.ExitWriteLock();

			lock (_lockObject)
			{
				_writersQueue.Dequeue();
			}
		}
	}
}