using System;
using Microsoft.Win32;

namespace Toolmagic.Common.Shell
{
	[Serializable]
	public sealed class OpenRegistryKeyException : Exception
	{
		public OpenRegistryKeyException(RegistryKey parentKey, string childKeyName)
			: base(FormatMessage(parentKey, childKeyName))
		{
		}

		private static string FormatMessage(RegistryKey parentKey, string childKeyName)
		{
			Argument.IsNotNull(parentKey, nameof(parentKey));
			Argument.IsNotEmpty(childKeyName, nameof(childKeyName));

			return $@"Can't create or open {parentKey}\{childKeyName} registry key";
		}
	}
}