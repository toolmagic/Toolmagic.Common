using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolmagic.Common.Collections
{
	public static class CollectionExtension
	{
		public static ICollection<T> AddRange<T>(this ICollection<T> source, IEnumerable<T> addition)
		{
			Argument.IsNotNull(source, @"source");

			// ReSharper disable once PossibleMultipleEnumeration
			Argument.IsNotNull(addition, @"addition");

			// ReSharper disable once PossibleMultipleEnumeration
			foreach (var item in addition)
			{
				source.Add(item);
			}

			return source;
		}

		public static ICollection<T> AddRange<T>(this ICollection<T> source, IList<T> addition, int start, int count)
		{
			Argument.IsNotNull(source, @"source");
			Argument.IsNotNull(addition, @"addition");
			Argument.IsInRange(start, @"start", 0, addition.Count - 1);
			Argument.IsInRange(count, @"count", 0, addition.Count - 1 - start);

			for (var i = start; i < start + count; i++)
			{
				source.Add(addition[i]);
			}

			return source;
		}

		public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int chunkSize)
		{
			// ReSharper disable once PossibleMultipleEnumeration
			Argument.IsNotNull(source, @"source");
			Argument.IsInRange(chunkSize, @"chunkSize", 1, int.MaxValue);

			var result = new List<List<T>>();

			var currentChunkSize = 0;
			// ReSharper disable once PossibleMultipleEnumeration
			foreach (var item in source)
			{
				if (currentChunkSize == 0)
				{
					result.Add(new List<T>(Math.Min(chunkSize, 4096)));
				}

				result.Last().Add(item);

				currentChunkSize = (currentChunkSize + 1)%chunkSize;
			}

			return result;
		}
	}
}