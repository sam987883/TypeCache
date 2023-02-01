// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL;

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b>
/// If the parameter a type of <see cref="IResolveFieldContext"/> or <see cref="IResolveFieldContext{TSource}"/>, then it will not show up in the endpoint-<br />
/// Instead it will be injected with the instance of <see cref="IResolveFieldContext"/> or <see cref="IResolveFieldContext{TSource}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = false)]
public sealed class GraphQLTypeAttribute : Attribute
{
	public GraphQLTypeAttribute(Type graphQLType)
	{
		this.GraphQLType = graphQLType;
	}

	public Type GraphQLType { get; }
}
