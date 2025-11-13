// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLExtensions
{
	extension(IComplexGraphType @this)
	{
		public FieldType AddField<T>(string name, IFieldResolver resolver)
			=> @this.AddField(new()
			{
				Name = name,
				Type = typeof(T).ToGraphQLType(false),
				Resolver = resolver
			});

		public FieldType AddField(string name, IGraphType resolvedType, IFieldResolver resolver)
			=> @this.AddField(new()
			{
				Name = name,
				ResolvedType = resolvedType,
				Resolver = resolver
			});

		public FieldType AddField(MethodEntity method)
			=> @this.AddField(method, new MethodFieldResolver(method));

		public FieldType AddField(MethodEntity method, IFieldResolver resolver)
		{
			var fieldType = CreateFieldType(method, method.Return.ParameterType);
			fieldType.Resolver = resolver;
			return fieldType;
		}

		public FieldType AddField(StaticMethodEntity method)
			=> @this.AddField(method, new StaticMethodFieldResolver(method));

		public FieldType AddField(StaticMethodEntity method, IFieldResolver resolver)
		{
			var fieldType = CreateFieldType(method, method.Return.ParameterType);
			fieldType.Resolver = resolver;
			return fieldType;
		}

		public FieldType AddFieldStream(MethodEntity method)
			=> @this.AddFieldStream(method, new MethodSourceStreamResolver(method));

		public FieldType AddFieldStream(MethodEntity method, ISourceStreamResolver resolver)
		{
			var fieldType = CreateFieldType(method, method.Return.ParameterType);
			fieldType.StreamResolver = resolver;
			return fieldType;
		}

		public FieldType AddFieldStream(StaticMethodEntity method)
			=> @this.AddFieldStream(method, new StaticMethodSourceStreamResolver(method));

		public FieldType AddFieldStream(StaticMethodEntity method, ISourceStreamResolver resolver)
		{
			var fieldType = CreateFieldType(method, method.Return.ParameterType);
			fieldType.StreamResolver = resolver;
			return fieldType;
		}
	}

	extension(EnumerationGraphType @this)
	{
		public void AddOrderBy(string column, string? deprecationReason = null)
		{
			var asc = Sort.Ascending.ToSQL();
			var desc = Sort.Descending.ToSQL();

			@this.Add(Invariant($"{column}_{asc}"), Invariant($"{column} {asc}"), Invariant($"{column} {asc}"), deprecationReason);
			@this.Add(Invariant($"{column}_{desc}"), Invariant($"{column} {desc}"), Invariant($"{column} {desc}"), deprecationReason);
		}
	}

	extension<T>(IEnumerable<T> @this) where T : notnull
	{
		/// <summary>
		/// Use this to create a GraphQL Connection object to return in your endpoint to support paging.
		/// </summary>
		/// <param name="offset">The number of records to skip.</param>
		/// <param name="totalCount">The total record count of the record set being paged.</param>
		/// <returns>The <see cref="Connection{T}"/>.</returns>
		public Connection<T> ToConnection(uint offset, int totalCount)
		{
			var items = @this.AsArray();
			return new(offset, items)
			{
				PageInfo = new(offset, offset + (uint)items.Length, totalCount),
				TotalCount = totalCount
			};
		}
	}

	extension(Type @this)
	{
		public string GraphQLName
			=> @this.DeclaredAttributes.GraphQLName switch
			{
				null when !@this.IsGenericType => @this.BaseName,
				null => string.Join('_', [@this.BaseName, .. @this.GenericTypeArguments.Select(_ => _.GraphQLName)]),
				var name => name,
			};

		public Type ToGraphQLType(bool isInputType)
		{
			if (@this.Is(typeof(Nullable<>)))
				return @this.GenericTypeArguments[0].ToGraphQLType(isInputType);

			if (@this.IsEnum)
				return typeof(EnumGraphType<>).MakeGenericType(@this);

			var objectType = @this.ObjectType;
			(objectType is ObjectType.Delegate).ThrowIfTrue();
			(objectType is ObjectType.Object).ThrowIfTrue();

			var scalarGraphType = @this.ScalarType.ToGraphType();
			if (scalarGraphType is not null)
				return scalarGraphType;

			var collectionType = @this.CollectionType;
			if (collectionType.IsDictionary)
				return typeof(KeyValuePair<,>).MakeGenericType(@this.GenericTypeArguments).ToGraphQLType(isInputType).ToNonNullGraphType().ToListGraphType();

			if (objectType is ObjectType.Task || objectType is ObjectType.ValueTask)
				return @this.IsGenericType
					? @this.GenericTypeArguments.First()!.ToGraphQLType(false)
					: throw new ArgumentOutOfRangeException(nameof(@this), Invariant($"{nameof(Task)} and {nameof(ValueTask)} are not allowed as GraphQL types."));

			if (@this.HasElementType)
			{
				var elementType = @this.GetElementType()!.ToGraphQLType(isInputType);
				if (!elementType.IsNullable)
					elementType = elementType.ToNonNullGraphType();

				return elementType.ToListGraphType();
			}

			if (@this.Is(typeof(IEnumerable<>)) || @this.Implements(typeof(IEnumerable<>)))
				return @this.GenericTypeArguments.First()!.ToGraphQLType(isInputType).ToListGraphType();

			if (@this.IsInterface)
				return typeof(Types.InterfaceGraphType<>).MakeGenericType(@this);

			var graphType = isInputType ? typeof(InputGraphType<>) : typeof(OutputGraphType<>);

			return graphType.MakeGenericType(@this);
		}

		/// <summary>
		/// <c>=&gt; <see langword="typeof"/>(ListGraphType&lt;&gt;).MakeGenericType(@this);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Type ToListGraphType()
			=> typeof(ListGraphType<>).MakeGenericType(@this);

		/// <summary>
		/// <c>=&gt; <see langword="typeof"/>(NonNullGraphType&lt;&gt;).MakeGenericType(@this);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Type ToNonNullGraphType()
			=> typeof(NonNullGraphType<>).MakeGenericType(@this);
	}

	extension(PropertyEntity @this)
	{
		public FieldType ToFieldType()
		{
			var type = @this.ToGraphQLType(false);
			var arguments = new QueryArguments();

			if (type.IsAssignableTo<ScalarGraphType>() && !type.Implements(typeof(NonNullGraphType<>)))
				arguments.Add("null", type, description: "Return this if the value is null.");

			if (@this.PropertyType.IsAssignableTo<IFormattable>())
				arguments.Add<string>("format", nullable: true, description: "Use .NET format specifiers to format the data.");

			if (type.Is<DateTimeGraphType>() || type.Is<NonNullGraphType<DateTimeGraphType>>())
				arguments.Add<string>("timeZone", nullable: true, description: Invariant($"{typeof(TimeZoneInfo).Namespace}.{nameof(TimeZoneInfo)}.{nameof(TimeZoneInfo.ConvertTimeBySystemTimeZoneId)}(value, [..., ...] | [UTC, ...])"));
			else if (type.Is<DateTimeOffsetGraphType>() || type.Is<NonNullGraphType<DateTimeOffsetGraphType>>())
				arguments.Add<string>("timeZone", nullable: true, description: Invariant($"{typeof(TimeZoneInfo).Namespace}.{nameof(TimeZoneInfo)}.{nameof(TimeZoneInfo.ConvertTimeBySystemTimeZoneId)}(value, ...)"));
			else if (type.Is<StringGraphType>() || type.Is<NonNullGraphType<StringGraphType>>())
			{
				arguments.Add<StringCase?>("case", description: "value.ToLower(), value.ToLowerInvariant(), value.ToUpper(), value.ToUpperInvariant()");
				arguments.Add<int?>("length", description: "value.Left(length)");
				arguments.Add<string>("match", nullable: true, description: "value.ToRegex(RegexOptions.Compiled | RegexOptions.Singleline).Match(match).");
				arguments.Add<string>("trim", nullable: true, description: "value.Trim()");
				arguments.Add<string>("trimEnd", nullable: true, description: "value.TrimEnd()");
				arguments.Add<string>("trimStart", nullable: true, description: "value.TrimStart()");
			}

			return new()
			{
				Arguments = arguments,
				Type = type,
				Name = @this.Attributes.GraphQLName ?? @this.Name,
				Description = @this.Attributes.GraphQLDescription,
				DeprecationReason = @this.Attributes.GraphQLDeprecationReason,
				Resolver = (IFieldResolver)typeof(PropertyFieldResolver<>).MakeGenericType(@this.Type).Create([@this])!
			};
		}

		public Type ToGraphQLType(bool isInputType)
		{
			var type = @this.Attributes.GraphQLType ?? @this.PropertyType.ToGraphQLType(isInputType);
			if (!type.Is(typeof(NonNullGraphType<>)) && @this.Attributes.Any<NotNullAttribute>())
				type = type.ToNonNullGraphType();

			return type;
		}
	}

	extension(ParameterEntity @this)
	{
		public Type ToGraphQLType()
		{
			var type = @this.Attributes.GraphQLType ?? @this.ParameterType.ToGraphQLType(true);
			if (!type.Is(typeof(NonNullGraphType<>)) && @this.Attributes.Any<NotNullAttribute>())
				type = type.ToNonNullGraphType();

			return type;
		}
	}

	private static FieldType CreateFieldType(Method method, Type returnType)
		=> new()
		{
			Arguments = new QueryArguments(method.Parameters
				.Where(_ => !_.Attributes.GraphQLIgnore && !_.ParameterType.Is<IResolveFieldContext>())
				.Select(_ => new QueryArgument(_.Attributes.GraphQLType ?? _.ParameterType.ToGraphQLType(false))
				{
					Name = _.Attributes.GraphQLName ?? _.Name,
					Description = _.Attributes.GraphQLDescription,
				})),
			Name = (method.Attributes.GraphQLName ?? method.Name).TrimEndIgnoreCase("Async"),
			Description = method.Attributes.GraphQLDescription,
			DeprecationReason = method.Attributes.GraphQLDeprecationReason,
			Type = returnType.ToGraphQLType(false).ToNonNullGraphType()
		};
}
