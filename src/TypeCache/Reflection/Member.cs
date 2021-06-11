// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Collections.Extensions;

namespace TypeCache.Reflection
{
	public abstract class Member
	{
		internal Member(MemberInfo memberInfo, bool isInternal, bool isPublic)
		{
			this.Attributes = memberInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.Internal = isInternal;
			this.Name = this.Attributes.First<NameAttribute>()?.Name ?? memberInfo.Name;
			this.Public = isPublic;
		}

		public IImmutableList<Attribute> Attributes { get; }

		public bool Internal { get; }

		public string Name { get; }

		public bool Public { get; }
	}
}
