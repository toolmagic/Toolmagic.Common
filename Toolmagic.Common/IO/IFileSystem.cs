using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Toolmagic.Common.IO
{
	public interface IFileSystem
	{
		bool FileExists(string path);
		bool DirectoryExists(string path);
		void WriteAllText(string path, string contents, Encoding encoding = null);
		void WriteStream(string path, Stream stream);
		void DeleteFile(string path);
		void DeleteDirectory(string path);
		void CreateDirectory(string path);
		Stream CreateFileStream(string path, FileMode mode, FileAccess access);
		string GetTempPath();
		void CopyFile(string sourceFileName, string destFileName);
		IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
		IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);
		IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption);
	}
}