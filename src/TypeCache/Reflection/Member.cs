// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public abstract class Member
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		private Member(MemberInfo memberInfo, TypeMember? typeMember)
		{
			this.Attributes = memberInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.Name = this.Attributes.First<NameAttribute>()?.Name ?? memberInfo.Name;
			this.Type = typeMember;
		}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		protected Member(Type type)
			: this(type, null)
		{
			this._GetHashCode = type.TypeHandle.GetHashCode;
			this.Internal = !type.IsVisible;
			this.Public = type.IsPublic;
		}

		protected Member(MethodBase methodBase)
			: this(methodBase, methodBase.GetTypeMember())
		{
			this._GetHashCode = methodBase.MethodHandle.GetHashCode;
			this.Internal = methodBase.IsAssembly;
			this.Public = methodBase.IsPublic;
		}

		protected Member(PropertyInfo propertyInfo)
			: this(propertyInfo, propertyInfo.GetTypeMember())
		{
			var accessor = propertyInfo.GetAccessors(true).First()!;
			this._GetHashCode = accessor.MethodHandle.GetHashCode;
			this.Internal = accessor.IsAssembly;
			this.Public = accessor.IsPublic;
		}

		protected Member(FieldInfo fieldInfo)
			: this(fieldInfo, fieldInfo.GetTypeMember())
		{
			this._GetHashCode = fieldInfo.FieldHandle.GetHashCode;
			this.Internal = fieldInfo.IsAssembly;
			this.Public = fieldInfo.IsPublic;
		}

		protected Member(FieldInfo fieldInfo, Func<int> getHashCode)
			: this(fieldInfo, fieldInfo.GetTypeMember())
		{
			this._GetHashCode = getHashCode;
			this.Internal = fieldInfo.IsAssembly;
			this.Public = fieldInfo.IsPublic;
		}

		protected Member(EventInfo eventInfo)
			: this(eventInfo, eventInfo.GetTypeMember())
		{
			this._GetHashCode = eventInfo.AddMethod!.MethodHandle.GetHashCode;
			this.Internal = eventInfo.AddMethod!.IsAssembly;
			this.Public = eventInfo.AddMethod!.IsPublic;
		}

		private readonly Func<int> _GetHashCode;

		public IImmutableList<Attribute> Attributes { get; }

		public bool Internal { get; }

		public string Name { get; }

		public bool Public { get; }

		protected TypeMember? Type { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this._GetHashCode();
	}
}
