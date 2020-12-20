// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using System;

namespace TypeCache.GraphQL.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class GraphTypeAttribute : Attribute
    {
        public GraphTypeAttribute(Type type)
        {
            this.Type = type.IsGraphType() ? type : throw new NotSupportedException($"{nameof(GraphTypeAttribute)}: Type [{type.Name}] is not a graph type.");
		}

        public Type Type { get; }
    }
}
