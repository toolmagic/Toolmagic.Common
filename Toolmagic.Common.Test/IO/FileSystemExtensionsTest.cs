using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using Toolmagic.Common.IO;

namespace Toolmagic.Common.Test.IO
{
	[TestFixture]
	public sealed class FileSystemExtensionsTest
	{
		private static IEnumerable IsDirectoryEmptyTestData
		{
			get
			{
				yield return new TestCaseData(@"C:\temp", true, new[] {@"C:\temp\file1"}).Returns(false);
				yield return new TestCaseData(@"C:\temp", true, new string[] {}).Returns(true);
				yield return new TestCaseData(@"C:\blabla", false, new string[] {}).Returns(typeof(DirectoryNotFoundException));
				yield return new TestCaseData(string.Empty, false, new string[] {}).Returns(typeof(ArgumentException));
				yield return new TestCaseData(null, false, new string[] {}).Returns(typeof(ArgumentNullException));
			}
		}

		[TestCaseSource(nameof(IsDirectoryEmptyTestData))]
		public object IsDirectoryEmptyTest(string path, bool isDirectoryExists, IEnumerable<string> entries)
		{
			var fileSystem = MockRepository.GenerateStrictMock<IFileSystem>();
			fileSystem.Stub(x => x.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories)).Return(entries);
			fileSystem.Stub(x => x.DirectoryExists(path)).Return(isDirectoryExists);

			try
			{
				return fileSystem.IsDirectoryEmpty(path);
			}
			catch (Exception t)
			{
				return t.GetType();
			}
		}
	}
}