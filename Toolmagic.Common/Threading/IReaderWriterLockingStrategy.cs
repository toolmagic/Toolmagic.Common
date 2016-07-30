namespace Toolmagic.Common.Threading
{
	internal interface IReaderWriterLockingStrategy
	{
		ReadWritePriority Priority { get; }
		void EnterReadLock();
		void ExitReadLock();
		void EnterWriteLock();
		void ExitWriteLock();
	}
}