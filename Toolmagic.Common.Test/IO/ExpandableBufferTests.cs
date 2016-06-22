using System;
using System.Reflection;
using NUnit.Framework;
using Toolmagic.Common.IO;

namespace Toolmagic.Common.Test.IO
{
	[TestFixture]
	public sealed class ExpandableBufferTests
	{
		private byte[] GetInternalBuffer(ExpandableBuffer expandableBuffer)
		{
			var bufferFieldInfo = typeof(ExpandableBuffer).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.IsNotNull(bufferFieldInfo);

			return (byte[]) bufferFieldInfo.GetValue(expandableBuffer);
		}

		[Test]
		public void AddRangeIncreasesCapacityToTwoTimesWithFillFactor05Test()
		{
			// Arrange
			var buffer = new ExpandableBuffer(0.5);
			var tempBuffer = new byte[10];

			// Act
			buffer.AddRange(tempBuffer, tempBuffer.Length);

			// Assert
			var internalBuffer = GetInternalBuffer(buffer);
			Assert.AreEqual(20, internalBuffer.Length);

			/* Attempt  Length   InternalBufferLength
			 *  before     0            0
			 *    0.      10           20     <--- increases with fillfactor = 0.5
			*/
		}

		[Test]
		public void CreateExpandableBufferFailsWithNegativeFillFactorTest()
		{
			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					// ReSharper disable once ObjectCreationAsStatement
					() => { new ExpandableBuffer(-0.9); }
				);
			Assert.AreEqual(@"fillFactor", exception.ParamName);
		}

