namespace Toolmagic.Common.Tasks
{
	public interface IHierarchicalQueue<T>
	{
		bool IsCompleted { get; }
		bool TryEnqueue(T parentItem, T item);
		bool TryDequeue(out T item);
		void CompleteItem(T item);
	}
}