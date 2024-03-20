// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions;

public static class QueryArgumentsExtensions
{
	public static void Add<T>(this QueryArguments @this, string name, bool? nullable = null, object? defaultValue = null, string? description = null)
	{
		if (typeof(T).Implements(typeof(IGraphType)))
		{
			@this.Add(new QueryArgument(typeof(T))
			{
				Name = name,
				DefaultValue = defaultValue,
				Description = description
			});
			return;
		}

		var isList = typeof(T).IsArray || typeof(T).Implements(typeof(IEnumerable<>).MakeGenericType(typeof(T)));
		var type = typeof(T) switch
		{
			{ IsArray: true } => typeof(T).GetElementType()!,
			_ when isList => typeof(T).GenericTypeArguments[0],
			_ => typeof(T)
		};

		var isValueNullable = type.Is(typeof(Nullable<>));
		if (isValueNullable)
			type = type.GenericTypeArguments[0];

		var graphType = type switch
		{
			{ IsEnum: true } => typeof(GraphQLEnumType<>),
			_ when type.Implements(typeof(ISpanParsable<>)) => typeof(GraphQLScalarType<>),
			_ => typeof(GraphQLInputType<>)
		};

		type = graphType.MakeGenericType(type);

		if (isList)
		{
			if (!isValueNullable)
				type = type.ToNonNullGraphType();

			type = type.ToListGraphType();
			nullable ??= defaultValue is not null;

			if (nullable is false)
				type = type.ToNonNullGraphType();
		}
		else
		{
			nullable ??= isValueNullable || defaultValue is not null;
			if (nullable is false)
				type = type.ToNonNullGraphType();
		}

		@this.Add(new QueryArgument(type)
		{
			Name = name,
			DefaultValue = defaultValue,
			Description = description
		});
	}

	public static void Add(this QueryArguments @this, string name, Type type, object? defaultValue = null, string? description = null)
		=> @this.Add(new(type)
		{
			Name = name,
			DefaultValue = defaultValue,
			Description = description
		});

	public static void Add(this QueryArguments @this, string name, IGraphType resolvedType, object? defaultValue = null, string? description = null)
		=> @this.Add(new(resolvedType)
		{
			Name = name,
			DefaultValue = defaultValue,
			Description = description
		});
}
