using System.Collections.Generic;
using System.Linq;

namespace Toolmagic.Common.Tasks
{
	internal sealed class LimitedHierarchicalQueue<T> : IHierarchicalQueue<T>
	{
		private readonly Dictionary<T, int> _depths = new Dictionary<T, int>();
		private readonly object _lockObject = new object();
		private readonly int _maxHierarchyDepth;
		private readonly int _maxQueueCount;
		private readonly Queue<T> _queue = new Queue<T>();
		private int _totalQueueCount;

		public LimitedHierarchicalQueue(T[] initialItems, int maxHierarchyDepth, int maxQueueLength)
		{
			_maxHierarchyDepth = maxHierarchyDepth;
			_maxQueueCount = maxQueueLength;

			initialItems
				.ToList()
				.ForEach(item => { TryEnqueue(default(T), item); });

			_totalQueueCount = initialItems.Length;
		}

		public bool TryEnqueue(T parentItem, T item)
		{
			lock (_lockObject)
			{
				if (_totalQueueCount >= _maxQueueCount)
				{
					return false;
				}

				var parentDepth = 0;

				if (typeof (T).IsValueType)
				{
					_depths.TryGetValue(parentItem, out parentDepth);
				}
				else
				{
					if (parentItem != null)
					{
						parentDepth = _depths[parentItem];
					}
				}

				if (parentDepth == _maxHierarchyDepth)
				{
					return false;
				}

				if (_depths.ContainsKey(item))
				{
					return false;
				}

				_depths.Add(item, parentDepth + 1);
				_queue.Enqueue(item);
				_totalQueueCount++;

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
					return true;
				}
				item = default(T);
				return false;
			}
		}
	}
}