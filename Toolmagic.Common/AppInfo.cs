using System.Reflection;

namespace Toolmagic.Common
{
	public static class AppInfo
	{
		public static string ProductName
			=> Assembly.GetEntryAssembly()
				.GetCustomAttribute<AssemblyProductAttribute>()
				.Product;

		public static string ProductVersion
			=> Assembly.GetEntryAssembly()
				.GetCustomAttribute<AssemblyFileVersionAttribute>()
				.Version;

		public static string ProductNameAndVersion
			=> $"{ProductName} v.{ProductVersion}";
	}
}