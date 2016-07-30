using System;
using System.Collections.Generic;

namespace Toolmagic.Common.Threading
{
	public sealed class PriorityReaderWriterLock : IDisposable
	{
		private static readonly IDictionary<ReadWritePriority, Func<IReaderWriterLockingStrategy>> Strategies =
			new Dictionary<ReadWritePriority, Func<IReaderWriterLockingStrategy>>
			{
				{ReadWritePriority.Default, () => new DefaultReadWriteLockingStrategy()},
				{ReadWritePriority.ReadFirst, () => new ReadFirstLockingStrategy()}
			};

		private readonly Disposable<IReaderWriterLockingStrategy> _lockingStrategy;

		public PriorityReaderWriterLock(ReadWritePriority priority)
		{
			_lockingStrategy = Disposable.Wrap(Strategies[priority].Invoke());
		}

		public ReadWritePriority Priority => _lockingStrategy.Value.Priority;

		public void Dispose()
		{
			_lockingStrategy.Dispose();
		}

		public void EnterReadLock()
		{
			_lockingStrategy.Value.EnterReadLock();
		}

		public void ExitReadLock()
		{
			_lockingStrategy.Value.ExitReadLock();
		}

		public void EnterWriteLock()
		{
			_lockingStrategy.Value.EnterWriteLock();
		}

		public void ExitWriteLock()
		{
			_lockingStrategy.Value.ExitWriteLock();
		}
	}
}