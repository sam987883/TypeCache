// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IMethodCache<out T>
	{
		D? GetMethod<D>(string name) where D : Delegate;

		IImmutableDictionary<string, IImmutableList<IMethodMember>> Methods { get; }
	}
}
