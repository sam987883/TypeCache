// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class GraphDescriptionAttribute : Attribute
    {
        public GraphDescriptionAttribute(string description) => this.Description = description;

        public string Description { get; }
    }
}
