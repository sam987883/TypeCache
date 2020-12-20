// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;

namespace TypeCache.Reflection.Members
{
	internal sealed class EnumMember<T> : Member, IEnumMember<T>
		where T : struct, Enum
	{
		public EnumMember(FieldInfo fieldInfo, T value) : base(fieldInfo)
		{
			this.Value = value;
		}

		public T Value { get; }
	}
}
