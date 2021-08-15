// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.SQL
{
	public class GraphOrderByType<T> : EnumerationGraphType
		where T : class
	{
		public GraphOrderByType()
		{
			var typeName = TypeOf<T>.Attributes.GraphName() ?? TypeOf<T>.Name;
			this.Name = $"{typeName}_OrderBy";
			this.Description = $"Order by `{typeName}`.";

			foreach (var property in TypeOf<T>.Properties.Values.If(property => !property.Attributes.GraphIgnore()))
			{
				var propertyName = property.Attributes.GraphName() ?? property.Name;
				var description = property.Attributes.GraphDescription();
				var deprecationReason = property.Attributes.ObsoleteMessage();

				this.AddValue($"{propertyName}_ASC", description, new OrderBy<T> { Expression = propertyName, Sort = Sort.Ascending }, deprecationReason);
				this.AddValue($"{propertyName}_DESC", description, new OrderBy<T> { Expression = propertyName, Sort = Sort.Descending }, deprecationReason);
			}
		}
	}
}
