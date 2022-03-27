// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLInputType<T> : InputObjectGraphType<T>
{
	public GraphQLInputType()
	{
		this.Name = TypeOf<T>.Member.GraphQLInputName();
		this.Description = TypeOf<T>.Member.GraphQLDescription();

		TypeOf<T>.Properties.Values
			.If(property => property.Getter is not null && property.Setter is not null && !property.GraphQLIgnore())
			.Do(property => this.AddField(property!.ToInputFieldType()));
	}
}
