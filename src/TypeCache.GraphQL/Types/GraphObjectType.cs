// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.Types;
using System;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphObjectType<T> : ObjectGraphType<T> where T : class
	{
		public GraphObjectType(IPropertyCache<T> propertyCache, IPropertyCache<IResolveFieldContext<T>> contextPropertyCache)
		{
			var graphAttribute = typeof(T).GetCustomAttributes(true).First<object, GraphAttribute>();
			this.Name = graphAttribute?.Name ?? typeof(T).GetName();

			var source = contextPropertyCache.Properties[nameof(IResolveFieldContext.Source)];
			propertyCache.Properties.Values
				.If(property => property.GetMethod != null && property.Attributes.First<Attribute, GraphAttribute>()?.Ignore != true)
				.Do(property => this.AddField(CreateFieldType(property, source)));
		}

		private static FieldType CreateFieldType(IPropertyMember property, IPropertyMember source)
		{
			var graphAttribute = property.Attributes.First<Attribute, GraphAttribute>();
			return new FieldType
			{
				Type = graphAttribute?.Type ?? property.GetGraphType(false),
				Name = graphAttribute?.Name ?? property.Name,
				Description = graphAttribute?.Description,
				DeprecationReason = property.Attributes.First<Attribute, ObsoleteAttribute>()?.Message,
				Resolver = new FieldResolver(context => source[context])
			};
		}
	}
}
