// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// <b>GraphQL</b><br />
	/// When using paging with the GraphQL Connection object, use this to mark the property of the model to use as the cursor value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
    public class GraphCursorAttribute : Attribute
    {
    }
}
