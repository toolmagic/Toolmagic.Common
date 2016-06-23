using System.Collections.Generic;
using System.Linq;

namespace Toolmagic.Common.Tasks
{
	internal sealed class LimitedHierarchicalQueue<T> : IHierarchicalQueue<T>
	{
		private readonly Dictionary<T, int> _itemDepths = new Dictionary<T, int>();
		private readonly object _lockObject = new object();
		private readonly int _maxHierarchyDepth;
		private readonly int _maxQueueCount;
		private readonly Queue<T> _queue = new Queue<T>();
		private int _inProgressCount;
		private int _processedCount;

		public LimitedHierarchicalQueue(T[] initialItems, int maxHierarchyDepth, int maxQueueLength)
		{
			_maxHierarchyDepth = maxHierarchyDepth;
			_maxQueueCount = maxQueueLength;

			initialItems
				.ToList()
				.ForEach(item => { TryEnqueue(default(T), item); });
		}

		public bool TryEnqueue(T parentItem, T item)
		{
			lock (_lockObject)
			{
				if (_processedCount == _maxQueueCount)
				{
					return false;
				}

				if (_itemDepths.ContainsKey(item))
				{
					return false;
				}

				var parentDepth = 0;

				if (typeof(T).IsValueType)
				{
					_itemDepths.TryGetValue(parentItem, out parentDepth);
				}
				else
				{
					if (parentItem != null)
					{
						parentDepth = _itemDepths[parentItem];
					}
				}

				if (parentDepth == _maxHierarchyDepth)
				{
					return false;
				}

				_itemDepths.Add(item, parentDepth + 1);
				_queue.Enqueue(item);

				return true;
			}
		}

		public bool TryDequeue(out T item)
		{
			lock (_lockObject)
			{
				if (_queue.Count > 0)
				{
					item = _queue.Dequeue();
					_inProgressCount++;
					return true;
				}
				item = default(T);
				return false;
			}
		}

		public void CompleteItem(T item)
		{
			lock (_lockObject)
			{
				_inProgressCount--;
				_processedCount++;
			}
		}

		public bool IsCompleted
		{
			get
			{
				lock (_lockObject)
				{
					return _inProgressCount == 0 && _queue.Count == 0;
				}
			}
		}
	}
}