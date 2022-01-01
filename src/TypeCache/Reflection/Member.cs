// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Collections.Extensions;

namespace TypeCache.Reflection;

public abstract class Member : IMember
{
	protected Member(MemberInfo memberInfo)
	{
		this.Attributes = memberInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.Internal = memberInfo switch
		{
			Type type => !type.IsVisible,
			MethodBase methodBase => methodBase.IsAssembly,
			PropertyInfo propertyInfo => propertyInfo.GetAccessors(true).First()!.IsAssembly,
			FieldInfo fieldInfo => fieldInfo.IsAssembly,
			EventInfo eventInfo => eventInfo.AddMethod!.IsAssembly,
			_ => default
		};
		this.Name = this.Attributes.First<NameAttribute>()?.Name ?? memberInfo.Name;
		this.Public = memberInfo switch
		{
			Type type => type.IsPublic,
			MethodBase methodBase => methodBase.IsPublic,
			PropertyInfo propertyInfo => propertyInfo.GetAccessors(true).First()!.IsPublic,
			FieldInfo fieldInfo => fieldInfo.IsPublic,
			EventInfo eventInfo => eventInfo.AddMethod!.IsPublic,
			_ => default
		};
	}

	public IImmutableList<Attribute> Attributes { get; }

	public bool Internal { get; }

	/// <summary>
	/// The member name - can be overwritten with <see cref="NameAttribute"/>.
	/// </summary>
	public string Name { get; }

	public bool Public { get; }
}
