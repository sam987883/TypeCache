// Copyright (c) 2020 Samuel Abraham

using System;

namespace Sam987883.Reflection
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class NameAttribute : Attribute
	{
		public NameAttribute(string name) =>
			this.Name = name;

		public string Name { get; }
	}
}
