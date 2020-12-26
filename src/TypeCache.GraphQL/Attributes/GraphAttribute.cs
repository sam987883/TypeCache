// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using System;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// GraphQL overrides:
	/// <list type="number">
	/// <item><term>name</term> <description>Sets the name of the enum field, object property, endpoint or endpoint parameter.</description></item>
	/// <item><term>description</term> <description>Sets the description of the enum field, object property, endpoint or endpoint parameter.</description></item>
	/// <item><term>type</term> <description>Sets the GraphType of the object property or endpoint parameter.</description></item>
	/// <item><term>ignore</term> <description>When set to true the object property or endpoint parameter will not be included in the GraphType or endpoint.</description></item>
	/// <item><term>inject</term> <description>Inject the IResolveFieldContext instance into the endpoint parameter.</description></item>
	/// </list>
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Method)]
	public class GraphAttribute : Attribute
	{
		public GraphAttribute(string name = null, string description = null, Type type = null, bool ignore = false, bool inject = false)
		{
			type?.IsGraphType().Assert($"{nameof(GraphAttribute)}: Type is not a graph type.", true);
			this.Name = name;
			this.Description = description;
			this.Type = type;
			this.Ignore = ignore;
			this.Inject = inject;
		}

		public string Name { get; }

		public string Description { get; }

		public Type Type { get; }

		public bool Ignore { get; }

		public bool Inject { get; }
	}
}
