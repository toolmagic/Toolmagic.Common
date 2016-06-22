using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Toolmagic.Common.IO
{
	/// <summary>
	///     Represents routines for temp folders specific for applications.
	/// </summary>
	public static class TempFolder
	{
		/// <summary>
		///     Returns new temp file name in directory specific for the application.
		/// </summary>
		/// <param name="fileSystem">File system instance.</param>
		/// <param name="applicationKey">Application key (actually it's sub-folder of temp directory).</param>
		/// <param name="fileExtension">Required file extension.</param>
		/// <param name="fileNamePrefix">Optional file name prefix.</param>
		/// <returns>Temporary file name.</returns>
		public static string GetTempFileName(IFileSystem fileSystem, string applicationKey, string fileExtension,
			string fileNamePrefix = null)
		{
			Argument.IsNotNull(fileSystem, "fileSystem");
			Argument.IsNotEmpty(applicationKey, "applicationKey");
			Argument.IsNotEmpty(fileExtension, "fileExtension");

			var rootApplicationFolder = Path.Combine(fileSystem.GetTempPath(), applicationKey);
			if (!fileSystem.DirectoryExists(rootApplicationFolder))
			{
				fileSystem.CreateDirectory(rootApplicationFolder);
			}

			var fileName = Guid.NewGuid().ToString("N");
			if (!string.IsNullOrWhiteSpace(fileNamePrefix))
			{
				fileName = $"{fileNamePrefix}-{fileName}";
			}

			return Path.Combine(rootApplicationFolder, Path.ChangeExtension(fileName, fileExtension));
		}

		/// <summary>
		///     Cleans temp folder specific for the application.
		/// </summary>
		/// <param name="fileSystem">File system instance.</param>
		/// <param name="applicationKey">Application key (actually it's sub-folder of temp directory).</param>
		public static void CleanFolder(IFileSystem fileSystem, string applicationKey)
		{
			Argument.IsNotNull(fileSystem, "fileSystem");
			Argument.IsNotEmpty(applicationKey, "applicationKey");

			var rootApplicationFolder = Path.Combine(fileSystem.GetTempPath(), applicationKey);

			if (!fileSystem.DirectoryExists(rootApplicationFolder))
			{
				return;
			}

			var fileSystemEntries = fileSystem
				.EnumerateFileSystemEntries(rootApplicationFolder, "*.*", SearchOption.AllDirectories)
				.OrderByDescending(f => f.Length);

			foreach (var fileSystemEntry in fileSystemEntries)
			{
				try
				{
					if (fileSystem.FileExists(fileSystemEntry))
					{
						fileSystem.DeleteFile(fileSystemEntry);
					}
					else if (fileSystem.DirectoryExists(fileSystemEntry))
					{
						fileSystem.DeleteDirectory(fileSystemEntry);
					}
				}
				catch (IOException t)
				{
					Trace.WriteLine($"{t.Message}\r\n{t.StackTrace}");
				}
			}
		}
	}
}