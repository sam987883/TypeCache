// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLEnumType<T> : EnumerationGraphType
	where T : struct, Enum
{
	public GraphQLEnumType()
	{
		this.Name = EnumOf<T>.Attributes.FirstOrDefault<GraphQLNameAttribute>()?.Name ?? EnumOf<T>.Name;
		this.Description = EnumOf<T>.Attributes.FirstOrDefault<GraphQLDescriptionAttribute>()?.Description;
		this.DeprecationReason = EnumOf<T>.Attributes.FirstOrDefault<GraphQLDeprecationReasonAttribute>()?.DeprecationReason;

		var changeEnumCase = EnumOf<T>.Attributes switch
		{
			var attributes when attributes.OfType<ConstantCaseAttribute>().TryFirst(out var attribute) => attribute.ChangeEnumCase,
			var attributes when attributes.OfType<CamelCaseAttribute>().TryFirst(out var attribute) => attribute.ChangeEnumCase,
			var attributes when attributes.OfType<PascalCaseAttribute>().TryFirst(out var attribute) => attribute.ChangeEnumCase,
			_ => new Func<string, string>(_ => _)
		};

		EnumOf<T>.Tokens
			.Where(token => !token.GraphQLIgnore())
			.Select(token => new EnumValueDefinition(token.Attributes.FirstOrDefault<GraphQLNameAttribute>()?.Name ?? changeEnumCase(token.Name), token.Value)
			{
				Description = token.GraphQLDescription(),
				DeprecationReason = token.GraphQLDeprecationReason()
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
