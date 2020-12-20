// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IStaticPropertyCache<out T>
		where T : class
	{
		IImmutableDictionary<string, IStaticPropertyMember> Properties { get; }
	}
}
