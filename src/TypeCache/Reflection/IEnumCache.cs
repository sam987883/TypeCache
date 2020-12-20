// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using TypeCache.Common;

namespace TypeCache.Reflection
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
