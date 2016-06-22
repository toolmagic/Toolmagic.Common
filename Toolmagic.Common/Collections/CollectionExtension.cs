using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolmagic.Common.Collections
{
	public static class CollectionExtension
	{
		public static ICollection<T> AddRange<T>(this ICollection<T> source, IEnumerable<T> collection)
		{
			Argument.IsNotNull(source, nameof(source));

			// ReSharper disable once PossibleMultipleEnumeration
			Argument.IsNotNull(collection, nameof(collection));

			if (source.IsReadOnly)
			{
				throw new ArgumentException("Can't add items to read-only collection", nameof(source));
			}

			// ReSharper disable once PossibleMultipleEnumeration
			foreach (var item in collection)
			{
				source.Add(item);
			}

			return source;
		}

		public static ICollection<T> AddRange<T>(this ICollection<T> source, IList<T> collection, int start, int count)
		{
			Argument.IsNotNull(source, nameof(source));
			Argument.IsNotNull(collection, nameof(collection));
			Argument.IsInRange(start, nameof(start), 0, collection.Count - 1);
			Argument.IsInRange(count, nameof(count), 0, collection.Count - start);

			if (source.IsReadOnly)
			{
				throw new ArgumentException("Can't add items to read-only collection", nameof(source));
			}

			for (var i = start; i < start + count; i++)
			{
				source.Add(collection[i]);
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