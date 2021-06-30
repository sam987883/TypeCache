// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphObjectEnumType<T> : EnumerationGraphType where T : class
	{
		public GraphObjectEnumType()
		{
			var graphName = TypeOf<T>.Attributes.GraphName();
			this.Name = graphName ?? $"{TypeOf<T>.Name}_Fields";
			this.Description = $"Fields of type `{graphName ?? TypeOf<T>.Name}`.";

			foreach (var property in TypeOf<T>.Properties.Values.If(property => !property.Attributes.GraphIgnore()))
			{
				var name = property.Attributes.GraphName() ?? property.Name;
				var description = property.Attributes.GraphDescription();
				var deprecationReason = property.Attributes.ObsoleteMessage();

				this.AddValue(name, description, name, deprecationReason);
			}
		}
	}
}
