﻿// Copyright (c) 2020 Samuel Abraham

using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IFieldCache<T>
		where T : class
	{
		IImmutableDictionary<string, IFieldMember<T>> Fields { get; }

		IImmutableList<string> GetNames { get; }

		IImmutableList<string> SetNames { get; }

		void Map(T from, T to);
	}
}
