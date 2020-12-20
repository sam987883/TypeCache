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
            this.Name = typeof(T).GetName();
            propertyCache.Properties.Values
                .If(property => property.GetMethod != null && !property.Attributes.Any<Attribute, GraphIgnoreAttribute>())
                .Do(property => this.AddField(CreateFieldType(property)));
        }

        private static FieldType CreateFieldType(IPropertyMember property)
            => new FieldType
            {
                Type = property.GetGraphType(true),
                Name = property.Name,
                Description = property.Attributes.First<Attribute, GraphDescriptionAttribute>()?.Description,
                DeprecationReason = property.Attributes.First<Attribute, ObsoleteAttribute>()?.Message
            };
    }
}
