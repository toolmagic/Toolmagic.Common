using System.Collections.Generic;
using Microsoft.Win32;

namespace Toolmagic.Common.Shell
{
	public sealed class FileAssociation
	{
		public FileAssociation(string fileExtension)
		{
			Argument.IsNotEmpty(fileExtension, "FileAssociation");

			FileExtension = fileExtension;
			Commands = new Dictionary<string, string>();
		}

		public string FileExtension { get; }
		public string FileDescription { get; set; }
		public string FileIcon { get; set; }
		public IDictionary<string, string> Commands { get; set; }

		public static void Apply(FileAssociation fileAssociation)
		{
			Argument.IsNotNull(fileAssociation, "fileAssociation");

			var fileAssociationRootKey =
				Registry
					.ClassesRoot
					.OpenSubKey(fileAssociation.FileExtension, true);

			if (fileAssociationRootKey == null)
			{
				throw new OpenRegistryKeyException(Registry.ClassesRoot, fileAssociation.FileExtension);
			}

			fileAssociationRootKey.SetValue(null, fileAssociation.FileDescription);

			if (!string.IsNullOrWhiteSpace(fileAssociation.FileIcon))
			{
				const string defaultIconKeyName = @"DefaultIcon";

				var fileIconKey = fileAssociationRootKey.OpenSubKey(defaultIconKeyName, true);

				if (fileIconKey == null)
				{
					throw new OpenRegistryKeyException(fileAssociationRootKey, defaultIconKeyName);
				}

				fileIconKey.SetValue(null, fileAssociation.FileIcon);
			}

			if (fileAssociation.Commands.Count > 0)
			{
				const string shellKeyName = @"shell";

				var fileCommandsRootKey = fileAssociationRootKey.OpenSubKey(shellKeyName, true);

				if (fileCommandsRootKey == null)
				{
					throw new OpenRegistryKeyException(fileAssociationRootKey, shellKeyName);
				}

				foreach (var fileCommand in fileAssociation.Commands)
				{
					var fileCommandKey = fileCommandsRootKey.OpenSubKey(fileCommand.Key, true);

					if (fileCommandKey == null)
					{
						throw new OpenRegistryKeyException(fileCommandsRootKey, fileCommand.Key);
					}

					fileCommandKey.SetValue(null, fileCommand.Value);
				}
			}
		}
	}
}