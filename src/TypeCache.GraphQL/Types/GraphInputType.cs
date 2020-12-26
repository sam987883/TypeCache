// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using System;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphInputType<T> : InputObjectGraphType<T> where T : class
	{
		public GraphInputType(IPropertyCache<T> propertyCache)
		{
			var graphAttribute = typeof(T).GetCustomAttributes(true).First<object, GraphAttribute>();
			this.Name = graphAttribute?.Name ?? typeof(T).GetName();

			propertyCache.Properties.Values
				.If(property => property.GetMethod != null && property.Attributes.First<Attribute, GraphAttribute>()?.Ignore != true)
				.Do(property => this.AddField(CreateFieldType(property)));
		}

		private static FieldType CreateFieldType(IPropertyMember property)
		{
			var graphAttribute = property.Attributes.First<Attribute, GraphAttribute>();
			return new FieldType
			{
				Type = graphAttribute?.Type ?? property.GetGraphType(true),
				Name = graphAttribute?.Name ?? property.Name,
				Description = graphAttribute?.Description,
				DeprecationReason = property.Attributes.First<Attribute, ObsoleteAttribute>()?.Message
			};
		}
	}
}
