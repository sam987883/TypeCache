// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Reflection;
using TypeCache.Attributes;

namespace TypeCache.Reflection;

public interface IMember
{
	/// <inheritdoc cref="MemberInfo.CustomAttributes"/>
	public IReadOnlyList<Attribute> Attributes { get; }

	public bool Internal { get; }

	/// <inheritdoc cref="MemberInfo.Name"/>
	/// <remarks>Can be overwritten with <c><see cref="NameAttribute"/></c>.</remarks>
	public string Name { get; }

	public bool Public { get; }
}
