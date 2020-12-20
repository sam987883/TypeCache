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
            this.Name = typeof(T).GetName();
            var source = contextPropertyCache.Properties[nameof(IResolveFieldContext<T>.Source)];
            propertyCache.Properties.Values
                .If(property => property.GetMethod != null && !property.Attributes.Any<Attribute, GraphIgnoreAttribute>())
                .Do(property => this.AddField(CreateFieldType(property, source)));
        }

        private static FieldType CreateFieldType(IPropertyMember property, IPropertyMember source)
            => new FieldType
            {
                Type = property.GetGraphType(false),
                Name = property.Name,
                Description = property.Attributes.First<Attribute, GraphDescriptionAttribute>()?.Description,
                DeprecationReason = property.Attributes.First<Attribute, ObsoleteAttribute>()?.Message,
                Resolver = new FieldResolver(context => source[context])
            };
    }
}
