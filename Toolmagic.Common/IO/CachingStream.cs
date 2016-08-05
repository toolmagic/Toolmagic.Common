using System;
using System.IO;

namespace Toolmagic.Common.IO
{
	public sealed class CachingStream : Stream
	{
		private const int BlockSize = 4096;
		private readonly object _lockObject = new object();
		private readonly Stream _stream;
		private ExpandableBuffer _expandableBuffer = new ExpandableBuffer();
		private bool _isEof;
		private long _position;

		public CachingStream(Stream stream)
		{
			Argument.IsNotNull(stream, nameof(stream));

			_stream = stream;
		}

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => false;

		public override long Length
		{
			get
			{
				lock (_lockObject)
				{
					EnsureClosed();

					if (!_isEof)
					{
						throw new NotSupportedException();
					}

					return _expandableBuffer.Length;
				}
			}
		}

		public override long Position
		{
			get
			{
				lock (_lockObject)
				{
					EnsureClosed();

					return _position;
				}
			}
			set
			{
				Argument.IsInRange(value, @"Position", 0, int.MaxValue);

				lock (_lockObject)
				{
					EnsureClosed();

					if (_expandableBuffer.Length < value)
					{
						ExpandBuffer(value);
					}

					SetPosition(value, @"Position");
				}
			}
		}

		private void SetPosition(long position, string argumentName = null)
		{
			Argument.IsInRange(position, argumentName, 0, _expandableBuffer.Length);

			_position = position;
		}

		public override void Close()
		{
			lock (_lockObject)
			{
				_stream.Close();
				if (_expandableBuffer != null)
				{
					_expandableBuffer.Clear();
					_expandableBuffer = null;
				}
			}

			base.Close();
		}

		public override void Flush()
		{
			// do nothing
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					return SeekBegin(offset);

				case SeekOrigin.Current:
					return SeekCurrent(offset);

				case SeekOrigin.End:
					return SeekEnd(offset);

				default:
					throw new NotSupportedException();
			}
		}

		private void EnsureClosed()
		{
			if (_expandableBuffer == null)
			{
				throw new ObjectDisposedException("Stream has already been closed");
			}
		}

		private long SeekBegin(long offset)
		{
			Argument.IsInRange(offset, @"offset", 0, int.MaxValue);

			lock (_lockObject)
			{
				EnsureClosed();

				if (_expandableBuffer.Length < offset)
				{
					ExpandBuffer(offset);
				}

				SetPosition(offset);
				return offset;
			}
		}

		private long SeekCurrent(long offset)
		{
			lock (_lockObject)
			{
				EnsureClosed();

				Argument.IsInRange(offset, @"offset", -_position, int.MaxValue);

				var newPosition = _position + offset;

				if (_expandableBuffer.Length < newPosition)
				{
					ExpandBuffer(newPosition);
				}

				SetPosition(newPosition);
				return newPosition;
			}
		}

		private long SeekEnd(long offset)
		{
			Argument.IsInRange(offset, @"offset", (long) int.MinValue - 1, 0);

			lock (_lockObject)
			{
				EnsureClosed();

				ExpandBuffer(int.MaxValue);

				if (!_isEof)
				{
					throw new NotSupportedException("Stream is too long");
				}

				var newPosition = _expandableBuffer.Length + offset;
				SetPosition(newPosition);
				return newPosition;
			}
		}

		private void ExpandBuffer(long length)
		{
			var bytesToRead = length - _expandableBuffer.Length;

			while (bytesToRead > 0 && !_isEof)
			{
				var tempBufferSize = Math.Min(bytesToRead, BlockSize);
				var tempBuffer = new byte[tempBufferSize];
				var tempReadBytes = _stream.Read(tempBuffer, 0, tempBuffer.Length);

				if (tempReadBytes > 0)
				{
					_expandableBuffer.AddRange(tempBuffer, tempReadBytes);
				}

				bytesToRead -= tempReadBytes;
				_isEof = tempReadBytes == 0;
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			lock (_lockObject)
			{
				EnsureClosed();

				var newPosition = _position + count;

				if (_expandableBuffer.Length < newPosition)
				{
					ExpandBuffer(newPosition);
				}

				if (_position == _expandableBuffer.Length)
				{
					return 0;
				}

				var bytesRead = _expandableBuffer.GetRange(Convert.ToInt32(_position), buffer, offset, count);
				SetPosition(_position + bytesRead);
				return bytesRead;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}
}