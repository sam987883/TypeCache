// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b>
/// <list type="number">
/// <item><term>graphType</term> Overrides the Graph Type of the object property or endpoint parameter.</item>
/// <item><term>scalarType</term> Overrides the Graph Type of the object property or endpoint parameter using a scalar type.</item>
/// <item><term>listType, scalarType</term> Overrides the Graph Type of the object property or endpoint parameter using a list of scalar type.</item>
/// </list>
/// If the parameter a type of <see cref="IResolveFieldContext"/> or <see cref="IResolveFieldContext{TSource}"/>, then it will not show up in the endpoint-<br />
/// Instead it will be injected with the instance of <see cref="IResolveFieldContext"/> or <see cref="IResolveFieldContext{TSource}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = false)]
public class GraphQLTypeAttribute : Attribute
{
	/// <summary>
	/// Overrides the Graph Type of the object property or endpoint parameter.
	/// </summary>
	public GraphQLTypeAttribute(Type graphType)
	{
		graphType.AssertNotNull();
		graphType.IsGraphType().Assert(true);

		this.GraphType = graphType;
	}

	/// <summary>
	/// Overrides the Graph Type of the object property or endpoint parameter using a scalar type.
	/// </summary>
	public GraphQLTypeAttribute(ScalarType scalarType)
		=> this.GraphType = scalarType.GraphType();

	/// <summary>
	/// Overrides the Graph Type of the object property or endpoint parameter using a list of scalar type.
	/// </summary>
	public GraphQLTypeAttribute(ListType listType, ScalarType scalarType)
		=> this.GraphType = listType switch
		{
			ListType.List => typeof(ListGraphType<>).MakeGenericType(scalarType.GraphType()),
			ListType.NotNullList => typeof(NonNullGraphType<>).MakeGenericType(typeof(ListGraphType<>)).MakeGenericType(scalarType.GraphType()),
			_ => scalarType.GraphType()
		};

	/// <summary>
	/// The Graph Type for override.
	/// </summary>
	public Type GraphType { get; }

	public static GraphQLTypeAttribute Int => new GraphQLTypeAttribute(typeof(IntGraphType));
}
