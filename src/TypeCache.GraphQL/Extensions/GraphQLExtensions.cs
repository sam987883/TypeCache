// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using System.Data;
using System.Numerics;
using System.Reflection;
using GraphQL;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLExtensions
{
	/// <summary>
	/// Returns a dictionary of default CLR type to graph type mappings for a set of built-in (primitive) types.
	/// </summary>
	private static readonly IReadOnlyDictionary<RuntimeTypeHandle, RuntimeTypeHandle> ScalarMappings = new Dictionary<RuntimeTypeHandle, RuntimeTypeHandle>
	{
		{ typeof(bool).TypeHandle, typeof(GraphQLBooleanType).TypeHandle },
		{ typeof(sbyte).TypeHandle, typeof(GraphQLNumberType<sbyte>).TypeHandle },
		{ typeof(short).TypeHandle, typeof(GraphQLNumberType<short>).TypeHandle },
		{ typeof(int).TypeHandle, typeof(GraphQLNumberType<int>).TypeHandle },
		{ typeof(long).TypeHandle, typeof(GraphQLNumberType<long>).TypeHandle },
		{ typeof(Int128).TypeHandle, typeof(GraphQLNumberType<Int128>).TypeHandle },
		{ typeof(BigInteger).TypeHandle, typeof(GraphQLNumberType<BigInteger>).TypeHandle },
		{ typeof(byte).TypeHandle, typeof(GraphQLNumberType<byte>).TypeHandle },
		{ typeof(ushort).TypeHandle, typeof(GraphQLNumberType<ushort>).TypeHandle },
		{ typeof(uint).TypeHandle, typeof(GraphQLNumberType<uint>).TypeHandle },
		{ typeof(ulong).TypeHandle, typeof(GraphQLNumberType<ulong>).TypeHandle },
		{ typeof(UInt128).TypeHandle, typeof(GraphQLNumberType<UInt128>).TypeHandle },
		{ typeof(nint).TypeHandle, typeof(GraphQLNumberType<nint>).TypeHandle },
		{ typeof(nuint).TypeHandle, typeof(GraphQLNumberType<nuint>).TypeHandle },
		{ typeof(Half).TypeHandle, typeof(GraphQLNumberType).TypeHandle },
		{ typeof(double).TypeHandle, typeof(GraphQLNumberType).TypeHandle },
		{ typeof(float).TypeHandle, typeof(GraphQLNumberType).TypeHandle },
		{ typeof(decimal).TypeHandle, typeof(GraphQLNumberType).TypeHandle },
		{ typeof(string).TypeHandle, typeof(GraphQLStringType).TypeHandle },
		{ typeof(DateOnly).TypeHandle, typeof(GraphQLStringType<DateOnly>).TypeHandle },
		{ typeof(DateTime).TypeHandle, typeof(GraphQLStringType<DateTime>).TypeHandle },
		{ typeof(DateTimeOffset).TypeHandle, typeof(GraphQLStringType<DateTimeOffset>).TypeHandle },
		{ typeof(TimeOnly).TypeHandle, typeof(GraphQLStringType<TimeOnly>).TypeHandle },
		{ typeof(TimeSpan).TypeHandle, typeof(GraphQLStringType<TimeSpan>).TypeHandle },
		{ typeof(Guid).TypeHandle, typeof(GraphQLStringType<Guid>).TypeHandle },
		{ typeof(Uri).TypeHandle, typeof(GraphQLUriType).TypeHandle },
	}.ToFrozenDictionary();

	public static FieldType AddField(this IObjectGraphType @this, string name, IGraphType resolvedType, IFieldResolver resolver)
	{
		var fieldType = new FieldType()
		{
			Name = name,
			ResolvedType = resolvedType,
			Resolver = resolver
		};
		@this.Fields.Add(fieldType);
		return fieldType;
	}

	public static FieldType AddField(this IObjectGraphType @this, MethodInfo methodInfo, IFieldResolver resolver)
	{
		var fieldType = new FieldType()
		{
			Arguments = methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore() && !parameterInfo.ParameterType.Is<IResolveFieldContext>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.GraphQLName(), parameterInfo.ToGraphQLType())
				{
					Description = parameterInfo.GraphQLDescription(),
				})
				.ToArray(),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			Resolver = resolver,
			Type = methodInfo.ReturnType.ToGraphQLType(false).ToNonNullGraphType()
		};

		@this.Fields.Add(fieldType);
		return fieldType;
	}

	public static FieldType AddField(this IObjectGraphType @this, MethodInfo methodInfo, ISourceStreamResolver resolver)
	{
		var fieldType = new FieldType()
		{
			Arguments = methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore() && !parameterInfo.ParameterType.Is<IResolveFieldContext>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.GraphQLName(), parameterInfo.ToGraphQLType())
				{
					Description = parameterInfo.GraphQLDescription(),
				})
				.ToArray(),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			StreamResolver = resolver,
			Type = methodInfo.ReturnType.ToGraphQLType(false).ToNonNullGraphType()
		};
		@this.Fields.Add(fieldType);
		return fieldType;
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

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLEnumType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLEnumType(this Type @this)
		=> typeof(GraphQLEnumType<>).MakeGenericType(@this);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLInputType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLInputType(this Type @this)
		=> typeof(GraphQLInputType<>).MakeGenericType(@this);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLInterfaceType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLInterfaceType(this Type @this)
		=> typeof(GraphQLInterfaceType<>).MakeGenericType(@this);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLObjectType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLObjectType(this Type @this)
		=> typeof(GraphQLObjectType<>).MakeGenericType(@this);

	public static Type ToGraphQLType(this ParameterInfo @this)
	{
		var type = @this.GraphQLType() ?? @this.ParameterType!.ToGraphQLType(true);
		if (!type.Is(typeof(NonNullGraphType<>)) && @this.HasCustomAttribute<NotNullAttribute>())
			type = type.ToNonNullGraphType();

		return type;
	}

	public static Type ToGraphQLType(this PropertyInfo @this, bool isInputType)
	{
		var type = @this.GraphQLType() ?? @this.PropertyType.ToGraphQLType(isInputType);
		if (!type.Is(typeof(NonNullGraphType<>)) && @this.HasCustomAttribute<NotNullAttribute>())
			type = type.ToNonNullGraphType();

		return type;
	}

	public static Type ToGraphQLType(this Type @this, bool isInputType)
	{
		if (@this.Is(typeof(Nullable<>)))
			return @this.GenericTypeArguments[0].ToGraphQLType(isInputType);

		if (@this.IsEnum)
			return @this.ToGraphQLEnumType();

		var objectType = @this.GetObjectType();
		(objectType is ObjectType.Delegate).ThrowIfTrue();
		(objectType is ObjectType.Object).ThrowIfTrue();

		var scalarGraphType = @this.GetScalarType().ToGraphType();
		if (scalarGraphType is not null)
			return scalarGraphType;

		var collectionType = @this.GetCollectionType();
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
			return @this.ToGraphQLInterfaceType();

		return isInputType ? @this.ToGraphQLInputType() : @this.ToGraphQLObjectType();
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
}
