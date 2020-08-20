// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Extensions;
using Sam987883.Reflection.Extensions;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace Sam987883.Reflection.Members
{
	internal abstract class Member : IMember
	{
		private Member(MemberInfo memberInfo, Type type)
		{
			this.ArrayTypeHandles = type.IsArray
				? type.GenericTypeArguments
					.To(_ => _.TypeHandle)
					.ToImmutable(type.GenericTypeArguments.Length)
				: ImmutableArray<RuntimeTypeHandle>.Empty;

			this.Attributes = memberInfo.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();
			this.IsNullable = type.IsClass;

			var (nameAttribute, exists) = this.Attributes.If<Attribute, NameAttribute>().First();
			this.Name = exists ? nameAttribute.Name : memberInfo.Name;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				this.IsNullable = true;
				type = type.GenericTypeArguments[0];
			}

			this.TypeHandle = type.TypeHandle;
			this.Type = type.ToNativeType();
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

		public IImmutableList<RuntimeTypeHandle> ArrayTypeHandles { get; }

		public IImmutableList<Attribute> Attributes { get; }

		public bool Internal { get; }

		public bool IsNullable { get; }

		public string Name { get; }

		public bool Public { get; }

		public NativeType Type { get; }

		public RuntimeTypeHandle TypeHandle { get; }
	}
}
