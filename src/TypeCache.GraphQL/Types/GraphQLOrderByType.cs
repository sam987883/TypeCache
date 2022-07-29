// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SQL;
using static System.FormattableString;

namespace TypeCache.GraphQL.Types;

public class GraphQLOrderByType<T> : EnumerationGraphType
{
	public GraphQLOrderByType()
	{
		var graphName = TypeOf<T>.Member.GraphQLName();
		this.Name = Invariant($"{graphName}OrderBy");
		this.Description = $"Order by `{graphName}`.";

		foreach (var property in TypeOf<T>.Properties.If(_ => !_.GraphQLIgnore()))
		{
			var propertyName = property.GraphQLName();
			var description = property.GraphQLDescription();
			var deprecationReason = property.GraphQLDeprecationReason();
			this.Add(new EnumValueDefinition(Invariant($"{propertyName}_ASC"), new OrderBy<T> { Expression = propertyName, Sort = Sort.Ascending })
			{
				Description = description,
				DeprecationReason = deprecationReason
			});
			this.Add(new EnumValueDefinition(Invariant($"{propertyName}_DESC"), new OrderBy<T> { Expression = propertyName, Sort = Sort.Descending })
			{
				Description = description,
				DeprecationReason = deprecationReason
			});
		}
	}
}
