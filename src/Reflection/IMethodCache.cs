// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IMethodCache<T>
		where T : class
	{
		(D? Method, bool Exists) GetMethod<D>(string name) where D : Delegate;

		IImmutableDictionary<string, IImmutableList<IMethodMember<T>>> Methods { get; }
	}
}
