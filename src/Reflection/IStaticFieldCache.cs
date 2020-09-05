// Copyright (c) 2020 Samuel Abraham

using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IStaticFieldCache<T>
		where T : class
	{
		IImmutableDictionary<string, IStaticFieldMember> Fields { get; }
	}
}
