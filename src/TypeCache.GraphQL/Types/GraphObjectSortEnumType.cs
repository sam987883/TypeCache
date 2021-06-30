// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SQL;

namespace TypeCache.GraphQL.Types
{
	public class GraphObjectSortEnumType<T> : EnumerationGraphType
		where T : class
	{
		public GraphObjectSortEnumType()
		{
			var typeName = TypeOf<T>.Attributes.GraphName() ?? TypeOf<T>.Name;
			this.Name = $"{typeName}_Sort";
			this.Description = $"Order by `{typeName}`.";

			foreach (var property in TypeOf<T>.Properties.Values.If(property => !property.Attributes.GraphIgnore()))
			{
				var name = property.Attributes.GraphName() ?? property.Name;
				var description = property.Attributes.GraphDescription();
				var deprecationReason = property.Attributes.ObsoleteMessage();

				this.AddValue($"{name}_ASC", description, new OrderBy<T> { Expression = name, Sort = Sort.Ascending }, deprecationReason);
				this.AddValue($"{name}_DESC", description, new OrderBy<T> { Expression = name, Sort = Sort.Descending }, deprecationReason);
			}
		}
	}
}
