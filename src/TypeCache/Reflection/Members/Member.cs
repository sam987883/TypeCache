// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Members
{
	internal abstract class Member : IMember
	{
		private Member(MemberInfo memberInfo, Type type)
		{
			this.TypeHandle = type.TypeHandle;
			this.Attributes = memberInfo.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();
			this.Name = this.Attributes.First<Attribute, NameAttribute>()?.Name ?? memberInfo.Name;

			if (type.IsGenericType)
			{
				var genericType = type.GetGenericTypeDefinition();
				this.IsNullable = genericType == typeof(Nullable<>);
				this.IsTask = genericType == typeof(Task<>);
				this.IsValueTask = genericType == typeof(ValueTask<>);

				if (this.IsNullable || this.IsTask || this.IsValueTask)
					type = type.GenericTypeArguments[0];

				this.CollectionType = type.ToCollectionType();
				this.NativeType = type.ToNativeType();
			}
			else
			{
				this.CollectionType = type.ToCollectionType();
				this.IsTask = type == typeof(Task);
				this.IsValueTask = type == typeof(ValueTask);
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

		public bool IsTask { get; }

		public bool IsValueTask { get; }

		public string Name { get; }

		public NativeType NativeType { get; }

		public bool Public { get; }

		public RuntimeTypeHandle TypeHandle { get; }
	}
}
