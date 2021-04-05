// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// <b>Graph QL</b><br />Mark an endpoint as a mutation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class GraphMutationAttribute : Attribute
	{
	}
}
