// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Mappers.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLInputType<T> : GraphQLComplexType, IInputObjectGraphType
{
	public GraphQLInputType()
		: base(TypeOf<T>.Member.GraphQLInputName())
	{
		this.Description = TypeOf<T>.Member.GraphQLDescription();
		this.DeprecationReason = TypeOf<T>.Member.GraphQLDeprecationReason();

		TypeOf<T>.Properties.Values
			.If(property => property.Getter is not null && property.Setter is not null && !property.GraphQLIgnore())
			.Do(property => this.AddField(property!.ToInputFieldType()));
	}

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool IsValidDefault(object value)
		=> value is T;

	public object ParseDictionary(IDictionary<string, object?> value)
	{
		if (value is null)
			return null!;

		if (typeof(T) == typeof(object))
			return value;

		var item = TypeOf<T>.Create()!;
		item.MapProperties(value);
		return item;
	}

	public GraphQLValue ToAST(object value)
	{
		var output = new DefaultInterpolatedStringHandler(79, 2);
		output.AppendLiteral("Please override the '");
		output.AppendFormatted(nameof(ToAST));
		output.AppendLiteral("' method of the '");
		output.AppendFormatted(this.Name);
		output.AppendLiteral("' Input Object to support this operation.");
		throw new NotImplementedException(output.ToStringAndClear());
	}
}
