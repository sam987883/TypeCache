// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;
using static System.FormattableString;
using static System.Globalization.CultureInfo;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLAttributeExtensions
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string? GraphQLDeprecationReason(this MemberInfo @this)
		=> @this.GetCustomAttribute<GraphQLDeprecationReasonAttribute>()?.DeprecationReason;

	public static string? GraphQLDescription(this MemberInfo @this)
		=> @this.GetCustomAttribute<GraphQLDescriptionAttribute>()?.Description switch
		{
			null => null,
			var description when @this is Type type && type.IsGenericType => string.Format(InvariantCulture, description, type.GenericTypeArguments.Select(_ => _.GraphQLName()).ToArray()),
			var description => description
		};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string? GraphQLDescription(this ParameterInfo @this)
		=> @this.GetCustomAttribute<GraphQLDescriptionAttribute>()?.Description;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GraphQLIgnore(this MemberInfo @this)
		=> @this.HasCustomAttribute<GraphQLIgnoreAttribute>();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GraphQLIgnore(this ParameterInfo @this)
		=> @this.HasCustomAttribute<GraphQLIgnoreAttribute>();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string? GraphQLKey(this MemberInfo @this)
		=> @this.GetCustomAttribute<GraphQLKeyAttribute>()?.Name;

	public static string GraphQLInputName(this Type @this)
		=> @this.GetCustomAttribute<GraphQLInputNameAttribute>()?.Name switch
		{
			null => Invariant($"{@this.GraphQLName()}Input"),
			var name when @this.IsGenericType => string.Format(InvariantCulture, name, @this.GenericTypeArguments.Select(_ => _.Name()).ToArray()),
			var name => name
		};

	public static string GraphQLName(this MemberInfo @this)
		=> @this switch
		{
			MethodInfo methodInfo => methodInfo.GraphQLName(),
			Type type => type.GraphQLName(),
			_ => @this.GetCustomAttribute<GraphQLNameAttribute>()?.Name ?? @this.Name()
		};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string GraphQLName(this MethodInfo @this)
		=> @this.GetCustomAttribute<GraphQLNameAttribute>()?.Name ?? @this.Name().TrimEnd("Async");

	public static string GraphQLName(this Type @this)
		=> @this.GetCustomAttribute<GraphQLNameAttribute>()?.Name switch
		{
			null when @this.IsGenericType => Invariant($"{@this.GenericTypeArguments.First().Name()}{@this.Name()}"),
			null => @this.Name(),
			var name when @this.IsGenericType => string.Format(InvariantCulture, name, @this.GenericTypeArguments.Select(type => type.Name()).ToArray()),
			var name => name
		};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string GraphQLName(this ParameterInfo @this)
		=> @this.GetCustomAttribute<GraphQLNameAttribute>()?.Name ?? @this.Name();

	public static Type GraphQLType(this ParameterInfo @this)
	{
		var attribute = @this.GetCustomAttribute<GraphQLTypeAttribute>();
		var type = attribute is not null
			? attribute.GetType().GenericTypeArguments[0]
			: @this.ParameterType!.GraphQLType(true);

		if (!type.Is(typeof(NonNullGraphType<>)) && @this.HasCustomAttribute<NotNullAttribute>())
			type = type.ToNonNullGraphType();

		return type;
	}

	public static Type GraphQLType(this PropertyInfo @this, bool isInputType)
	{
		var attribute = @this.GetCustomAttribute<GraphQLTypeAttribute>();
		var type = attribute is not null
			? attribute.GetType().GenericTypeArguments[0]
			: @this.PropertyType.GraphQLType(isInputType);

		if (!type.Is(typeof(NonNullGraphType<>)) && @this.HasCustomAttribute<NotNullAttribute>())
			type = type.ToNonNullGraphType();

		return type;
	}

	internal static Type GraphQLType(this Type @this, bool isInputType)
	{
		var objectType = @this.GetObjectType();
		(objectType is ObjectType.Delegate).AssertFalse();
		(objectType is ObjectType.Object).AssertFalse();

		var systemType = @this.GetSystemType();
		if (systemType is SystemType.Nullable)
		{
			var type = @this.GenericTypeArguments.First();
			return type switch
			{
				{ IsEnum: true } => type.ToGraphQLEnumType(),
				_ when SystemGraphTypes.TryGetValue(type.GetSystemType(), out var handle) => handle.ToType(),
				_ => throw new UnreachableException($"No GraphQL type could be found for type [{type.Name()}].")
			};
		}

		return @this switch
		{
			{ IsEnum: true } => @this.ToGraphQLEnumType().ToNonNullGraphType(),
			_ when SystemGraphTypes.TryGetValue(systemType, out var handle) => @this.IsValueType ? handle.ToType().ToNonNullGraphType() : handle.ToType(),
			{ HasElementType: true } => @this.GetElementType()!.GraphQLType(isInputType).ToListGraphType(),
			{ IsGenericType: true } when systemType.IsAny(SystemType.Task, SystemType.ValueTask) => @this.GenericTypeArguments.First().NullableGraphQLType(),
			{ IsGenericType: true } when objectType is ObjectType.Dictionary => typeof(KeyValuePair<,>).MakeGenericType(@this.GenericTypeArguments).GraphQLType(isInputType).ToListGraphType(),
			{ IsGenericType: true } => @this.GenericTypeArguments.First().GraphQLType(isInputType).ToListGraphType(),
			{ IsInterface: true } => @this.ToGraphQLInterfaceType(),
			_ when isInputType => @this.ToGraphQLInputType(),
			_ => @this.ToGraphQLObjectType()
		};
	}

	internal static Type NullableGraphQLType(this Type @this)
	{
		var objectType = @this.GetObjectType();
		(objectType is ObjectType.Delegate).AssertFalse();
		(objectType is ObjectType.Object).AssertFalse();

		var systemType = @this.GetSystemType();
		return @this switch
		{
			{ IsEnum: true } => @this.ToGraphQLEnumType(),
			_ when systemType is SystemType.Nullable => @this.GenericTypeArguments.First().NullableGraphQLType(),
			_ when SystemGraphTypes.TryGetValue(systemType, out var handle) => handle.ToType(),
			{ HasElementType: true } => @this.GetElementType()!.NullableGraphQLType().ToListGraphType(),
			{ IsGenericType: true } when systemType.IsAny(SystemType.Task, SystemType.ValueTask) => @this.GenericTypeArguments.First().NullableGraphQLType(),
			{ IsGenericType: true } when objectType is ObjectType.Dictionary => typeof(KeyValuePair<,>).MakeGenericType(@this.GenericTypeArguments).NullableGraphQLType().ToListGraphType(),
			{ IsGenericType: true } => @this.GenericTypeArguments.First().NullableGraphQLType().ToListGraphType(),
			{ IsInterface: true } => @this.ToGraphQLInterfaceType(),
			_ => @this.ToGraphQLObjectType()
		};
	}

	private static readonly IReadOnlyDictionary<SystemType, RuntimeTypeHandle> SystemGraphTypes = new Dictionary<SystemType, RuntimeTypeHandle>(26, EnumOf<SystemType>.Comparer)
	{
		{ SystemType.String, typeof(StringGraphType).TypeHandle },
		{ SystemType.Uri, typeof(UriGraphType).TypeHandle },
		{ SystemType.Boolean, typeof(BooleanGraphType).TypeHandle },
		{ SystemType.SByte, typeof(SByteGraphType).TypeHandle },
		{ SystemType.Int16, typeof(ShortGraphType).TypeHandle },
		{ SystemType.Int32, typeof(IntGraphType).TypeHandle },
		{ SystemType.Index, typeof(IntGraphType).TypeHandle },
		{ SystemType.Int64, typeof(LongGraphType).TypeHandle },
		{ SystemType.IntPtr, typeof(LongGraphType).TypeHandle },
		{ SystemType.Int128, typeof(BigIntGraphType).TypeHandle },
		{ SystemType.Byte, typeof(ByteGraphType).TypeHandle },
		{ SystemType.UInt16, typeof(UShortGraphType).TypeHandle },
		{ SystemType.UInt32, typeof(UIntGraphType).TypeHandle },
		{ SystemType.UInt64, typeof(ULongGraphType).TypeHandle },
		{ SystemType.UIntPtr, typeof(ULongGraphType).TypeHandle },
		{ SystemType.UInt128, typeof(BigIntGraphType).TypeHandle },
		{ SystemType.BigInteger, typeof(BigIntGraphType).TypeHandle },
		{ SystemType.Half, typeof(HalfGraphType).TypeHandle },
		{ SystemType.Single, typeof(FloatGraphType).TypeHandle },
		{ SystemType.Double, typeof(FloatGraphType).TypeHandle },
		{ SystemType.Decimal, typeof(DecimalGraphType).TypeHandle },
		{ SystemType.DateOnly, typeof(DateOnlyGraphType).TypeHandle },
		{ SystemType.DateTime, typeof(DateTimeGraphType).TypeHandle },
		{ SystemType.DateTimeOffset, typeof(DateTimeOffsetGraphType).TypeHandle },
		{ SystemType.TimeOnly, typeof(TimeOnlyGraphType).TypeHandle },
		{ SystemType.TimeSpan, typeof(TimeSpanSecondsGraphType).TypeHandle },
		{ SystemType.Guid, typeof(GuidGraphType).TypeHandle },
		{ SystemType.Range, typeof(StringGraphType).TypeHandle }
	}.ToImmutableDictionary();
}
