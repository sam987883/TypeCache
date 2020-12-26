// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// Mark endpoint as a GraphQL mutation endpoint.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class GraphMutationAttribute : Attribute
	{
	}
}
