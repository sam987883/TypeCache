// Copyright (c) 2021 Samuel Abraham

using global::GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Types;

/// <inheritdoc cref="EnumerationGraphType"/>
public sealed class EnumGraphType<T> : EnumerationGraphType
	where T : struct, Enum
{
	public EnumGraphType() : base()
	{
		this.Name = Enum<T>.Attributes.GraphQLName ?? Enum<T>.Name;
		this.Description = Enum<T>.Attributes.GraphQLDescription;
		this.DeprecationReason = Enum<T>.Attributes.GraphQLDeprecationReason;

		Func<string, string>? changeEnumCase = Enum<T>.Attributes switch
		{
			_ when Enum<T>.Attributes.TryFirst<ConstantCaseAttribute>(out var attribute) => attribute.ChangeEnumCase,
			_ when Enum<T>.Attributes.TryFirst<CamelCaseAttribute>(out var attribute) => attribute.ChangeEnumCase,
			_ when Enum<T>.Attributes.TryFirst<PascalCaseAttribute>(out var attribute) => attribute.ChangeEnumCase,
			_ => null
		};

		Enum<T>.Values
			.Where(_ => !_.Attributes.GraphQLIgnore)
			.Select(_ => new EnumValueDefinition(_.Attributes.GraphQLName ?? changeEnumCase?.Invoke(_.Name) ?? _.Name, _)
			{
				Description = _.Attributes.GraphQLDescription,
				DeprecationReason = _.Attributes.GraphQLDeprecationReason
			})
			.ForEach(this.Add);
	}

	/// <inheritdoc/>
	public override object? Serialize(object? value)
		=> value is not null ? (T)value : null;

	/// <inheritdoc/>
	public override bool CanParseValue(object? value)
		=> value switch
		{
			null => true,
			T token => Enum<T>.IsDefined(token),
			string text => Enum.TryParse<T>(text, true, out _),
			_ when value.GetType() == typeof(T).GetEnumUnderlyingType() => true,
			_ => false
		};

	/// <inheritdoc/>
	public override object? ParseValue(object? value)
		=> value switch
		{
			null => null,
			T token when Enum<T>.IsDefined(token) => token,
			T token => null,
			string text => Enum.Parse<T>(text, true),
			_ => Enum.ToObject(typeof(T), value)
		};
}
