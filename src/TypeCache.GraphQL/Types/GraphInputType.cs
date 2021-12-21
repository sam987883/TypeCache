// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphInputType<T> : InputObjectGraphType<T>
{
	public GraphInputType()
	{
		this.Name = TypeOf<T>.Member.GraphInputName();
		this.Description = TypeOf<T>.Member.GraphDescription();

		TypeOf<T>.Properties.Values
			.If(property => property.Getter is not null && property.Setter is not null && !property.GraphIgnore())
			.Do(property => this.AddField(property!.ToFieldType(true)));
	}
}
