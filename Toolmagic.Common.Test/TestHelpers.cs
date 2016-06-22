using System;
using System.IO;

namespace Toolmagic.Common.Test
{
	public static class TestHelpers
	{
		public static Stream GetResourceStream(string resourceName)
		{
			Argument.IsNotEmpty(resourceName, nameof(resourceName));

			var fullResourceName = $"{typeof(TestHelpers).Namespace}.{resourceName}";
			var stream = typeof(TestHelpers).Assembly.GetManifestResourceStream(fullResourceName);

			if (stream == null)
			{
				throw new Exception("Unknown resource: " + fullResourceName);
			}

			return stream;
		}
	}
}