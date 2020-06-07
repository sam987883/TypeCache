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
			this.Attributes = info.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();
			var (nameAttribute, exists) = this.Attributes.If<Attribute, NameAttribute>().First();
			this.Name = exists ? nameAttribute.Name : info.Name;
			this.TypeHandle = type.TypeHandle;
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

		public IImmutableList<Attribute> Attributes { get; }

		public bool Internal { get; }

		public string Name { get; }

		public bool Public { get; }

		public RuntimeTypeHandle TypeHandle { get; }
	}
}
