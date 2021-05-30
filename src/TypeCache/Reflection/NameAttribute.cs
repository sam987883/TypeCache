// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;

namespace TypeCache.Reflection
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
	public class NameAttribute : Attribute
	{
		public NameAttribute(string name)
		{
			name.AssertNotBlank(nameof(name));
			this.Name = name;
		}

		public string Name { get; }
	}
}
