// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLObjectType<T> : ObjectGraphType<T>
{
	public GraphQLObjectType()
	{
		this.Name = TypeOf<T>.Member.GraphQLName();
		this.Description = TypeOf<T>.Member.GraphQLDescription();
		this.DeprecationReason = TypeOf<T>.Member.ObsoleteMessage();

		TypeOf<T>.InterfaceTypes
			.If(type => type.ElementType is null && !type.GenericHandle.HasValue)
			.Do(type => this.Interface(type));

		TypeOf<T>.Properties.Values
			.If(property => property.Getter is not null && !property.GraphQLIgnore())
			.Do(property => this.AddField(property.ToFieldType()));
	}
}
