using System;
using System.Collections.Generic;

namespace Toolmagic.Common
{
	public static class Disposable
	{
		public static Disposable<T> Wrap<T>(T value) where T : class
		{
			var notNullValue = Argument.IsNotNull(value, nameof(value));

			return Wrap(notNullValue);
		}

		public static Disposable<T> Wrap<T>(NotNull<T> value) where T : class
		{
			return new Disposable<T>(value);
		}

		public static void Dispose<T>(T obj) where T : class
		{
			if (obj != null)
			{
				Wrap(obj).Dispose();
			}
		}

		public static void Dispose<T>(NotNull<T> obj) where T : class
		{
			Wrap(obj).Dispose();
		}

		public static DisposabeHolder CreateHolder()
		{
			return new DisposabeHolder();
		}
	}

	public sealed class Disposable<T> : IDisposable where T : class
	{
		private bool _disposedValue;
		private NotNull<T> _value;

		internal Disposable(NotNull<T> value)
		{
			_value = value;
		}

		public T Value => _value.Value;

		public void Dispose()
		{
			Dispose(true);
		}

		public static implicit operator T(Disposable<T> value)
		{
			return value.Value;
		}

		private void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					(_value.Value as IDisposable)?.Dispose();

					var disposableCollection = _value.Value as IEnumerable<IDisposable>;
					if (disposableCollection != null)
					{
						foreach (var disposable in disposableCollection)
						{
							disposable.Dispose();
						}
					}
				}
				_disposedValue = true;
			}
		}
	}
}