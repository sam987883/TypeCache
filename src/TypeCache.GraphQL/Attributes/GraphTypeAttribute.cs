// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// <b>GraphQL</b>
	/// <list type="number">
	/// <item><term>graphType</term> <description>Overrides the Graph Type of the object property or endpoint parameter.</description></item>
	/// <item><term>scalarType, nullable</term> <description>Overrides only the Scalar Graph Type used by the generated Graph Type of the object property or endpoint parameter.</description></item>
	/// </list>
	/// If the parameter a type of <see cref="IResolveFieldContext"/> or <see cref="IResolveFieldContext&lt;Object&gt;"/>, then it will not show up in the endpoint-<br />
	/// Instead it will be injected with the instance of <see cref="IResolveFieldContext"/> or <see cref="IResolveFieldContext&lt;Object&gt;"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
	public class GraphTypeAttribute : Attribute
	{
		public GraphTypeAttribute(Type graphType)
		{
			graphType.AssertNotNull(nameof(graphType));
			graphType.IsGraphType().Assert(nameof(graphType), true);

			this.GraphType = graphType;
		}

		public GraphTypeAttribute(ScalarType scalar)
		{
			this.GraphType = scalar.GraphType();
		}

		public GraphTypeAttribute(ListType list, ScalarType scalar)
		{
			var scalarType = scalar.GraphType();
			this.GraphType = list switch
			{
				ListType.List => typeof(ListGraphType<>).MakeGenericType(scalarType),
				ListType.NonNullList => typeof(NonNullGraphType<>).MakeGenericType(typeof(ListGraphType<>)).MakeGenericType(scalarType),
				_ => scalarType
			};
		}

		/// <summary>
		/// The Graph Type for override.
		/// </summary>
		public Type GraphType { get; }
	}
}
