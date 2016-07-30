using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Toolmagic.Common.IO;

namespace Toolmagic.Common.Test.IO
{
	[TestFixture]
	public sealed class CachingStreamTests
	{
		private const string ResourceName = @"IO.Resources.Crawler.NET.pdf";

		[Test]
		public void CachingStreamDoesNothingOnFlushingTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					cachingStream.Flush();
				}
			}
		}

		//[Test]
		//[Ignore("Implement this test later! It requires too large memory (2GB) or redesign to mock expandable buffer")]
		//public void CachingStreamFailsReadingOnTooLarge2GbUnderlyingStreamTest()
		//{
		//	Assert.Fail("Not implemented");
		//}

		[Test]
		public void CachingStreamFailsToGetLengthBeforeEofTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					Assert.Throws<NotSupportedException>
						(
							// ReSharper disable once UnusedVariable
							() =>
							{
								var length = cachingStream.Length;
							}
						);
				}
			}
		}

		[Test]
		public void CachingStreamFailsToSeekAfterEndTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					var exception = Assert.Throws<ArgumentOutOfRangeException>
						(
							() => { cachingStream.Seek(1, SeekOrigin.End); }
						);
					Assert.AreEqual(@"offset", exception.ParamName);
				}
			}
		}

		[Test]
		public void CachingStreamFailsToSeekBeforeBeginTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					var exception = Assert.Throws<ArgumentOutOfRangeException>
						(
							() => { cachingStream.Seek(-1, SeekOrigin.Begin); }
						);
					Assert.AreEqual(@"offset", exception.ParamName);
				}
			}
		}

		[Test]
		public void CachingStreamFailsToSeekCurrentBeforeBeginTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					cachingStream.Seek(100, SeekOrigin.Begin);

					var exception = Assert.Throws<ArgumentOutOfRangeException>
						(
							() => { cachingStream.Seek(-101, SeekOrigin.Current); }
						);
					Assert.AreEqual(@"offset", exception.ParamName);
				}
			}
		}

		[Test]
		public void CachingStreamFailsToSeekWithUnknownOriginTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					Assert.Throws<NotSupportedException>
						(
							() => { cachingStream.Seek(0, (SeekOrigin) 100); }
						);
				}
			}
		}

		[Test]
		public void CachingStreamFailsToSetLengthTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					Assert.Throws<NotSupportedException>
						(
							() => { cachingStream.SetLength(100); }
						);
				}
			}
		}

		[Test]
		public void CachingStreamFailsToSetNegativePositionTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					var exception = Assert.Throws<ArgumentOutOfRangeException>
						(
							() => { cachingStream.Position = -1; }
						);
					Assert.AreEqual(@"Position", exception.ParamName);
				}
			}
		}

		[Test]
		public void CachingStreamFailsToSetTooLargePositionTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					var exception = Assert.Throws<ArgumentOutOfRangeException>
						(
							() => { cachingStream.Position = int.MaxValue; }
						);
					Assert.AreEqual(@"Position", exception.ParamName);
				}
			}
		}

		[Test]
		public void CachingStreamFailsToWriteTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					byte[] buffer = {0, 1};

					Assert.Throws<NotSupportedException>
						(
							() => { cachingStream.Write(buffer, 0, buffer.Length); }
						);
				}
			}
		}

		[Test]
		public void CachingStreamFeaturesTest()
		{
			using (var stream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(stream))
				{
					Assert.IsNotNull(cachingStream);

					Assert.IsTrue(cachingStream.CanRead);
					Assert.IsTrue(cachingStream.CanSeek);
					Assert.IsFalse(cachingStream.CanWrite);
				}
			}
		}

		[Test]
		public void CachingStreamGetsLengthTest()
		{
			using (var stream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(stream))
				{
					cachingStream.Seek(0, SeekOrigin.End);

					Assert.AreEqual(stream.Length, cachingStream.Length);
				}
			}
		}

		[Test]
		public void CachingStreamReadsEntireBookTest()
		{
			const int bufferSize = 4096;

			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					using (var notcachingStream = TestHelpers.GetResourceStream(ResourceName))
					{
						while (true)
						{
							var bufferedTempBuffer = new byte[bufferSize];
							var notBufferedTempBuffer = new byte[bufferSize];

							var bytesReadFromcachingStream = cachingStream.Read(bufferedTempBuffer, 0, bufferedTempBuffer.Length);
							var bytesReadFromNiotcachingStream = notcachingStream.Read(notBufferedTempBuffer, 0,
								notBufferedTempBuffer.Length);

							Assert.AreEqual(bytesReadFromcachingStream, bytesReadFromNiotcachingStream);
							Assert.IsTrue(bufferedTempBuffer.SequenceEqual(notBufferedTempBuffer));

							if (bytesReadFromcachingStream == 0)
							{
								break;
							}
						}

						Assert.AreEqual(cachingStream.Length, notcachingStream.Length);
						Assert.AreEqual(cachingStream.Position, notcachingStream.Position);
					}
				}
			}
		}

		[Test]
		public void CachingStreamSeeksCurrentTest()
		{
			using (var stream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(stream))
				{
					var tempBuffer = new byte[100];
					var bytesRead = cachingStream.Read(tempBuffer, 0, tempBuffer.Length);
					Assert.AreEqual(tempBuffer.Length, bytesRead);

					var position = cachingStream.Position;
					Assert.AreEqual(position, cachingStream.Seek(0, SeekOrigin.Current));

					position = cachingStream.Position;
					Assert.AreEqual(position + 100, cachingStream.Seek(100, SeekOrigin.Current));

					position = cachingStream.Position;
					Assert.AreEqual(position - 55, cachingStream.Seek(-55, SeekOrigin.Current));
				}
			}
		}

		[Test]
		public void CachingStreamSeeksToBeginTest()
		{
			using (var stream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(stream))
				{
					cachingStream.Seek(0, SeekOrigin.Begin);
					Assert.AreEqual(0, cachingStream.Position);

					cachingStream.Seek(10, SeekOrigin.Begin);
					Assert.AreEqual(10, cachingStream.Position);
				}
			}
		}

		[Test]
		public void CachingStreamSeeksToEndTest()
		{
			using (var stream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(stream))
				{
					cachingStream.Seek(0, SeekOrigin.End);

					Assert.AreEqual(cachingStream.Length, cachingStream.Position);
				}
			}
		}

		[Test]
		public void CachingStreamSetsPositionTest()
		{
			using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(sourceStream))
				{
					const int position = 100;

					cachingStream.Position = position;
					Assert.AreEqual(position, cachingStream.Position);
				}
			}
		}

		[Test]
		public void ClosedCachingStreamFailsOnReadTest()
		{
			Assert.Throws<ObjectDisposedException>
				(
					() =>
					{
						using (var sourceStream = TestHelpers.GetResourceStream(ResourceName))
						{
							using (var cachingStream = new CachingStream(sourceStream))
							{
								cachingStream.Close();
								Assert.AreEqual(0, cachingStream.Position);
							}
						}
					}
				);
		}

		[Test]
		public void CreateCachingStreamFailsWithNullUnderlyingStreamTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					// ReSharper disable once ObjectCreationAsStatement
					() => { new CachingStream(null); }
				);
			Assert.AreEqual(@"stream", exception.ParamName);
		}

		[Test]
		public void CreateCachingStreamTest()
		{
			using (var stream = TestHelpers.GetResourceStream(ResourceName))
			{
				using (var cachingStream = new CachingStream(stream))
				{
					Assert.IsNotNull(cachingStream);
				}
			}
		}
	}
}