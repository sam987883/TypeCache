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
		var typeName = TypeOf<T>.Member.GraphQLName();
		this.Name = Invariant($"{typeName}_OrderBy");
		this.Description = $"Order by `{typeName}`.";

		foreach (var property in TypeOf<T>.Properties.Values.If(property => !property.GraphQLIgnore()))
		{
			var propertyName = property.GraphQLName();
			var description = property.GraphQLDescription();
			var deprecationReason = property.ObsoleteMessage();

			this.AddValue(Invariant($"{propertyName}_ASC"), description, new OrderBy<T> { Expression = propertyName, Sort = Sort.Ascending }, deprecationReason);
			this.AddValue(Invariant($"{propertyName}_DESC"), description, new OrderBy<T> { Expression = propertyName, Sort = Sort.Descending }, deprecationReason);
		}
	}
}
