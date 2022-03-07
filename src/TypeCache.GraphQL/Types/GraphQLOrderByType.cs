// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SQL;
using static System.FormattableString;

namespace TypeCache.GraphQL.Types;

public class GraphQLOrderByType<T> : EnumerationGraphType
	where T : class
{
	public GraphQLOrderByType()
	{
		this.Name = TypeOf<T>.Attributes.GraphName() ?? $"{TypeOf<T>.Name}_OrderBy";
		this.Description = $"Order by `{TypeOf<T>.Name}`.";

		foreach (var property in TypeOf<T>.Properties.Values.If(property => !property.GraphIgnore()))
		{
			var propertyName = property.GraphName();
			var description = property.GraphDescription();
			var deprecationReason = property.ObsoleteMessage();

			this.AddValue(Invariant($"{propertyName}_ASC"), description, new OrderBy<T> { Expression = propertyName, Sort = Sort.Ascending }, deprecationReason);
			this.AddValue(Invariant($"{propertyName}_DESC"), description, new OrderBy<T> { Expression = propertyName, Sort = Sort.Descending }, deprecationReason);
		}
	}
}
