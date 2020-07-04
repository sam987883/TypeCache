// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using static sam987883.Extensions.IEnumerableExtensions;

namespace sam987883.Reflection.Members
{
	internal abstract class Member : IMember
	{
		private Member(MemberInfo info, Type type)
		{
			this.ArrayTypeHandles = type.IsArray
				? type.GenericTypeArguments
					.To(_ => _.TypeHandle)
					.ToImmutable(type.GenericTypeArguments.Length)
				: ImmutableArray<RuntimeTypeHandle>.Empty;

			this.Attributes = info.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();

			this.IsString = type == typeof(string);
			this.IsValueType = type.IsValueType;

			var (nameAttribute, exists) = this.Attributes.If<Attribute, NameAttribute>().First();
			this.Name = exists ? nameAttribute.Name : info.Name;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				this.IsNullable = true;
				this.TypeHandle = type.GenericTypeArguments[0].TypeHandle;
			}
			else
			{
				this.IsNullable = type.IsClass;
				this.TypeHandle = type.TypeHandle;
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

			this.Internal = methodInfo.IsAssembly;
			this.Public = methodInfo.IsPublic;
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

		public bool IsString { get; }

		public bool IsValueType { get; }

		public string Name { get; }

		public bool Public { get; }

		public RuntimeTypeHandle TypeHandle { get; }
	}
}
