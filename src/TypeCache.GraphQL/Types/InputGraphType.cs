// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Types;

public sealed class InputGraphType<T> : InputObjectGraphType<T>
	where T : notnull
{
	public InputGraphType()
	{
		this.Name = Type<T>.Attributes.GraphQLInputName() ?? Invariant($"{Type<T>.Name}Input");
		this.Description = Type<T>.Attributes.GraphQLDescription();
		this.DeprecationReason = Type<T>.Attributes.GraphQLDeprecationReason();

		Type<T>.Properties.Values
			.Where(_ => _.CanRead && _.CanWrite && !_.Attributes.GraphQLIgnore())
			.ForEach(_ => this.AddField(new()
			{
				Type = _.ToGraphQLType(true),
				Name = _.Attributes.GraphQLName() ?? _.Name,
				Description = _.Attributes.GraphQLDescription(),
				DeprecationReason = _.Attributes.GraphQLDeprecationReason()
			}));
	}
}
