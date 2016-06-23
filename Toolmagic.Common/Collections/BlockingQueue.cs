using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolmagic.Common.Collections
{
	public class BlockingQueue<T> : IBlockingQueue<T>
	{
		private readonly object _lockObject = new object();
		private readonly Queue<T> _queue = new Queue<T>();
		private int _inProgressCount;

		public BlockingQueue(IEnumerable<T> initialItems)
		{
			InitializeInitialItems(initialItems);
		}

		protected BlockingQueue()
		{
		}

		public bool IsCompleted => _inProgressCount == 0 && _queue.Count == 0;

		public bool TryEnqueue(T item)
		{
			lock (_lockObject)
			{
				return TryEnqueueInternal(item);
			}
		}

		public bool TryDequeue(out T item)
		{
			lock (_lockObject)
			{
				return TryDequeueInternal(out item);
			}
		}

		public void ReleaseItem(T item)
		{
			lock (_lockObject)
			{
				ReleaseItemInternal(item);
			}
		}

		protected void InitializeInitialItems(IEnumerable<T> initialItems)
		{
			// ReSharper disable once PossibleMultipleEnumeration
			Argument.IsNotNull(initialItems, nameof(initialItems));

			// ReSharper disable once PossibleMultipleEnumeration
			var initialItemsList = initialItems.ToList();

			// ReSharper disable once PossibleMultipleEnumeration
			if (!initialItems.Any())
			{
				throw new ArgumentException("Initial items collection cannot be empty.", nameof(initialItems));
			}

			initialItemsList.ForEach(item => { TryEnqueue(item); });
		}

		protected virtual void ReleaseItemInternal(T item)
		{
			_inProgressCount--;
		}

		protected virtual bool TryDequeueInternal(out T item)
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

		protected virtual bool TryEnqueueInternal(T item)
		{
			_queue.Enqueue(item);
			return true;
		}

		protected int GetQueueCountInternal()
		{
			return _queue.Count;
		}
	}
}