// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLObjectEnumType<T> : EnumerationGraphType where T : class
{
	public GraphQLObjectEnumType()
	{
		var graphName = TypeOf<T>.Member.GraphQLName();
		this.Name = Invariant($"{graphName}Fields");
		this.Description = Invariant($"Fields of type `{graphName}`.");

		TypeOf<T>.Properties
			.If(property => !property.GraphQLIgnore())
			.Map(property => new EnumValueDefinition(property.GraphQLName(), property.GraphQLName())
			{
				Description = property.GraphQLDescription(),
				DeprecationReason = property.GraphQLDeprecationReason()
			})
			.Do(this.Add);
	}
}
