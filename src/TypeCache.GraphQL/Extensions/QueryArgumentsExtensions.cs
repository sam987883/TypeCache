// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static class QueryArgumentsExtensions
{
	extension(QueryArguments @this)
	{
		public void Add<T>(string name, bool? nullable = null, object? defaultValue = null, string? description = null)
		{
			if (typeof(T).Implements(typeof(IGraphType)))
			{
				@this.Add(new(typeof(T))
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

			var scalarType = type.ScalarType;
			var graphType = type switch
			{
				{ IsEnum: true } => typeof(EnumGraphType<>),
				_ when scalarType is not ScalarType.None => scalarType.ToGraphType(),
				_ => typeof(InputGraphType<>)
			};

			type = graphType!.IsGenericType ? graphType.MakeGenericType(type) : graphType;

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

			@this.Add(new(type)
			{
				Name = name,
				DefaultValue = defaultValue,
				Description = description
			});
		}

		public void Add(string name, Type type, object? defaultValue = null, string? description = null)
			=> @this.Add(new(type)
			{
				Name = name,
				DefaultValue = defaultValue,
				Description = description
			});

		public void Add(string name, IGraphType resolvedType, object? defaultValue = null, string? description = null)
			=> @this.Add(new(resolvedType)
			{
				Name = name,
				DefaultValue = defaultValue,
				Description = description
			});
	}
}
