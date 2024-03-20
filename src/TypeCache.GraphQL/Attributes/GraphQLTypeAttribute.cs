// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.Types;

namespace TypeCache.GraphQL.Attributes;

public abstract class GraphQLTypeAttribute : Attribute
{
}

/// <summary>
/// <b>GraphQL</b>
/// If the parameter a type of <see cref="IResolveFieldContext"/> or <see cref="IResolveFieldContext{TSource}"/>, then it will not show up in the endpoint-<br />
/// Instead it will be injected with the instance of <see cref="IResolveFieldContext"/> or <see cref="IResolveFieldContext{TSource}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = false)]
public sealed class GraphQLTypeAttribute<T> : GraphQLTypeAttribute
	where T : IGraphType
{
}
