// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Extensions;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLEnumType<T> : EnumerationGraphType<T> where T : struct, Enum
{
	private readonly Func<string, string> _ChangeEnumCase = DefaultChangeEnumCase;

	public GraphQLEnumType()
	{
		this.Name = TypeOf<T>.Member.GraphQLName();

		EnumOf<T>.Tokens.Values.If(token => !token.GraphQLIgnore()).Do(token =>
		{
			var name = token.GraphQLName();
			var description = token.GraphQLDescription();
			var deprecationReason = token.ObsoleteMessage();

			this.AddValue(name, description, token.Value, deprecationReason);
		});
	}

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	protected override string ChangeEnumCase(string value)
		=> this._ChangeEnumCase(value);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	private static string DefaultChangeEnumCase(string value)
		=> value;
}
