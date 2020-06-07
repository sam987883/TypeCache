// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using System;

namespace sam987883.Dependencies
{
	internal class NamedService<T>
	{
		private readonly LazyLoad<T> _Service;

		public NamedService(string name, T service) : this(name, () => service) { }

		public NamedService(string name, Func<T> createService)
		{
			this.Name = name;
			this._Service = new LazyLoad<T>(createService);
		}

		public string Name { get; }

		public T Service => this._Service.Value;
	}
}
