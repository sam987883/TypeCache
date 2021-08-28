// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// <b>GraphQL</b><br/>
	/// Sets the name of the input type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class GraphInputNameAttribute : Attribute
	{
		/// <exception cref="ArgumentNullException"/>
		public GraphInputNameAttribute(string name)
		{
			name.AssertNotBlank(nameof(name));

			this.Name = name;
		}

		public string Name { get; }
	}
}
