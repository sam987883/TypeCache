// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Cache
{
	internal static class ConstructorCache<T>
	{
		internal static IImmutableList<IConstructorMember> Constructors { get; }

		static ConstructorCache()
		{
			Constructors = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.To(constructorInfo => (IConstructorMember)new ConstructorMember(constructorInfo))
				.ToImmutable();
		}
	}
}
