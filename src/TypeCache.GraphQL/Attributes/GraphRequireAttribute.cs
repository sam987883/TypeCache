// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class GraphRequireAttribute : Attribute
    {
    }
}
