namespace Toolmagic.Common.Threading
{
	internal interface IReaderWriterLockingStrategy
	{
		ReadWritePriority Priority { get; }
		void EnterReadLock();
		void ExitReadLock();
		void EnterUpgradeableReadLock();
		void ExitUpgradeableReadLock();
		void EnterWriteLock();
		void ExitWriteLock();
	}
}