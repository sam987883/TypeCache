// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using System;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter)]
    public class GraphInjectAttribute : Attribute
    {
        public GraphInjectAttribute(Func<IResolveFieldContext, object> resolver)
		{
            resolver.AssertNotNull($"{nameof(GraphInjectAttribute)}.{nameof(this.Resolver)}");
            this.Resolver = resolver;
        }

        public Func<IResolveFieldContext, object> Resolver { get; }
    }
}
