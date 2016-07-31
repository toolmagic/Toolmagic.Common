using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Toolmagic.Common.IO;

namespace Toolmagic.Common.Test.IO
{
	[TestFixture]
	public sealed class FileSystemTestSuite
	{
		[Test]
		public void GetActualDirectoryNameTest()
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				Assert.Inconclusive("This test supports Windows NT only");
			}

			var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Assert.IsNotNull(path);

			System.Console.WriteLine(path);

			IFileSystem fileSystem = new FileSystem();

			Assert.AreEqual(path, fileSystem.GetActualPath(path));
			Assert.AreEqual(path, fileSystem.GetActualPath(path.ToUpperInvariant()));
			Assert.AreEqual(path, fileSystem.GetActualPath(path.ToLowerInvariant()));
		}

		[Test]
		public void GetActualFileNameTest()
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				Assert.Inconclusive("This test supports Windows NT only");
			}

			var fileName = Assembly.GetExecutingAssembly().Location;

			// HACK: fix R# test runner bug with upper-case extension of assembly file
			if (fileName.Contains(@"AppData\Local\Temp"))
			{
				fileName = fileName.Substring(0, fileName.Length - ".dll".Length) + ".DLL";
			}

			System.Console.WriteLine(fileName);

			IFileSystem fileSystem = new FileSystem();

			Assert.AreEqual(fileName, fileSystem.GetActualPath(fileName));
			Assert.AreEqual(fileName, fileSystem.GetActualPath(fileName.ToUpperInvariant()));
			Assert.AreEqual(fileName, fileSystem.GetActualPath(fileName.ToLowerInvariant()));
		}

		// TODO: add UNIX or Samba tests (case-sensitive)

		// TODO: add UNC tests ("\\"-roots)
	}
}