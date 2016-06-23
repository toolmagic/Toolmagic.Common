using System.Collections.Generic;
using System.Linq;

namespace Toolmagic.Common.Collections
{
	public sealed class HierarchyElement<T>
	{
		public HierarchyElement(T parent, T child)
		{
			Parent = parent;
			Child = child;
		}

		public T Parent { get; }
		public T Child { get; }
	}

	public sealed class HierarchicalLimitedBlockingQueue<T> : LimitedBlockingQueue<HierarchyElement<T>>
	{
		private readonly IDictionary<T, int> _itemsDepth = new Dictionary<T, int>();
		private readonly int _maxHierarchyDepth;

		public HierarchicalLimitedBlockingQueue(IEnumerable<T> initialItems, int maxItemCount, int maxHierarchyDepth)
		{
			Argument.IsInRange(maxHierarchyDepth, nameof(maxHierarchyDepth), 1, int.MaxValue);

			_maxHierarchyDepth = maxHierarchyDepth;

			var initialHierarchyElements = initialItems.Select(item => new HierarchyElement<T>(default(T), item));
			InitializeLimitedInitialItems(initialHierarchyElements, maxItemCount);
		}

		protected override bool TryEnqueueInternal(HierarchyElement<T> item)
		{
			if (_itemsDepth.ContainsKey(item.Child))
			{
				return false;
			}

			var parentDepth = 0;

			if (typeof(T).IsValueType)
			{
				_itemsDepth.TryGetValue(item.Parent, out parentDepth);
			}
			else
			{
				if (item.Parent != null)
				{
					parentDepth = _itemsDepth[item.Parent];
				}
			}

			if (parentDepth == _maxHierarchyDepth)
			{
				return false;
			}

			if (base.TryEnqueueInternal(item))
			{
				_itemsDepth.Add(item.Child, parentDepth + 1);
				return true;
			}

			return false;
		}
	}
}