// Copyright (c) 2020 Samuel Abraham

using Sam987883.Reflection.Extensions;
using System;
using System.Collections.Immutable;
using System.Reflection;
using static Sam987883.Common.Extensions.IEnumerableExtensions;

namespace Sam987883.Reflection.Members
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
			this.IsNullable = parameterInfo.ParameterType.IsClass;
			this.Optional = parameterInfo.IsOptional;
			this.Out = parameterInfo.IsOut;

			if (parameterInfo.ParameterType.IsGenericType && parameterInfo.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				this.IsNullable = true;
				var type = parameterInfo.ParameterType.GenericTypeArguments[0];
				this.TypeHandle = type.TypeHandle;
				this.Type = type.ToNativeType();
			}
			else
			{
				this.TypeHandle = parameterInfo.ParameterType.TypeHandle;
				this.Type = parameterInfo.ParameterType.ToNativeType();
			}
		}

		public IImmutableList<RuntimeTypeHandle> ArrayTypeHandles { get; }

		public IImmutableList<Attribute> Attributes { get; }

		public object? DefaultValue { get; }

		public bool HasDefaultValue { get; }

		public bool IsNullable { get; }

		public string Name { get; }

		public bool Optional { get; }

		public bool Out { get; }

		public NativeType Type { get; }

		public RuntimeTypeHandle TypeHandle { get; }

		public bool Supports(Type type)
		{
			var parameterType = System.Type.GetTypeFromHandle(this.TypeHandle);
			return type.Equals(parameterType) || type.IsSubclassOf(parameterType);
		}
	}
}
