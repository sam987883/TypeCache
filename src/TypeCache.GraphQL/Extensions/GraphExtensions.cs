// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TypeCache.Common;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions
{
	public static class GraphExtensions
	{
		private static readonly Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> _scalarGraphTypes = new Dictionary<RuntimeTypeHandle, RuntimeTypeHandle>()
		{
			{ typeof(bool).TypeHandle, typeof(BooleanGraphType).TypeHandle },
			{ typeof(sbyte).TypeHandle, typeof(SByteGraphType).TypeHandle },
			{ typeof(byte).TypeHandle, typeof(ByteGraphType).TypeHandle },
			{ typeof(short).TypeHandle, typeof(ShortGraphType).TypeHandle },
			{ typeof(ushort).TypeHandle, typeof(UShortGraphType).TypeHandle },
			{ typeof(int).TypeHandle, typeof(IntGraphType).TypeHandle },
			{ typeof(Index).TypeHandle, typeof(IntGraphType).TypeHandle },
			{ typeof(uint).TypeHandle, typeof(UIntGraphType).TypeHandle },
			{ typeof(long).TypeHandle, typeof(LongGraphType).TypeHandle },
			{ typeof(ulong).TypeHandle, typeof(ULongGraphType).TypeHandle },
			{ typeof(float).TypeHandle, typeof(FloatGraphType).TypeHandle },
			{ typeof(double).TypeHandle, typeof(FloatGraphType).TypeHandle },
			{ typeof(decimal).TypeHandle, typeof(DecimalGraphType).TypeHandle },
			{ typeof(DateTime).TypeHandle, typeof(DateTimeGraphType).TypeHandle },
			{ typeof(DateTimeOffset).TypeHandle, typeof(DateTimeOffsetGraphType).TypeHandle },
			{ typeof(TimeSpan).TypeHandle, typeof(TimeSpanSecondsGraphType).TypeHandle },
			{ typeof(Guid).TypeHandle, typeof(GuidGraphType).TypeHandle },
			{ typeof(Uri).TypeHandle, typeof(UriGraphType).TypeHandle },
			{ typeof(Range).TypeHandle, typeof(StringGraphType).TypeHandle },
			{ typeof(string).TypeHandle, typeof(StringGraphType).TypeHandle }
		};

		public static Type GetGraphType(this IMember @this, bool isInput)
		{
			var graphType = @this switch
			{
				IMethodMember method => method.ReturnAttributes.First<Attribute, GraphAttribute>()?.Type,
				IStaticMethodMember method => method.ReturnAttributes.First<Attribute, GraphAttribute>()?.Type,
				_ => @this.Attributes.First<Attribute, GraphAttribute>()?.Type
			};

			if (graphType != null)
				return graphType;

			graphType = @this.NativeType.GetGraphType(@this.TypeHandle, isInput);

			if (@this.CollectionType != CollectionType.None)
				graphType = typeof(ListGraphType<>).MakeGenericType(graphType);

			if ((!@this.IsNullable && @this.NativeType != NativeType.Object)
				|| @this.Attributes.Any<Attribute, NotNullAttribute>())
				graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType);

			return graphType;
		}

		private static Type GetGraphType(this NativeType @this, RuntimeTypeHandle typeHandle, bool isInput)
			=> @this switch
			{
				NativeType.Object => (isInput ? typeof(GraphInputType<>) : typeof(GraphObjectType<>)).MakeGenericType(typeHandle.ToType()),
				NativeType.Enum => typeof(GraphEnumType<>).MakeGenericType(typeHandle.ToType()),
				_ => _scalarGraphTypes[typeHandle].ToType()
			};

		public static QueryArguments ToQueryArguments(this IMethodMember @this)
			=> new QueryArguments(@this.Parameters
					.If(parameter =>
					{
						var graphAttribute = parameter.Attributes.First<Attribute, GraphAttribute>();
						return graphAttribute?.Ignore != true && graphAttribute?.Inject != true;
					})
					.To(parameter => parameter.ToQueryArgument()));

		private static QueryArgument ToQueryArgument(this IParameter @this)
		{
			var graphAttribute = @this.Attributes.First<Attribute, GraphAttribute>();
			var graphType = graphAttribute?.Type;
			if (graphType == null)
			{
				graphType = @this.NativeType.GetGraphType(@this.TypeHandle, true);

				if (@this.CollectionType != CollectionType.None)
					graphType = typeof(ListGraphType<>).MakeGenericType(graphType);

				if ((!@this.IsNullable && @this.NativeType != NativeType.Object)
					|| @this.Attributes.Any<Attribute, NotNullAttribute>())
					graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType);
			}

			return new QueryArgument(graphType)
			{
				Name = graphAttribute?.Name ?? @this.Name,
				Description = graphAttribute?.Description
			};
		}
	}
}
