// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Reflection;
using static System.FormattableString;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLEnumType<T> : EnumerationGraphType
	where T : struct, Enum
{
	public GraphQLEnumType()
	{
		this.Name = EnumOf<T>.Attributes.First<GraphQLNameAttribute>()?.Name ?? EnumOf<T>.Name;
		this.Description = EnumOf<T>.Attributes.First<GraphQLDescriptionAttribute>()?.Description;
		this.DeprecationReason = EnumOf<T>.Attributes.First<GraphQLDeprecationReasonAttribute>()?.DeprecationReason;

		var changeEnumCase = new Func<string, string>(_ => _);
		if (EnumOf<T>.Attributes.IfFirst<ConstantCaseAttribute>(out var constantCaseAttribute))
			changeEnumCase = constantCaseAttribute.ChangeEnumCase;
		else if (EnumOf<T>.Attributes.IfFirst<CamelCaseAttribute>(out var camelCaseAttribute))
			changeEnumCase = camelCaseAttribute.ChangeEnumCase;
		else if (EnumOf<T>.Attributes.IfFirst<PascalCaseAttribute>(out var pascalCaseAttribute))
			changeEnumCase = pascalCaseAttribute.ChangeEnumCase;

		EnumOf<T>.Tokens
			.If(token => !token.GraphQLIgnore())
			.Map(token => new EnumValueDefinition(token.Attributes.First<GraphQLNameAttribute>()?.Name ?? changeEnumCase(token.Name), token.Value)
			{
				Description = token.GraphQLDescription(),
				DeprecationReason = token.GraphQLDeprecationReason()
			})
			.Do(this.Add);
	}

	/// <inheritdoc/>
	public override bool CanParseValue(object? value)
		=> value switch
		{
			null or T => true,
			Enum token => token is T,
			_ => Enum.IsDefined(typeof(T), value)
		};

	/// <inheritdoc/>
	public override object? ParseValue(object? value)
		=> value switch
		{
			null => null,
			Enum token => token,
			sbyte number => Enum.ToObject(typeof(T), number),
			byte number => Enum.ToObject(typeof(T), number),
			short number => Enum.ToObject(typeof(T), number),
			ushort number => Enum.ToObject(typeof(T), number),
			int number => Enum.ToObject(typeof(T), number),
			uint number => Enum.ToObject(typeof(T), number),
			long number => Enum.ToObject(typeof(T), number),
			ulong number => Enum.ToObject(typeof(T), number),
			_ => Enum.ToObject(typeof(T), value)
		};

	/// <inheritdoc/>
	public override object? Serialize(object? value)
		=> this.ParseValue(value) switch
		{
			T token => token.ToString("G"),
			_ => null
		};
}
