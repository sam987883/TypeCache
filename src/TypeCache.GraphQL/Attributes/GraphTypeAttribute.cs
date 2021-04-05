// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// <b>Graph QL</b><br />
	/// Overrides the Graph Type of the object property or endpoint parameter.<br />
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

		public GraphTypeAttribute(ScalarType scalarType)
		{
			this.ScalarType = scalarType;
		}

		/// <summary>
		/// The Graph Type for override.
		/// </summary>
		public Type? GraphType { get; }

		/// <summary>
		/// The Graph Scalar Type to use.
		/// </summary>
		public ScalarType? ScalarType { get; }
	}
}
