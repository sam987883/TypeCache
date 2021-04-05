// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// <b>Graph QL</b><br />Mark an endpoint as a query.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class GraphQueryAttribute : Attribute
	{
	}
}
