using System.Collections.Generic;

namespace Toolmagic.Common.Collections
{
	public interface IMultiKeyDictionary<TKey1, TKey2, TValue>
	{
		int Count { get; }

		IEnumerable<TKey1> Keys1 { get; }
		IEnumerable<TKey2> Keys2 { get; }
		IEnumerable<TValue> Values { get; }
		void Add(TKey1 key1, TKey2 key2, TValue value);

		bool ContainsKey1(TKey1 key1);
		bool ContainsKey2(TKey2 key2);

		bool TryGetValueByKey1(TKey1 key1, out TValue value);
		bool TryGetValueByKey2(TKey2 key2, out TValue value);

		bool TryGetKey1(TKey2 key2, out TKey1 key1);
		bool TryGetKey2(TKey1 key1, out TKey2 key2);

		void RemoveByKey1(TKey1 key1);
		void RemoveByKey2(TKey2 key2);
	}
}