// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using static sam987883.Extensions.IEnumerableExtensions;

namespace sam987883.Reflection.Members
{
	internal sealed class Parameter : IParameter
	{
		public Parameter(ParameterInfo parameterInfo)
		{
			this.Attributes = parameterInfo.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();
			var (nameAttribute, exists) = this.Attributes.If<Attribute, NameAttribute>().First();
			this.Name = exists ? nameAttribute.Name : parameterInfo.Name;
			this.TypeHandle = parameterInfo.ParameterType.TypeHandle;
			this.DefaultValue = parameterInfo.DefaultValue;
			this.HasDefaultValue = parameterInfo.HasDefaultValue;
			this.Optional = parameterInfo.IsOptional;
			this.Out = parameterInfo.IsOut;
		}

		public IImmutableList<Attribute> Attributes { get; }

		public object? DefaultValue { get; }

		public bool HasDefaultValue { get; }

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
