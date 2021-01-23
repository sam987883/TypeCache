// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;

namespace TypeCache.Reflection
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
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
