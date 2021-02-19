// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TypeCache.Collections;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public static class MemberCache
	{
		static MemberCache()
		{
			Constructors = new LazyDictionary<RuntimeTypeHandle, IImmutableList<ConstructorMember>>(MemberFactory.CreateConstructorMembers);
			Fields = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, FieldMember>>(MemberFactory.CreateFieldMembers);
			Indexers = new LazyDictionary<RuntimeTypeHandle, IImmutableList<IndexerMember>>(MemberFactory.CreateIndexerMembers);
			Methods = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<MethodMember>>>(MemberFactory.CreateMethodMembers);
			Properties = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, PropertyMember>>(MemberFactory.CreatePropertyMembers);
			StaticFields = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, StaticFieldMember>>(MemberFactory.CreateStaticFieldMembers);
			StaticMethods = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<StaticMethodMember>>>(MemberFactory.CreateStaticMethodMembers);
			StaticProperties = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, StaticPropertyMember>>(MemberFactory.CreateStaticPropertyMembers);
			Types = new LazyDictionary<RuntimeTypeHandle, TypeMember>(typeHandle => MemberFactory.CreateTypeMember(typeHandle.ToType()));
		}

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableList<ConstructorMember>> Constructors { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, FieldMember>> Fields { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableList<IndexerMember>> Indexers { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<MethodMember>>> Methods { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, PropertyMember>> Properties { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, StaticFieldMember>> StaticFields { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<StaticMethodMember>>> StaticMethods { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, StaticPropertyMember>> StaticProperties { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, TypeMember> Types { get; }
	}
}
