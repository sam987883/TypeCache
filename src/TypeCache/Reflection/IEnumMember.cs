// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Reflection
{
	public interface IEnumMember<out T> : IMember
		where T : struct, Enum
	{
		T Value { get; }
	}
}
