// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using GraphQL;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.SQL;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Extensions
{
	public static class GraphAttributeExtensions
	{
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string? GraphDescription(this IMember @this)
			=> @this.Attributes.First<GraphDescriptionAttribute>()?.Description;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string? GraphDescription(this MethodParameter @this)
			=> @this.Attributes.First<GraphDescriptionAttribute>()?.Description;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool GraphIgnore(this IMember @this)
			=> @this.Attributes.Any<GraphIgnoreAttribute>();

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool GraphIgnore(this MethodParameter @this)
			=> @this.Attributes.Any<GraphIgnoreAttribute>() || @this.Type.Handle.Is<IResolveFieldContext>();

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string? GraphKey(this IMember @this)
			=> @this.Attributes.First<GraphKeyAttribute>()?.Name;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string GraphInputName(this TypeMember @this)
			=> @this.Attributes.First<GraphInputNameAttribute>()?.Name ?? $"{@this.Name}Input";

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string GraphName(this IMember @this)
			=> @this.Attributes.First<GraphNameAttribute>()?.Name ?? @this.Name;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string? GraphName(this IEnumerable<Attribute> @this)
			=> @this.First<GraphNameAttribute>()?.Name;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string GraphName(this MethodMember @this)
			=> @this.Attributes.First<GraphNameAttribute>()?.Name ?? @this.Name.TrimEnd("Async");

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string GraphName(this MethodParameter @this)
			=> @this.Attributes.First<GraphNameAttribute>()?.Name ?? @this.Name;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool GraphNonNull(this TypeMember @this)
			=> @this.Attributes.Any<NotNullAttribute>() || !@this.Nullable;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Type? GraphType(this IEnumerable<Attribute> @this)
			=> @this.First<GraphTypeAttribute>()?.GraphType;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Type GraphType(this MethodParameter @this)
			=> @this.Attributes.GraphType() ?? @this.Type.GraphType(true);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Type GraphType(this PropertyMember @this, bool isInputType)
			=> @this.Attributes.GraphType() ?? @this.PropertyType.GraphType(isInputType);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Type GraphType(this ReturnParameter @this)
			=> @this.Attributes.GraphType() ?? @this.Type.GraphType(false);

		public static Type GraphType(this ScalarType @this)
			=> @this switch
			{
				ScalarType.ID => typeof(IdGraphType),
				ScalarType.HashID => typeof(GraphHashIdType),
				ScalarType.Boolean => typeof(BooleanGraphType),
				ScalarType.SByte => typeof(SByteGraphType),
				ScalarType.Short => typeof(ShortGraphType),
				ScalarType.Int => typeof(IntGraphType),
				ScalarType.Long => typeof(LongGraphType),
				ScalarType.Byte => typeof(ByteGraphType),
				ScalarType.UShort => typeof(UShortGraphType),
				ScalarType.UInt => typeof(UIntGraphType),
				ScalarType.ULong => typeof(ULongGraphType),
				ScalarType.Float => typeof(FloatGraphType),
				ScalarType.Decimal => typeof(DecimalGraphType),
				ScalarType.Date => typeof(DateGraphType),
				ScalarType.DateTime => typeof(DateTimeGraphType),
				ScalarType.DateTimeOffset => typeof(DateTimeOffsetGraphType),
				ScalarType.TimeSpanMilliseconds => typeof(TimeSpanMillisecondsGraphType),
				ScalarType.TimeSpanSeconds => typeof(TimeSpanSecondsGraphType),
				ScalarType.Guid => typeof(GuidGraphType),
				ScalarType.String => typeof(StringGraphType),
				ScalarType.Uri => typeof(UriGraphType),
				ScalarType.NonNullableID => typeof(NonNullGraphType<IdGraphType>),
				ScalarType.NonNullableHashID => typeof(NonNullGraphType<GraphHashIdType>),
				ScalarType.NonNullableBoolean => typeof(NonNullGraphType<BooleanGraphType>),
				ScalarType.NonNullableSByte => typeof(NonNullGraphType<SByteGraphType>),
				ScalarType.NonNullableShort => typeof(NonNullGraphType<ShortGraphType>),
				ScalarType.NonNullableInt => typeof(NonNullGraphType<IntGraphType>),
				ScalarType.NonNullableLong => typeof(NonNullGraphType<LongGraphType>),
				ScalarType.NonNullableByte => typeof(NonNullGraphType<ByteGraphType>),
				ScalarType.NonNullableUShort => typeof(NonNullGraphType<UShortGraphType>),
				ScalarType.NonNullableUInt => typeof(NonNullGraphType<UIntGraphType>),
				ScalarType.NonNullableULong => typeof(NonNullGraphType<ULongGraphType>),
				ScalarType.NonNullableFloat => typeof(NonNullGraphType<FloatGraphType>),
				ScalarType.NonNullableDecimal => typeof(NonNullGraphType<DecimalGraphType>),
				ScalarType.NonNullableDate => typeof(NonNullGraphType<DateGraphType>),
				ScalarType.NonNullableDateTime => typeof(NonNullGraphType<DateTimeGraphType>),
				ScalarType.NonNullableDateTimeOffset => typeof(NonNullGraphType<DateTimeOffsetGraphType>),
				ScalarType.NonNullableTimeSpanMilliseconds => typeof(NonNullGraphType<TimeSpanMillisecondsGraphType>),
				ScalarType.NonNullableTimeSpanSeconds => typeof(NonNullGraphType<TimeSpanSecondsGraphType>),
				ScalarType.NonNullableGuid => typeof(NonNullGraphType<GuidGraphType>),
				ScalarType.NonNullableString => typeof(NonNullGraphType<StringGraphType>),
				ScalarType.NonNullableUri => typeof(NonNullGraphType<UriGraphType>),
				_ => typeof(StringGraphType)
			};

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string? ObsoleteMessage(this IMember @this)
			=> @this.Attributes.First<ObsoleteAttribute>()?.Message;

		internal static Type GraphType(this TypeMember @this, bool isInputType)
		{
			var graphType = @this.Kind switch
			{
				Kind.Delegate or Kind.Pointer => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.Kind)}", $"No custom graph type was found that supports: {@this.Kind.Name()}"),
				Kind.Enum => typeof(GraphEnumType<>).MakeGenericType(@this),
				Kind.Collection => typeof(ListGraphType<>).MakeGenericType(@this.EnclosedType!.Value.GraphType(isInputType)),
				Kind.Interface => typeof(GraphInterfaceType<>).MakeGenericType(@this),
				_ => @this.SystemType switch
				{
					SystemType.String => typeof(StringGraphType),
					SystemType.Uri => typeof(UriGraphType),
					SystemType.Boolean => typeof(BooleanGraphType),
					SystemType.SByte => typeof(SByteGraphType),
					SystemType.Int16 => typeof(ShortGraphType),
					SystemType.Int32 or SystemType.Index => typeof(IntGraphType),
					SystemType.Int64 or SystemType.NInt => typeof(LongGraphType),
					SystemType.Byte => typeof(ByteGraphType),
					SystemType.UInt16 => typeof(UShortGraphType),
					SystemType.UInt32 => typeof(UIntGraphType),
					SystemType.UInt64 or SystemType.NUInt => typeof(ULongGraphType),
					SystemType.Single or SystemType.Double => typeof(FloatGraphType),
					SystemType.Decimal => typeof(DecimalGraphType),
					SystemType.DateTime => typeof(DateTimeGraphType),
					SystemType.DateTimeOffset => typeof(DateTimeOffsetGraphType),
					SystemType.TimeSpan => typeof(TimeSpanSecondsGraphType),
					SystemType.Guid => typeof(GuidGraphType),
					SystemType.Range => typeof(StringGraphType),
					SystemType.Nullable or SystemType.Task or SystemType.ValueTask => @this.EnclosedType!.Value.GraphType(isInputType),
					_ when @this.Is(typeof(OrderBy<>)) => typeof(GraphOrderByType<>).MakeGenericType(@this),
					_ when isInputType => typeof(GraphInputType<>).MakeGenericType(@this),
					_ => typeof(GraphObjectType<>).MakeGenericType(@this)
				}
			};

			return @this.GraphNonNull() ? typeof(NonNullGraphType<>).MakeGenericType(graphType) : graphType;
		}
	}
}
