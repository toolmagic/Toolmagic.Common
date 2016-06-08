using System;

namespace Toolmagic.Common.Tasks
{
	public sealed class SearchTaskOptions
	{
		private int _dequeueWaitTimeout = 100;
		private int _maxDegreeOfParallelism = Environment.ProcessorCount*2;
		private int _maxHierarchyDepth = 5;
		private int _maxQueueLength = 1000;

		public int MaxQueueLength
		{
			get { return _maxQueueLength; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(MaxQueueLength));
				}
				_maxQueueLength = value;
			}
		}

		public int MaxHierarchyDepth
		{
			get { return _maxHierarchyDepth; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(MaxHierarchyDepth));
				}
				_maxHierarchyDepth = value;
			}
		}

		public int MaxDegreeOfParallelism
		{
			get { return _maxDegreeOfParallelism; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(MaxDegreeOfParallelism));
				}
				_maxDegreeOfParallelism = value;
			}
		}

		public int DequeueWaitTimeout
		{
			get { return _dequeueWaitTimeout; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(DequeueWaitTimeout));
				}
				_dequeueWaitTimeout = value;
			}
		}
	}
}