namespace Toolmagic.Common.Tasks
{
	public interface IHierarchicalQueue<T>
	{
		bool TryEnqueue(T parentItem, T item);
		bool TryDequeue(out T item);
	}
}