// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLExtensions
{
	public static FieldType AddField<T>(this IComplexGraphType @this, string name, IFieldResolver resolver)
		=> @this.AddField(new()
		{
			Name = name,
			Type = typeof(T).ToGraphQLType(false),
			Resolver = resolver
		});

	public static FieldType AddField(this IComplexGraphType @this, string name, IGraphType resolvedType, IFieldResolver resolver)
		=> @this.AddField(new()
		{
			Name = name,
			ResolvedType = resolvedType,
			Resolver = resolver
		});

	public static FieldType AddField(this IComplexGraphType @this, MethodEntity method)
		=> @this.AddField(method, new MethodFieldResolver(method));

	public static FieldType AddField(this IComplexGraphType @this, MethodEntity method, IFieldResolver resolver)
	{
		var fieldType = CreateFieldType(method);
		fieldType.Resolver = resolver;
		return fieldType;
	}

	public static FieldType AddField(this IComplexGraphType @this, StaticMethodEntity method)
		=> @this.AddField(method, new StaticMethodFieldResolver(method));

	public static FieldType AddField(this IComplexGraphType @this, StaticMethodEntity method, IFieldResolver resolver)
	{
		var fieldType = CreateFieldType(method);
		fieldType.Resolver = resolver;
		return fieldType;
	}

	public static FieldType AddFieldStream(this IComplexGraphType @this, MethodEntity method)
		=> @this.AddFieldStream(method, new MethodSourceStreamResolver(method));

	public static FieldType AddFieldStream(this IComplexGraphType @this, MethodEntity method, ISourceStreamResolver resolver)
	{
		var fieldType = CreateFieldType(method);
		fieldType.StreamResolver = resolver;
		return fieldType;
	}

	public static FieldType AddFieldStream(this IComplexGraphType @this, StaticMethodEntity method)
		=> @this.AddFieldStream(method, new StaticMethodSourceStreamResolver(method));

	public static FieldType AddFieldStream(this IComplexGraphType @this, StaticMethodEntity method, ISourceStreamResolver resolver)
	{
		var fieldType = CreateFieldType(method);
		fieldType.StreamResolver = resolver;
		return fieldType;
	}

	public static void AddOrderBy(this EnumerationGraphType @this, string column, string? deprecationReason = null)
	{
		var asc = Sort.Ascending.ToSQL();
		var desc = Sort.Descending.ToSQL();

		@this.Add(Invariant($"{column}_{asc}"), Invariant($"{column} {asc}"), Invariant($"{column} {asc}"), deprecationReason);
		@this.Add(Invariant($"{column}_{desc}"), Invariant($"{column} {desc}"), Invariant($"{column} {desc}"), deprecationReason);
	}

	/// <summary>
	/// Use this to create a GraphQL Connection object to return in your endpoint to support paging.
	/// </summary>
	/// <typeparam name="T">.</typeparam>
	/// <param name="data">The data<see cref="IEnumerable{T}"/>.</param>
	/// <param name="offset">The number of records to skip.</param>
	/// <param name="totalCount">The total record count of the record set being paged.</param>
	/// <returns>The <see cref="Connection{T}"/>.</returns>
	public static Connection<T> ToConnection<T>(this IEnumerable<T> data, uint offset, int totalCount)
		where T : notnull
	{
		var items = data.AsArray();
		return new(offset, items)
		{
			PageInfo = new(offset, offset + (uint)items.Length, totalCount),
			TotalCount = totalCount
		};
	}

	public static FieldType ToFieldType(this PropertyEntity @this)
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
			Name = @this.Attributes.GraphQLName() ?? @this.Name,
			Description = @this.Attributes.FirstOrDefault<GraphQLDescriptionAttribute>()?.Description,
			DeprecationReason = @this.Attributes.FirstOrDefault<GraphQLDeprecationReasonAttribute>()?.DeprecationReason,
			Resolver = (IFieldResolver)typeof(PropertyFieldResolver<>).MakeGenericType(@this.Type).Create([@this])!
		};
	}

	public static Type ToGraphQLType(this ParameterEntity @this)
	{
		var type = @this.Attributes.FirstOrDefault<GraphQLTypeAttribute>()?.Type ?? @this.ParameterType.ToGraphQLType(true);
		if (!type.Is(typeof(NonNullGraphType<>)) && @this.Attributes.Any<NotNullAttribute>())
			type = type.ToNonNullGraphType();

		return type;
	}

	public static Type ToGraphQLType(this PropertyEntity @this, bool isInputType)
	{
		var type = @this.Attributes.FirstOrDefault<GraphQLTypeAttribute>()?.Type ?? @this.PropertyType.ToGraphQLType(isInputType);
		if (!type.Is(typeof(NonNullGraphType<>)) && @this.Attributes.Any<NotNullAttribute>())
			type = type.ToNonNullGraphType();

		return type;
	}

	public static Type ToGraphQLType(this Type @this, bool isInputType)
	{
		if (@this.Is(typeof(Nullable<>)))
			return @this.GenericTypeArguments[0].ToGraphQLType(isInputType);

		if (@this.IsEnum)
			return typeof(EnumGraphType<>).MakeGenericType(@this);

		var objectType = @this.ObjectType();
		(objectType is ObjectType.Delegate).ThrowIfTrue();
		(objectType is ObjectType.Object).ThrowIfTrue();

		var scalarGraphType = @this.ScalarType().ToGraphType();
		if (scalarGraphType is not null)
			return scalarGraphType;

		var collectionType = @this.CollectionType();
		if (collectionType.IsDictionary())
			return typeof(KeyValuePair<,>).MakeGenericType(@this.GenericTypeArguments).ToGraphQLType(isInputType).ToNonNullGraphType().ToListGraphType();

		if (objectType is ObjectType.Task || objectType is ObjectType.ValueTask)
			return @this.IsGenericType
				? @this.GenericTypeArguments.First()!.ToGraphQLType(false)
				: throw new ArgumentOutOfRangeException(nameof(@this), Invariant($"{nameof(Task)} and {nameof(ValueTask)} are not allowed as GraphQL types."));

		if (@this.HasElementType)
		{
			var elementType = @this.GetElementType()!.ToGraphQLType(isInputType);
			if (elementType.IsValueType && !elementType.Is(typeof(Nullable<>)))
				elementType = elementType.ToNonNullGraphType();

			return elementType.ToListGraphType();
		}

		if (@this.Is(typeof(IEnumerable<>)) || @this.Implements(typeof(IEnumerable<>)))
			return @this.GenericTypeArguments.First()!.ToGraphQLType(isInputType).ToListGraphType();

		if (@this.IsInterface)
			return typeof(Types.InterfaceGraphType<>).MakeGenericType(@this);

		return isInputType
			? typeof(InputGraphType<>).MakeGenericType(@this)
			: typeof(OutputGraphType<>).MakeGenericType(@this);
	}

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(ListGraphType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToListGraphType(this Type @this)
		=> typeof(ListGraphType<>).MakeGenericType(@this);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(NonNullGraphType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToNonNullGraphType(this Type @this)
		=> typeof(NonNullGraphType<>).MakeGenericType(@this);

	private static FieldType CreateFieldType(Method method)
		=> new()
		{
			Arguments = new QueryArguments(method.Parameters
				.Where(_ => !_.Attributes.GraphQLIgnore() && !_.ParameterType.Is<IResolveFieldContext>())
				.Select(_ => new QueryArgument(_.Attributes.GraphQLType() ?? _.ParameterType.ToGraphQLType(false))
				{
					Name = _.Attributes.GraphQLName() ?? _.Name,
					Description = _.Attributes.GraphQLDescription(),
				})),
			Name = (method.Attributes.GraphQLName() ?? method.Name).TrimEndIgnoreCase("Async"),
			Description = method.Attributes.GraphQLDescription(),
			DeprecationReason = method.Attributes.GraphQLDeprecationReason(),
			Type = method.Return.ParameterType.ToGraphQLType(false).ToNonNullGraphType()
		};
}
