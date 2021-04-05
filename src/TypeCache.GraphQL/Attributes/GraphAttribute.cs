// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// <b>Graph QL</b>
	/// <list type="number">
	/// <item><term>Name</term> <description>Sets the name of the enum field, object property, object type, endpoint or endpoint parameter.</description></item>
	/// <item><term>Description</term> <description>Sets the description of the enum field, object property, endpoint or endpoint parameter.</description></item>
	/// </list>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Struct)]
	public class GraphAttribute : Attribute
	{
		public GraphAttribute(string? name = null, string? description = null)
		{
			this.Name = name;
			this.Description = description;
		}

		public string? Name { get; }

		public string? Description { get; }
	}
}
