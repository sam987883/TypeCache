// Copyright (c) 2020 Samuel Abraham

using sam987883.Extensions;
using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
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

		public bool IsString { get; }

		public bool IsValueType { get; }

		public string Name { get; }

		public bool Public { get; }

		public RuntimeTypeHandle TypeHandle { get; }

		public static MethodCallExpression? ConvertExpression(Expression value, Type type) => Type.GetTypeCode(type) switch
		{
			TypeCode.Boolean => Expression.Call(typeof(Convert), nameof(Convert.ToBoolean), null, value),
			TypeCode.Char => Expression.Call(typeof(Convert), nameof(Convert.ToChar), null, value),
			TypeCode.SByte => Expression.Call(typeof(Convert), nameof(Convert.ToSByte), null, value),
			TypeCode.Byte => Expression.Call(typeof(Convert), nameof(Convert.ToByte), null, value),
			TypeCode.Int16 => Expression.Call(typeof(Convert), nameof(Convert.ToInt16), null, value),
			TypeCode.UInt16 => Expression.Call(typeof(Convert), nameof(Convert.ToUInt16), null, value),
			TypeCode.Int32 => Expression.Call(typeof(Convert), nameof(Convert.ToInt32), null, value),
			TypeCode.UInt32 => Expression.Call(typeof(Convert), nameof(Convert.ToUInt32), null, value),
			TypeCode.Int64 => Expression.Call(typeof(Convert), nameof(Convert.ToInt64), null, value),
			TypeCode.UInt64 => Expression.Call(typeof(Convert), nameof(Convert.ToUInt64), null, value),
			TypeCode.Single => Expression.Call(typeof(Convert), nameof(Convert.ToSingle), null, value),
			TypeCode.Double => Expression.Call(typeof(Convert), nameof(Convert.ToDouble), null, value),
			TypeCode.Decimal => Expression.Call(typeof(Convert), nameof(Convert.ToDecimal), null, value),
			TypeCode.DateTime => Expression.Call(typeof(Convert), nameof(Convert.ToDateTime), null, value),
			TypeCode.String => Expression.Call(typeof(Convert), nameof(Convert.ToString), null, value),
			_ => null
		};
	}
}
