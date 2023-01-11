﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// <inheritdoc cref="EnumerationGraphType"/>
/// </summary>
public sealed class GraphQLEnumType<T> : EnumerationGraphType
	where T : struct, Enum
{
	public GraphQLEnumType()
	{
		this.Name = typeof(T).GraphQLName();
		this.Description = typeof(T).GraphQLDescription();
		this.DeprecationReason = typeof(T).GraphQLDeprecationReason();

		var changeEnumCase = EnumOf<T>.Attributes switch
		{
			_ when EnumOf<T>.Attributes.OfType<ConstantCaseAttribute>().TryFirst(out var attribute) => attribute.ChangeEnumCase,
			_ when EnumOf<T>.Attributes.OfType<CamelCaseAttribute>().TryFirst(out var attribute) => attribute.ChangeEnumCase,
			_ when EnumOf<T>.Attributes.OfType<PascalCaseAttribute>().TryFirst(out var attribute) => attribute.ChangeEnumCase,
			_ => new Func<string, string>(_ => _)
		};

		EnumOf<T>.Tokens
			.Where(token => !token.Attributes.Any<GraphQLIgnoreAttribute>())
			.Select(token => new EnumValueDefinition(token.Attributes.FirstOrDefault<GraphQLNameAttribute>()?.Name ?? changeEnumCase(token.Name), token.Value)
			{
				Description = token.Attributes.FirstOrDefault<GraphQLDescriptionAttribute>()?.Description,
				DeprecationReason = token.Attributes.FirstOrDefault<GraphQLDeprecationReasonAttribute>()?.DeprecationReason
			})
			.ToArray()
			.ForEach(this.Add);
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
		=> value is not null ? Enum.ToObject(typeof(T), value) : null;

	/// <inheritdoc/>
	public override object? Serialize(object? value)
		=> this.ParseValue(value);
}
