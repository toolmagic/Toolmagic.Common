using System;

namespace Toolmagic.Common.IO
{
	internal sealed class ExpandableBuffer
	{
		private readonly double _fillFactor;
		private byte[] _buffer;

		public ExpandableBuffer(double fillFactor = 0.9)
		{
			Argument.IsInRange(fillFactor, "fillFactor", 0.1, 1);

			_fillFactor = fillFactor;
			_buffer = new byte[] {};
			Length = 0;
		}

		public int Length { get; private set; }

		public int GetRange(int position, byte[] buffer, int offset, int count)
		{
			Argument.IsInRange(position, @"position", 0, Length - 1);
			Argument.IsNotNull(buffer, @"buffer");
			Argument.IsInRange(offset, @"offset", 0, int.MaxValue);
			Argument.IsInRange(count, @"count", 1, int.MaxValue);

			var readBytes = Math.Min(Length, position + count) - position;
			Buffer.BlockCopy(_buffer, position, buffer, offset, readBytes);
			return readBytes;
		}

		public void AddRange(byte[] buffer, int count)
		{
			Argument.IsNotNull(buffer, @"buffer");
			Argument.IsInRange(count, @"count", 1, buffer.Length);

			ExpandBuffer(Length + count);
			Buffer.BlockCopy(buffer, 0, _buffer, Length, count);
			Length += count;
		}

		private void ExpandBuffer(int newLength)
		{
			if (newLength <= _buffer.Length)
			{
				return;
			}

			var newCapacity = Convert.ToInt32(Math.Round(newLength/_fillFactor, MidpointRounding.AwayFromZero));
			Array.Resize(ref _buffer, newCapacity);
		}

		public void Clear()
		{
			Array.Resize(ref _buffer, 0);
			Length = 0;
		}
	}
}