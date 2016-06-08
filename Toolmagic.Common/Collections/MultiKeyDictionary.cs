using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolmagic.Common.Collections
{
	public sealed class MultiKeyDictionary<TKey1, TKey2, TValue> : IMultiKeyDictionary<TKey1, TKey2, TValue>
	{
		private readonly IDictionary<TKey1, KeyKeyValueTriple> _map1 = new Dictionary<TKey1, KeyKeyValueTriple>();
		private readonly IDictionary<TKey2, KeyKeyValueTriple> _map2 = new Dictionary<TKey2, KeyKeyValueTriple>();

		public void Add(TKey1 key1, TKey2 key2, TValue value)
		{
			if (_map1.ContainsKey(key1))
			{
				throw new ArgumentException($@"Duplicate key1: {key1}", nameof(key1));
			}

			if (_map2.ContainsKey(key2))
			{
				throw new ArgumentException($@"Duplicate key2: {key2}", nameof(key2));
			}

			var item = new KeyKeyValueTriple
			{
				Key1 = key1,
				Key2 = key2,
				Value = value
			};

			_map1.Add(key1, item);
			_map2.Add(key2, item);
		}

		public bool ContainsKey1(TKey1 key1)
		{
			return _map1.ContainsKey(key1);
		}

		public bool ContainsKey2(TKey2 key2)
		{
			return _map2.ContainsKey(key2);
		}

		public bool TryGetValueByKey1(TKey1 key1, out TValue value)
		{
			KeyKeyValueTriple item;
			if (_map1.TryGetValue(key1, out item))
			{
				value = item.Value;
				return true;
			}

			value = default(TValue);
			return false;
		}

		public bool TryGetValueByKey2(TKey2 key2, out TValue value)
		{
			KeyKeyValueTriple item;
			if (_map2.TryGetValue(key2, out item))
			{
				value = item.Value;
				return true;
			}

			value = default(TValue);
			return false;
		}

		public bool TryGetKey1(TKey2 key2, out TKey1 key1)
		{
			KeyKeyValueTriple item;
			if (_map2.TryGetValue(key2, out item))
			{
				key1 = item.Key1;
				return true;
			}

			key1 = default(TKey1);
			return false;
		}

		public bool TryGetKey2(TKey1 key1, out TKey2 key2)
		{
			KeyKeyValueTriple item;
			if (_map1.TryGetValue(key1, out item))
			{
				key2 = item.Key2;
				return true;
			}

			key2 = default(TKey2);
			return false;
		}

		public void RemoveByKey1(TKey1 key1)
		{
			var item = _map1[key1];
			_map1.Remove(item.Key1);
			_map2.Remove(item.Key2);
		}

		public void RemoveByKey2(TKey2 key2)
		{
			var item = _map2[key2];
			_map1.Remove(item.Key1);
			_map2.Remove(item.Key2);
		}

		public IEnumerable<TKey1> Keys1 => _map1.Keys;

		public IEnumerable<TKey2> Keys2 => _map2.Keys;

		public IEnumerable<TValue> Values
		{
			get { return _map1.Values.Select(v => v.Value); }
		}

		public int Count => _map1.Count;

		private sealed class KeyKeyValueTriple
		{
			public TKey1 Key1 { get; set; }
			public TKey2 Key2 { get; set; }
			public TValue Value { get; set; }
		}
	}
}