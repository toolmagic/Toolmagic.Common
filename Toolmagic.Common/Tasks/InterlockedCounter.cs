using System.Threading;

namespace Toolmagic.Common.Tasks
{
	internal sealed class InterlockedCounter
	{
		private long _value;

		public InterlockedCounter(long value)
		{
			_value = value;
		}

		public void Increment()
		{
			Interlocked.Increment(ref _value);
		}

		public void Decrement()
		{
			Interlocked.Decrement(ref _value);
		}

		public long GetValue()
		{
			return Interlocked.Read(ref _value);
		}

		public bool IsZero()
		{
			return Interlocked.CompareExchange(ref _value, 0, 0) == 0;
		}
	}
}