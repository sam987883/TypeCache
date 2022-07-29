// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
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

		EnumOf<T>.Tokens
			.If(token => !token.GraphQLIgnore())
			.Map(token => new EnumValueDefinition(token.GraphQLName(), token.Value)
			{
				Description = token.GraphQLDescription(),
				DeprecationReason = token.GraphQLDeprecationReason()
			})
			.Do(this.Add);
	}

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	protected override string ChangeEnumCase(string value)
		=> this._ChangeEnumCase(value);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	private static string DefaultChangeEnumCase(string value)
		=> value;
}
