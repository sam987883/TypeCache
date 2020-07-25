// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using static sam987883.Common.Extensions.IEnumerableExtensions;

namespace sam987883.Reflection.Members
{
	internal sealed class Parameter : IParameter
	{
		public Parameter(ParameterInfo parameterInfo)
		{
			this.ArrayTypeHandles = parameterInfo.ParameterType.IsArray
				? parameterInfo.ParameterType.GenericTypeArguments
					.To(_ => _.TypeHandle)
					.ToImmutable(parameterInfo.ParameterType.GenericTypeArguments.Length)
				: ImmutableArray<RuntimeTypeHandle>.Empty;
			this.Attributes = parameterInfo.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();
			this.Name = this.Attributes.If<Attribute, NameAttribute>().First().Value?.Name ?? parameterInfo.Name ?? string.Empty;
			this.DefaultValue = parameterInfo.DefaultValue;
			this.HasDefaultValue = parameterInfo.HasDefaultValue;
			this.IsString = parameterInfo.ParameterType == typeof(string);
			this.IsValueType = parameterInfo.ParameterType.IsValueType;
			this.Optional = parameterInfo.IsOptional;
			this.Out = parameterInfo.IsOut;

			if (parameterInfo.ParameterType.IsGenericType && parameterInfo.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				this.IsNullable = true;
				this.TypeHandle = parameterInfo.ParameterType.GenericTypeArguments[0].TypeHandle;
			}
			else
			{
				this.IsNullable = parameterInfo.ParameterType.IsClass;
				this.TypeHandle = parameterInfo.ParameterType.TypeHandle;
			}
		}

		public IImmutableList<RuntimeTypeHandle> ArrayTypeHandles { get; }

		public IImmutableList<Attribute> Attributes { get; }

		public object? DefaultValue { get; }

		public bool HasDefaultValue { get; }

		public bool IsNullable { get; }

		public bool IsString { get; }

		public bool IsValueType { get; }

		public string Name { get; }

		public bool Optional { get; }

		public bool Out { get; }

		public RuntimeTypeHandle TypeHandle { get; }

		public bool Supports(Type type)
		{
			var parameterType = Type.GetTypeFromHandle(this.TypeHandle);
			return type.Equals(parameterType) || type.IsSubclassOf(parameterType);
		}
	}
}
