using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolmagic.Common.IO
{
	public sealed class FileSystem : IFileSystem
	{
		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		public void WriteAllText(string path, string contents, Encoding encoding = null)
		{
			File.WriteAllText(path, contents, encoding ?? Encoding.UTF8);
		}

		public void WriteStream(string path, Stream stream)
		{
			using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				stream.CopyTo(fileStream);
			}
		}

		public void DeleteFile(string path)
		{
			File.Delete(path);
		}

		public void DeleteDirectory(string path)
		{
			Directory.Delete(path);
		}

		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public void CopyFile(string sourceFileName, string destFileName)
		{
			File.Copy(sourceFileName, destFileName);
		}

		public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateFiles(path, searchPattern, searchOption);
		}

		public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateDirectories(path, searchPattern, searchOption);
		}

		public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);
		}

		public string GetActualPath(string path)
		{
			Argument.IsNotEmpty(path, nameof(path));

			if (FileExists(path))
			{
				return GetFileActualPath(path);
			}

			if (DirectoryExists(path))
			{
				return GetDirectoryActualPath(path);
			}

			throw new IOException($"File entry does not exist: {path}");
		}

		public Stream CreateFileStream(string path, FileMode mode, FileAccess access)
		{
			return new FileStream(path, mode, access);
		}

		public string GetTempPath()
		{
			return Path.GetTempPath();
		}

		private string GetFileActualPath(string filePath)
		{
			var fileInfo = new FileInfo(filePath);

			var directory = fileInfo.Directory;

			// ReSharper disable once PossibleNullReferenceException
			var fileSystemEntry = directory.EnumerateFileSystemInfos(fileInfo.Name).First();

			return Path.Combine(GetDirectoryActualPath(directory.FullName), fileSystemEntry.Name);
		}

		private string GetDirectoryActualPath(string directoryPath)
		{
			var pathParts = new Collection<string>();

			var directory = new DirectoryInfo(directoryPath);
			var parentDirectory = directory.Parent;

			while (parentDirectory != null)
			{
				var childDirectory = parentDirectory.EnumerateFileSystemInfos(directory.Name).First();
				pathParts.Add(childDirectory.Name);

				directory = parentDirectory;
				parentDirectory = directory.Parent;
			}

			pathParts.Add(directory.FullName.ToUpperInvariant());

			return Path.Combine(pathParts.Reverse().ToArray());
		}
	}
}