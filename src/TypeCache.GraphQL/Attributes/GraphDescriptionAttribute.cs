﻿// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// <b>GraphQL</b><br/>
	/// Sets the description of the object type, object property, enum type, enum field, endpoint or endpoint parameter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Struct)]
	public class GraphDescriptionAttribute : Attribute
	{
		public GraphDescriptionAttribute(string description)
		{
			description.AssertNotBlank(nameof(description));

			this.Description = description;
		}

		public string Description { get; }
	}
}
