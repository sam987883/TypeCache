// Copyright (c) 2020 Samuel Abraham

using System;

namespace Sam987883.Reflection
{
	public interface IEnumMember<out T> : IMember
		where T : struct, Enum
	{
		T Value { get; }
	}
}
