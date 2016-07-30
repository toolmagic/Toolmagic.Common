using System.Collections.Generic;

namespace Toolmagic.Common.Collections
{
	public sealed class ConcurrentPriorityQueue<T> : IProrityQueue<T>
	{
		private readonly SortedSet<PriorityQueueItem<T>> _items =
			new SortedSet<PriorityQueueItem<T>>(new PriorityQueueItemComparer<T>());

		private readonly object _lockObject = new object();

		public bool Dequeue(out T item)
		{
			lock (_lockObject)
			{
				if (_items.Count == 0)
				{
					item = default(T);
					return false;
				}

				var maxItem = _items.Max;
				_items.Remove(maxItem);

				item = maxItem.Value;
				return true;
			}
		}

		public void Enqueue(T item, int priority)
		{
			lock (_items)
			{
				_items.Add(new PriorityQueueItem<T> {Value = item, Priority = priority});
			}
		}

		private struct PriorityQueueItem<TValue>
		{
			public TValue Value;
			public int Priority;
		}

		private sealed class PriorityQueueItemComparer<TValue> : IComparer<PriorityQueueItem<TValue>>
		{
			public int Compare(PriorityQueueItem<TValue> x, PriorityQueueItem<TValue> y)
			{
				if (x.Priority > y.Priority)
				{
					return 1;
				}

				if (x.Priority == y.Priority)
				{
					return x.Value.GetHashCode() - y.Value.GetHashCode();
				}

				return -1;
			}
		}
	}
}