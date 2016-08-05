using System.IO;
using System.Linq;

namespace Toolmagic.Common.IO
{
	public static class FileSystemExtensions
	{
		public static bool IsDirectoryEmpty(this IFileSystem fileSystem, string path)
		{
			Argument.IsNotEmpty(path, nameof(path));

			if (!fileSystem.DirectoryExists(path))
			{
				throw new DirectoryNotFoundException(path);
			}

			return !fileSystem
				.EnumerateFileSystemEntries(path, @"*", SearchOption.TopDirectoryOnly)
				.Any();
		}
	}
}