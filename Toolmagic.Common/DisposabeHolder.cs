using System;
using System.Collections.Generic;

namespace Toolmagic.Common
{
	public sealed class DisposabeHolder : IDisposable
	{
		private readonly ICollection<object> _objects = new List<object>();

		internal DisposabeHolder()
		{
		}

		public void Dispose()
		{
			foreach (var obj in _objects)
			{
				(obj as IDisposable)?.Dispose();
			}
		}

		public void Register(object obj)
		{
			_objects.Add(obj);
		}

		public void Cancel()
		{
			_objects.Clear();
		}
	}
}