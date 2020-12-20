// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using System;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Common;

namespace TypeCache.Reflection.Members
{
	internal abstract class Member : IMember
	{
		private Member(MemberInfo memberInfo, Type type)
		{
			this.TypeHandle = type.TypeHandle;
			this.Attributes = memberInfo.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();
			this.Name = this.Attributes.First<Attribute, NameAttribute>()?.Name ?? memberInfo.Name;
			this.CollectionType = type.ToCollectionType();

			if (type.IsGenericType)
			{
				this.IsNullable = type.GetGenericTypeDefinition() == typeof(Nullable<>);
				this.NativeType = type.GenericTypeArguments[0].ToNativeType();
			}
			else
			{
				this.IsNullable = false;
				this.NativeType = type.ToNativeType();
			}
		}

		protected Member(ConstructorInfo info) : this(info, info.DeclaringType)
		{
			this.Internal = info.IsAssembly;
			this.Public = info.IsPublic;
		}

		protected Member(FieldInfo info) : this(info, info.FieldType)
		{
			this.Internal = info.IsAssembly;
			this.Public = info.IsPublic;
		}

		protected Member(MethodInfo info) : this(info, info.ReturnType)
		{
			this.Internal = info.IsAssembly;
			this.Public = info.IsPublic;
		}

		protected Member(PropertyInfo propertyInfo) : this(propertyInfo, propertyInfo.PropertyType)
		{
			var methodInfo = propertyInfo.GetMethod ?? propertyInfo.SetMethod;
			if (methodInfo != null)
			{
				this.Internal = methodInfo.IsAssembly;
				this.Public = methodInfo.IsPublic;
			}
		}

		protected Member(Type type) : this(type, type)
		{
			this.Internal = !type.IsVisible;
			this.Public = type.IsPublic;
		}

		public IImmutableList<Attribute> Attributes { get; }

		public CollectionType CollectionType { get; }

		public bool Internal { get; }

		public bool IsNullable { get; }

		public string Name { get; }

		public NativeType NativeType { get; }

		public bool Public { get; }

		public RuntimeTypeHandle TypeHandle { get; }
	}
}
