// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IStaticFieldCache<out T>
		where T : class
	{
		IImmutableDictionary<string, IStaticFieldMember> Fields { get; }
	}
}
