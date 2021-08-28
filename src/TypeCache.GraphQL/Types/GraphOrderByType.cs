// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SQL;

namespace TypeCache.GraphQL.Types
{
	public class GraphOrderByType<T> : EnumerationGraphType
		where T : class
	{
		public GraphOrderByType()
		{
			this.Name = TypeOf<T>.Attributes.GraphName() ?? $"{TypeOf<T>.Name}_OrderBy";
			this.Description = $"Order by `{TypeOf<T>.Name}`.";

			foreach (var property in TypeOf<T>.Properties.Values.If(property => !property.GraphIgnore()))
			{
				var propertyName = property.GraphName();
				var description = property.GraphDescription();
				var deprecationReason = property.ObsoleteMessage();

				this.AddValue($"{propertyName}_ASC", description, new OrderBy<T> { Expression = propertyName, Sort = Sort.Ascending }, deprecationReason);
				this.AddValue($"{propertyName}_DESC", description, new OrderBy<T> { Expression = propertyName, Sort = Sort.Descending }, deprecationReason);
			}
		}
	}
}
