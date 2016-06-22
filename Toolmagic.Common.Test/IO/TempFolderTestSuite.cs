using System;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using Toolmagic.Common.IO;

namespace Toolmagic.Common.Test.IO
{
	[TestFixture]
	public sealed class TempFolderTestSuite
	{
		private const string AppKey = "MyApp";
		private const string FileExtension = @".png";
		private const string FileNamePrefix = @"image1";

		private static IFileSystem CreateFileSystemMock(string appKey, bool appFolderExists)
		{
			var fileSystem = MockRepository.GenerateStrictMock<IFileSystem>();

			var tempFolder = Path.GetTempPath();
			var appTempFolder = Path.Combine(tempFolder, appKey);

			fileSystem.Stub(f => f.GetTempPath()).Return(tempFolder);
			fileSystem.Stub(f => f.DirectoryExists(appTempFolder)).Return(appFolderExists);

			if (!appFolderExists)
			{
				fileSystem.Stub(f => f.CreateDirectory(appTempFolder));
			}

			return fileSystem;
		}

		[Test]
		public void TempFolderCleansNonexistentFolderTest()
		{
			var fileSystem = MockRepository.GenerateStrictMock<IFileSystem>();

			var tempFolder = Path.GetTempPath();
			var appTempFolder = Path.Combine(tempFolder, AppKey);

			fileSystem.Stub(f => f.GetTempPath()).Return(tempFolder);
			fileSystem.Stub(f => f.DirectoryExists(appTempFolder)).Return(false);

			TempFolder.CleanFolder(fileSystem, AppKey);
		}

		[Test]
		public void TempFolderCleanTest()
		{
			var fileSystem = MockRepository.GenerateStrictMock<IFileSystem>();

			var tempFolder = Path.GetTempPath();
			var appTempFolder = Path.Combine(tempFolder, AppKey);

			var tempFile1 = Path.Combine(appTempFolder, "file1.png");
			var tempFile2 = Path.Combine(appTempFolder, "file2.png");
			var subFolder = Path.Combine(appTempFolder, "folder1");
			var tempFile3 = Path.Combine(subFolder, "file3.png");

			string[] fileSystemEntries =
			{
				tempFile1,
				tempFile2,
				subFolder,
				tempFile3
			};

			fileSystem.Stub(f => f.GetTempPath()).Return(tempFolder);
			fileSystem.Stub(f => f.DirectoryExists(appTempFolder)).Return(true);

			fileSystem
				.Stub(f => f.EnumerateFileSystemEntries(appTempFolder, "*.*", SearchOption.AllDirectories))
				.Return(fileSystemEntries);

			fileSystem.Stub(f => f.FileExists(tempFile3)).Return(true);
			fileSystem.Stub(f => f.DeleteFile(tempFile3));

			fileSystem.Stub(f => f.FileExists(subFolder)).Return(false);
			fileSystem.Stub(f => f.DirectoryExists(subFolder)).Return(true);
			fileSystem.Stub(f => f.DeleteDirectory(subFolder));

			fileSystem.Stub(f => f.FileExists(tempFile2)).Return(true);
			fileSystem
				.Stub(f => f.DeleteFile(tempFile2))
				.Throw(new IOException());

			fileSystem.Stub(f => f.FileExists(tempFile1)).Return(true);
			fileSystem.Stub(f => f.DeleteFile(tempFile1));

			TempFolder.CleanFolder(fileSystem, AppKey);
		}

		[Test]
		public void TempFolderFailsOnEmptyAppKeyTest()
		{
			var fileSystem = MockRepository.GenerateStrictMock<IFileSystem>();

			var exception = Assert.Throws<ArgumentException>
				(
					() =>
					{
						var emptyAppKey = string.Empty;
						TempFolder.CleanFolder(fileSystem, emptyAppKey);
					}
				);
			Assert.AreEqual(@"applicationKey", exception.ParamName);
		}

		[Test]
		public void TempFolderFailsOnNullFileSystemTest()
		{
			const IFileSystem nullFileSystem = null;

			var exception = Assert.Throws<ArgumentNullException>
				(
					() => { TempFolder.CleanFolder(nullFileSystem, AppKey); }
				);
			Assert.AreEqual(@"fileSystem", exception.ParamName);
		}

		[Test]
		public void TempFolderGettingTempFileNameFailsOnEmptyAppKeySystemTest()
		{
			var nullFileSystem = CreateFileSystemMock(AppKey, false);
			var emptyAppKey = string.Empty;

			var exception = Assert.Throws<ArgumentException>
				(
					() => { TempFolder.GetTempFileName(nullFileSystem, emptyAppKey, FileExtension, FileNamePrefix); }
				);
			Assert.AreEqual(@"applicationKey", exception.ParamName);
		}

		[Test]
		public void TempFolderGettingTempFileNameFailsOnEmptyFileExtensionSystemTest()
		{
			var nullFileSystem = CreateFileSystemMock(AppKey, false);
			var emptyFileExtension = string.Empty;

			var exception = Assert.Throws<ArgumentException>
				(
					() => { TempFolder.GetTempFileName(nullFileSystem, AppKey, emptyFileExtension, FileNamePrefix); }
				);
			Assert.AreEqual(@"fileExtension", exception.ParamName);
		}

		[Test]
		public void TempFolderGettingTempFileNameFailsOnNullFileSystemTest()
		{
			const IFileSystem nullFileSystem = null;

			var exception = Assert.Throws<ArgumentNullException>
				(
					() => { TempFolder.GetTempFileName(nullFileSystem, AppKey, FileExtension, FileNamePrefix); }
				);
			Assert.AreEqual(@"fileSystem", exception.ParamName);
		}

		[Test]
		public void TempFolderReturnsTempFileNameTest()
		{
			// File system mock
			var fileSystem = CreateFileSystemMock(AppKey, false);

			// Act
			var tempFileName = TempFolder.GetTempFileName(fileSystem, AppKey, FileExtension);

			Assert.IsFalse(string.IsNullOrWhiteSpace(tempFileName));
			StringAssert.Contains(string.Format("{0}{1}{0}", Path.DirectorySeparatorChar, AppKey), tempFileName);
			StringAssert.EndsWith(FileExtension, tempFileName);
		}

		[Test]
		public void TempFolderReturnsTempFileNameWithExistingAppFolderTest()
		{
			// File system mock
			var fileSystem = CreateFileSystemMock(AppKey, true);

			// Act
			var tempFileName = TempFolder.GetTempFileName(fileSystem, AppKey, FileExtension);

			Assert.IsFalse(string.IsNullOrWhiteSpace(tempFileName));
			StringAssert.Contains(string.Format("{0}{1}{0}", Path.DirectorySeparatorChar, AppKey), tempFileName);
			StringAssert.EndsWith(FileExtension, tempFileName);
		}

		[Test]
		public void TempFolderReturnsTempFileNameWithFileNamePrefixTest()
		{
			// File system mock
			var fileSystem = CreateFileSystemMock(AppKey, true);

			// Act
			var tempFileName = TempFolder.GetTempFileName(fileSystem, AppKey, FileExtension, FileNamePrefix);

			Assert.IsFalse(string.IsNullOrWhiteSpace(tempFileName));
			StringAssert.Contains(string.Format("{0}{1}{0}", Path.DirectorySeparatorChar, AppKey), tempFileName);
			StringAssert.Contains($"{Path.DirectorySeparatorChar}{FileNamePrefix}-", tempFileName);
			StringAssert.EndsWith(FileExtension, tempFileName);
		}
	}
}