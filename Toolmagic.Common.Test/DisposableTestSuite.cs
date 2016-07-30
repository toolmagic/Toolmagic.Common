using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace Toolmagic.Common.Test
{
	[TestFixture]
	public sealed class DisposableTestSuite
	{
		public void DisposableCollectionCastsFromGenericCollectionTest()
		{
			IEnumerable<MemoryStream> streamCollection = new Collection<MemoryStream>();

			IEnumerable<IDisposable> disposableCollection = streamCollection;
			Assert.NotNull(disposableCollection);
		}

		[Test]
		public void DisposableAcceptsNotNullValueTest()
		{
			var disposable = MockRepository.GenerateStrictMock<IDisposable>();
			disposable.Expect(a => a.Dispose());

			using (Disposable.Wrap(NotNull.Wrap(disposable)))
			{
			}
		}

		[Test]
		public void DisposableDisposesDisposableCollectionTest()
		{
			ICollection<IDisposable> items = new Collection<IDisposable>();

			for (var i = 0; i < 10; i++)
			{
				var disposable = MockRepository.GenerateStrictMock<IDisposable>();
				disposable.Expect(a => a.Dispose());

				items.Add(disposable);
			}

			using (Disposable.Wrap(items))
			{
			}
		}

		[Test]
		public void DisposableDisposesDisposableTest()
		{
			var disposable = MockRepository.GenerateStrictMock<IDisposable>();
			disposable.Expect(a => a.Dispose());

			using (Disposable.Wrap(disposable))
			{
			}
		}


		[Test]
		public void DisposableFailsOnNullValueTest()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				using (Disposable.Wrap((object) null))
				{
				}
			});
		}

		[Test]
		public void DisposableSkipsNotDisposableTest()
		{
			var notDisposable = MockRepository.GenerateStrictMock<IEnumerable>();

			using (Disposable.Wrap(notDisposable))
			{
			}
		}

		[Test]
		public void DisposableWrapsUnnamedValueTest()
		{
			using (var stream = Disposable.Wrap(new MemoryStream()))
			{
				MemoryStream memoryStream = stream;
				memoryStream.Seek(0, SeekOrigin.Begin);
			}
		}
	}
}