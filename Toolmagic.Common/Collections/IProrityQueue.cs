namespace Toolmagic.Common.Collections
{
	public interface IProrityQueue<T>
	{
		void Enqueue(T item, int priority);
		bool Dequeue(out T item);
	}
}