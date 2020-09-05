// Copyright (c) 2020 Samuel Abraham

using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IStaticPropertyCache<T>
		where T : class
	{
		IImmutableDictionary<string, IStaticPropertyMember> Properties { get; }
	}
}