		[Test]
		public void CreateExpandableBufferFailsWithTooLargeFillFactorTest()
		{
			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					// ReSharper disable once ObjectCreationAsStatement
					() => { new ExpandableBuffer(1.00001); }
				);
			Assert.AreEqual(@"fillFactor", exception.ParamName);
		}

		[Test]
		public void CreateExpandableBufferFailsWithZeroFillFactorTest()
		{
			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					// ReSharper disable once ObjectCreationAsStatement
					() => { new ExpandableBuffer(0); }
				);
			Assert.AreEqual(@"fillFactor", exception.ParamName);
		}

		[Test]
		public void CreateExpandableBufferTest()
		{
			Assert.IsNotNull(new ExpandableBuffer());
		}

		[Test]
		public void CreateExpandableBufferWith100PercentsFillFactorTest()
		{
			Assert.IsNotNull(new ExpandableBuffer(1));
		}

		[Test]
		public void CreateExpandableBufferWithFillFactorTest()
		{
			Assert.IsNotNull(new ExpandableBuffer(0.8));
		}

		[Test]
		public void ExpandableBufferCleansTest()
		{
			var buffer = new ExpandableBuffer();
			var tempBuffer = new byte[] {1, 2, 3, 4, 5, 6, 7};

			buffer.AddRange(tempBuffer, tempBuffer.Length);
			Assert.AreEqual(tempBuffer.Length, buffer.Length);

			buffer.Clear();
			Assert.AreEqual(0, buffer.Length);
		}

		[Test]
		public void ExpandableBufferFailsToAddRangeWithLengthGreaterThanBufferLengthTest()
		{
			var buffer = new ExpandableBuffer();
			var tempBuffer = new byte[] {1, 2, 3, 4, 5, 6, 7};

			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					() =>
					{
						var tooLargeCount = tempBuffer.Length + 1;
						buffer.AddRange(tempBuffer, tooLargeCount);
					}
				);
			Assert.AreEqual(@"count", exception.ParamName);
		}

		[Test]
		public void ExpandableBufferFailsToAddRangeWithNegativeLengthTest()
		{
			var buffer = new ExpandableBuffer();
			var tempBuffer = new byte[] {1, 2, 3, 4, 5, 6, 7};

			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					() =>
					{
						const int incorrectNegativeCount = -1;
						buffer.AddRange(tempBuffer, incorrectNegativeCount);
					}
				);
			Assert.AreEqual(@"count", exception.ParamName);
		}

		[Test]
		public void ExpandableBufferFailsToAddRangeWithNullBufferTest()
		{
			var buffer = new ExpandableBuffer();

			var exception = Assert.Throws<ArgumentNullException>
				(
					() =>
					{
						const byte[] nullBuffer = null;
						buffer.AddRange(nullBuffer, 100);
					}
				);
			Assert.AreEqual(@"buffer", exception.ParamName);
		}

		[Test]
		public void ExpandableBufferFailsToAddRangeWithZeroLengthTest()
		{
			var buffer = new ExpandableBuffer();
			var tempBuffer = new byte[] {1, 2, 3, 4, 5, 6, 7};

			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					() =>
					{
						const int incorrectZeroCount = 0;
						buffer.AddRange(tempBuffer, incorrectZeroCount);
					}
				);
			Assert.AreEqual(@"count", exception.ParamName);
		}

		[Test]
		public void GetRangeFailsForEmptyBufferTest()
		{
			var emptyBuffer = new ExpandableBuffer();

			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					() =>
					{
						const int incorrectZeroPosition = 0;
						var tempBuffer = new byte[100];
						emptyBuffer.GetRange(incorrectZeroPosition, tempBuffer, 0, tempBuffer.Length);
					}
				);
			Assert.AreEqual(@"position", exception.ParamName);
		}

		[Test]
		public void GetRangeFailsWithIncorrectCountArgumentTest()
		{
			var buffer = new ExpandableBuffer();
			buffer.AddRange(new byte[10], 10);

			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					() =>
					{
						var tempBuffer = new byte[100];
						const int incorrectCount = -1;
						buffer.GetRange(0, tempBuffer, 0, incorrectCount);
					}
				);
			Assert.AreEqual(@"count", exception.ParamName);
		}

		[Test]
		public void GetRangeFailsWithIncorrectOffsetArgumentTest()
		{
			var buffer = new ExpandableBuffer();
			buffer.AddRange(new byte[10], 10);

			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					() =>
					{
						const int incorrectOffset = -1;
						var tempBuffer = new byte[100];
						buffer.GetRange(0, tempBuffer, incorrectOffset, tempBuffer.Length);
					}
				);
			Assert.AreEqual(@"offset", exception.ParamName);
		}

		[Test]
		public void GetRangeFailsWithIncorrectPositionArgumentTest()
		{
			var buffer = new ExpandableBuffer();

			var exception = Assert.Throws<ArgumentOutOfRangeException>
				(
					() =>
					{
						const int incorrectPosition = -1;
						var tempBuffer = new byte[100];
						buffer.GetRange(incorrectPosition, tempBuffer, 0, tempBuffer.Length);
					}
				);
			Assert.AreEqual(@"position", exception.ParamName);
		}

		[Test]
		public void GetRangeFailsWithNullBufferArgumentTest()
		{
			var buffer = new ExpandableBuffer();
			buffer.AddRange(new byte[10], 10);

			var exception = Assert.Throws<ArgumentNullException>
				(
					() =>
					{
						const byte[] incorrectTempBuffer = null;
						buffer.GetRange(0, incorrectTempBuffer, 0, 100);
					}
				);
			Assert.AreEqual(@"buffer", exception.ParamName);
		}

		[Test]
		public void RepeatedAddRangeIncreasesCapacityToTwoTimesWithFillFactor05Test()
		{
			// Arrange
			var buffer = new ExpandableBuffer(0.5);
			var tempBuffer = new byte[10];

			// Act
			const int repeatCount = 5;
			for (var i = 0; i < repeatCount; i++)
			{
				buffer.AddRange(tempBuffer, tempBuffer.Length);
			}

			// Assert
			var internalBuffer = GetInternalBuffer(buffer);
			Assert.AreEqual(60, internalBuffer.Length);

			/* Attempt  Length   InternalBufferLength
			 *  before     0            0
			 *    0.      10           20     <--- increases with fillfactor = 0.5
			 *    1.      20           20
			 *    2.      30           60     <--- increases with fillfactor = 0.5
			 *    3.      40           60
			 *    4.      50           60
			*/
		}

		[Test]
		public void SuccessReadAndWriteBuffersTest()
		{
			// Arrange
			var buffer = new ExpandableBuffer();
			var tempBuffer = new byte[] {1, 2, 3, 4, 5, 6, 7};
			const int repeatCount = 5;

			// Act
			for (var i = 0; i < repeatCount; i++)
			{
				buffer.AddRange(tempBuffer, tempBuffer.Length);
			}

			var outputBuffer = new byte[tempBuffer.Length*repeatCount];
			var readBytes = buffer.GetRange(0, outputBuffer, 0, outputBuffer.Length);

			// Assert
			Assert.AreEqual(readBytes, outputBuffer.Length);
			for (var i = 0; i < outputBuffer.Length; i++)
			{
				Assert.AreEqual(tempBuffer[i%tempBuffer.Length], outputBuffer[i]);
			}
		}

		[Test]
		public void SuccessReadAndWriteBufferTest()
		{
			// Arrange
			var buffer = new ExpandableBuffer();
			var tempBuffer = new byte[] {1, 2, 3, 4, 5, 6, 7};

			// Act
			buffer.AddRange(tempBuffer, tempBuffer.Length);

			var outputBuffer = new byte[tempBuffer.Length];
			var readBytes = buffer.GetRange(0, outputBuffer, 0, outputBuffer.Length);

			// Assert
			Assert.AreEqual(readBytes, outputBuffer.Length);
			for (var i = 0; i < outputBuffer.Length; i++)
			{
				Assert.AreEqual(tempBuffer[i], outputBuffer[i]);
			}
		}

		[Test]
		public void ZeroLengthOfNewBufferTest()
		{
			var buffer = new ExpandableBuffer();

			Assert.AreEqual(0, buffer.Length);
		}
	}
}