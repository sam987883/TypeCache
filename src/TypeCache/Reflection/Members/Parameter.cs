// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using TypeCache.Common;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Members
{
	internal sealed class Parameter : IParameter
	{
		public Parameter(ParameterInfo parameterInfo)
		{
			this.TypeHandle = parameterInfo.ParameterType.TypeHandle;
			this.Attributes = parameterInfo.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();
			this.Name = this.Attributes.First<Attribute, NameAttribute>()?.Name ?? parameterInfo.Name ?? string.Empty;
			this.CollectionType = parameterInfo.ParameterType.ToCollectionType();
			this.DefaultValue = parameterInfo.DefaultValue;
			this.HasDefaultValue = parameterInfo.HasDefaultValue;
			this.IsOptional = parameterInfo.IsOptional;
			this.IsOut = parameterInfo.IsOut;

			if (parameterInfo.ParameterType.IsGenericType)
			{
				var genericType = parameterInfo.ParameterType.GetGenericTypeDefinition();
				this.IsNullable = genericType == typeof(Nullable<>);
				this.IsTask = genericType == typeof(Task<>);
				this.IsValueTask = genericType == typeof(ValueTask<>);

				var type = parameterInfo.ParameterType.GenericTypeArguments[0];
				this.TypeHandle = type.TypeHandle;
				this.NativeType = type.ToNativeType();
			}
			else
			{
				this.IsNullable = parameterInfo.ParameterType.IsClass;
				this.NativeType = parameterInfo.ParameterType.ToNativeType();
			}
		}

		public IImmutableList<Attribute> Attributes { get; }

		public CollectionType CollectionType { get; }

		public object? DefaultValue { get; }

		public bool HasDefaultValue { get; }

		public bool IsNullable { get; }

		public bool IsOptional { get; }

		public bool IsOut { get; }

		public bool IsTask { get; }

		public bool IsValueTask { get; }

		public string Name { get; }

		public NativeType NativeType { get; }

		public RuntimeTypeHandle TypeHandle { get; }

		public bool Supports(Type type)
		{
			var parameterType = this.TypeHandle.ToType();
			return type.Equals(parameterType) || type.IsSubclassOf(parameterType);
		}
	}
}
