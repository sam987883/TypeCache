// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IEnumCache<T> : IMember
		where T : struct, Enum
	{
		IImmutableList<IEnumMember<T>> Fields { get; }

		bool Flags { get; }

		NativeType UnderlyingType { get; }

		RuntimeTypeHandle UnderlyingTypeHandle { get; }
	}
}
