// Copyright (c) 2020 Samuel Abraham

using System;
using System.Reflection;
using static sam987883.Extensions.EnumExtensions;

namespace sam987883.Reflection.Members
{
	internal sealed class EnumMember<T> : Member, IEnumMember<T>
		where T : struct, Enum
	{
		public EnumMember(FieldInfo fieldInfo, T value) : base(fieldInfo)
		{
			this.Name = value.Name();
			this.Value = value;
		}

		public string Name { get; }

		public T Value { get; }
	}
}
