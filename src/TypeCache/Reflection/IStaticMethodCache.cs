// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IStaticMethodCache<out T>
		where T : class
	{
		D? GetMethod<D>(string name) where D : Delegate;

		IImmutableDictionary<string, IImmutableList<IStaticMethodMember>> Methods { get; }
	}
}
