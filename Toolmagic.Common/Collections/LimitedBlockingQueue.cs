using System.Collections.Generic;
using System.Linq;

namespace Toolmagic.Common.Collections
{
	public class LimitedBlockingQueue<T> : BlockingQueue<T>
	{
		private int _maxItemCount;
		private int _processedCount;

		public LimitedBlockingQueue(IEnumerable<T> initialItems, int maxItemCount)
		{
			InitializeLimitedInitialItems(initialItems, maxItemCount);
		}

		protected LimitedBlockingQueue()
		{
		}

		protected void InitializeLimitedInitialItems(IEnumerable<T> initialItems, int maxItemCount)
		{
			// ReSharper disable once PossibleMultipleEnumeration
			Argument.IsNotNull(initialItems, nameof(initialItems));
			Argument.IsInRange(maxItemCount, nameof(maxItemCount), 1, int.MaxValue);

			_maxItemCount = maxItemCount;

			// ReSharper disable once PossibleMultipleEnumeration
			InitializeInitialItems(initialItems.Take(maxItemCount));
		}

		protected override bool TryDequeueInternal(out T item)
		{
			if (_processedCount + GetQueueCountInternal() > _maxItemCount)
			{
				item = default(T);
				return false;
			}

			if (base.TryDequeueInternal(out item))
			{
				_processedCount++;
				return true;
			}
			return false;
		}

		protected override bool TryEnqueueInternal(T item)
		{
			if (_processedCount + GetQueueCountInternal() >= _maxItemCount)
			{
				return false;
			}
			return base.TryEnqueueInternal(item);
		}
	}
}