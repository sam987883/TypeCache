﻿// Copyright (c) 2021 Samuel Abraham

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

namespace TypeCache.GraphQL.Extensions
{
	public static class GraphAttributeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GraphCursor(this IEnumerable<Attribute> @this)
			=> @this.Any<GraphCursorAttribute>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? GraphDescription(this IEnumerable<Attribute> @this)
			=> @this.First<GraphDescriptionAttribute>()?.Description;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? GraphName(this IEnumerable<Attribute> @this)
			=> @this.First<GraphNameAttribute>()?.Name;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GraphIgnore(this IEnumerable<Attribute> @this)
			=> @this.Any<GraphIgnoreAttribute>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type? GraphType(this IEnumerable<Attribute> @this)
			=> @this.First<GraphTypeAttribute>()?.GraphType;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type GetGraphType(this MethodParameter @this)
			=> GetGraphType(@this.Type, @this.Attributes, true);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type GetGraphType(this PropertyMember @this, bool isInputType)
			=> GetGraphType(@this.PropertyType, @this.Attributes, isInputType);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type GetGraphType(this ReturnParameter @this)
			=> GetGraphType(@this.Type, @this.Attributes, false);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? ObsoleteMessage(this IEnumerable<Attribute> @this)
			=> @this.First<ObsoleteAttribute>()?.Message;

		public static Type ToGraphType(this ScalarType @this)
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

		private static Type GetGraphType(TypeMember type, IEnumerable<Attribute> attributes, bool isInputType)
		{
			var graphType = attributes.GraphType();
			if (graphType is not null)
				return graphType;

			graphType = type.ToGraphType(isInputType);

			if (attributes.Any<NotNullAttribute>() || !type.IsNullable())
				graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType);

			return graphType;
		}

		private static Type ToGraphType(this TypeMember @this, bool isInputType)
			=> @this.Kind switch
			{
				Kind.Delegate or Kind.Pointer => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.Kind)}", $"No custom graph type was found that supports: {@this.Kind.Name()}"),
				Kind.Enum => typeof(GraphEnumType<>).MakeGenericType(@this),
				Kind.Collection => typeof(ListGraphType<>).MakeGenericType(@this.EnclosedType!.ToGraphType(isInputType)),
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
					SystemType.Nullable or SystemType.Task or SystemType.ValueTask => @this.EnclosedType!.ToGraphType(isInputType),
					_ when @this.Is(typeof(OrderBy<>)) => typeof(GraphObjectSortEnumType<>).MakeGenericType(@this),
					_ when isInputType => typeof(GraphInputType<>).MakeGenericType(@this),
					_ => typeof(GraphObjectType<>).MakeGenericType(@this)
				}
			};
	}
}
