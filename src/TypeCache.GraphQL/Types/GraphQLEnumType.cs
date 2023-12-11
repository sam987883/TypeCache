// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Utilities;

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

		var changeEnumCase = Enum<T>.Attributes switch
		{
			_ when Enum<T>.Attributes.TryFirst<ConstantCaseAttribute>(out var attribute) => attribute.ChangeEnumCase,
			_ when Enum<T>.Attributes.TryFirst<CamelCaseAttribute>(out var attribute) => attribute.ChangeEnumCase,
			_ when Enum<T>.Attributes.TryFirst<PascalCaseAttribute>(out var attribute) => attribute.ChangeEnumCase,
			_ => new Func<string, string>(_ => _)
		};

		Enum<T>.Values
			.Where(_ => !_.Attributes().Any<GraphQLIgnoreAttribute>())
			.Select(_ => new EnumValueDefinition(_.Attributes().FirstOrDefault<GraphQLNameAttribute>()?.Name ?? changeEnumCase(_.Name()), _)
			{
				Description = _.Attributes().FirstOrDefault<GraphQLDescriptionAttribute>()?.Description,
				DeprecationReason = _.Attributes().FirstOrDefault<GraphQLDeprecationReasonAttribute>()?.DeprecationReason
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
			_ => false
		};

	/// <inheritdoc/>
	public override object? ParseValue(object? value)
		=> value is not null ? Enum.ToObject(typeof(T), value) : null;

	/// <inheritdoc/>
	public override object? Serialize(object? value)
		=> this.ParseValue(value);
}
