namespace Toolmagic.Common.Collections
{
	public interface IBlockingQueue<T>
	{
		bool IsCompleted { get; }
		bool TryEnqueue(T item);
		bool TryDequeue(out T item);
		void ReleaseItem(T item);
	}
}