// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class MemberExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Implements<T>(this TypeMember @this)
			where T : class
			=> @this.Implements(typeof(T));

		public static bool Implements(this TypeMember @this, RuntimeTypeHandle handle)
			=> @this.BaseTypeHandle.Equals(handle) || @this.InterfaceTypeHandles.Any(handle.Equals);

		public static bool Implements(this TypeMember @this, Type type)
			=> @this.BaseTypeHandle.Equals(type.TypeHandle) || (type.IsInterface && @this.InterfaceTypeHandles.Any(type.TypeHandle.Equals));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this TypeMember @this)
			=> @this.Handle.Equals(typeof(T).TypeHandle);

		public static bool Is(this TypeMember @this, Type type)
			=> @this.Handle.Equals(type.TypeHandle) || (type.IsGenericTypeDefinition && @this.Handle.ToType().ToGenericType() == type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsCallableWith(this ConstructorMember @this, params object?[]? arguments)
			=> @this.Parameters.IsCallableWith(arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsCallableWith(this MethodMember @this, params object?[]? arguments)
			=> @this.Parameters.IsCallableWith(arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsCallableWith(this StaticMethodMember @this, params object?[]? arguments)
			=> @this.Parameters.IsCallableWith(arguments);

		private static bool IsCallableWith(this IImmutableList<Parameter> @this, params object?[]? arguments)
		{
			if (arguments.Any())
			{
				var argumentEnumerator = arguments.GetEnumerator();
				foreach (var parameter in @this)
				{
					if (argumentEnumerator.MoveNext())
					{
						if (argumentEnumerator.Current is not null)
						{
							if (!parameter.Type.Supports(argumentEnumerator.Current.GetType()))
								return false;
						}
						else if (!parameter.Type.IsNullable)
							return false;
					}
					else if (!parameter.HasDefaultValue && !parameter.IsOptional)
						return false;
				}
				return !argumentEnumerator.MoveNext();
			}
			return @this.Count == 0 || @this.All(parameter => parameter!.HasDefaultValue || parameter.IsOptional);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEnumerable(this TypeMember @this)
			=> @this.Handle.ToType().IsEnumerable();

		public static bool Match(this IReadOnlyList<Parameter> @this, IReadOnlyList<Parameter> parameters)
			=> @this.Count == parameters.Count && 0.Range(@this.Count).All(i => @this[i] == parameters[i]);

		public static bool IsConvertibleTo(this SystemType @this, SystemType type)
			=> type switch
			{
				SystemType.BigInteger => @this switch
				{
					SystemType.BigInteger or SystemType.Boolean or SystemType.Char or SystemType.String => true,
					SystemType.SByte or SystemType.Int16 or SystemType.Int32 or SystemType.NInt or SystemType.Int64 => true,
					SystemType.Byte or SystemType.UInt16 or SystemType.UInt32 or SystemType.NUInt or SystemType.UInt64 => true,
					_ => false
				},
				SystemType.Boolean => @this switch
				{
					SystemType.BigInteger or SystemType.Boolean or SystemType.Char or SystemType.String => true,
					SystemType.SByte or SystemType.Int16 or SystemType.Int32 or SystemType.NInt or SystemType.Int64 => true,
					SystemType.Byte or SystemType.UInt16 or SystemType.UInt32 or SystemType.NUInt or SystemType.UInt64 => true,
					SystemType.Single or SystemType.Double or SystemType.Half or SystemType.Decimal => true,
					_ => false
				},
				SystemType.Char => @this switch
				{
					SystemType.Boolean or SystemType.Char or SystemType.SByte or SystemType.Byte or SystemType.Int16 => true,
					_ => false
				},
				SystemType.String => @this switch
				{
					SystemType.BigInteger or SystemType.Boolean or SystemType.Char or SystemType.String => true,
					SystemType.SByte or SystemType.Int16 or SystemType.Int32 or SystemType.NInt or SystemType.Int64 => true,
					SystemType.Byte or SystemType.UInt16 or SystemType.UInt32 or SystemType.NUInt or SystemType.UInt64 => true,
					SystemType.Single or SystemType.Double or SystemType.Half or SystemType.Decimal => true,
					_ => false
				},
				SystemType.SByte => @this switch
				{
					SystemType.Boolean or SystemType.SByte => true,
					_ => false
				},
				SystemType.Int16 => @this switch
				{
					SystemType.Boolean or SystemType.Char or SystemType.SByte or SystemType.Int16 or SystemType.Byte => true,
					_ => false
				},
				SystemType.Int32 or SystemType.NInt => @this switch
				{
					SystemType.Boolean or SystemType.Char or SystemType.SByte or SystemType.Int16 or SystemType.Int32 or SystemType.NInt or SystemType.Int64 => true,
					SystemType.Byte or SystemType.UInt16 => true,
					_ => false
				},
				SystemType.Int64 => @this switch
				{
					SystemType.BigInteger or SystemType.Boolean or SystemType.Char or SystemType.String => true,
					SystemType.SByte or SystemType.Int16 or SystemType.Int32 or SystemType.NInt or SystemType.Int64 => true,
					SystemType.Byte or SystemType.UInt16 or SystemType.UInt32 or SystemType.NUInt => true,
					_ => false
				},
				SystemType.Byte => @this switch
				{
					SystemType.Boolean or SystemType.SByte or SystemType.Byte => true,
					_ => false
				},
				SystemType.UInt16 => @this switch
				{
					SystemType.Boolean or SystemType.Char or SystemType.SByte or SystemType.Int16 or SystemType.Byte or SystemType.UInt16 => true,
					_ => false
				},
				SystemType.UInt32 or SystemType.NUInt => @this switch
				{
					SystemType.Boolean or SystemType.Char or SystemType.SByte or SystemType.Int16 or SystemType.Int32 or SystemType.NInt => true,
					SystemType.Byte or SystemType.UInt16 or SystemType.UInt32 or SystemType.NUInt => true,
					_ => false
				},
				SystemType.UInt64 => @this switch
				{
					SystemType.Boolean or SystemType.Char => true,
					SystemType.SByte or SystemType.Int16 or SystemType.Int32 or SystemType.NInt or SystemType.Int64 => true,
					SystemType.Byte or SystemType.UInt16 or SystemType.UInt32 or SystemType.NUInt or SystemType.UInt64 => true,
					_ => false
				},
				SystemType.Single => @this switch
				{
					SystemType.Single => true,
					_ => false
				},
				SystemType.Double => @this switch
				{
					SystemType.Single or SystemType.Double => true,
					_ => false
				},
				SystemType.Half => @this switch
				{
					SystemType.Single or SystemType.Double or SystemType.Half => true,
					_ => false
				},
				SystemType.Decimal => @this switch
				{
					SystemType.Single or SystemType.Double or SystemType.Half or SystemType.Decimal => true,
					_ => false
				},
				_ => false
			};

		public static bool Supports(this TypeMember @this, Type type)
		{
			if (@this.Handle.Equals(type.TypeHandle))
				return true;

			var parameterType = @this.Handle.ToType();
			if (type.IsSubclassOf(parameterType))
				return true;

			if (type.GetSystemType().IsConvertibleTo(@this.SystemType))
				return true;

			if (type.IsEnum)
				type = type.GetEnumUnderlyingType();

			if (parameterType.IsEnum)
				parameterType = parameterType.GetEnumUnderlyingType();

			return type == parameterType;
		}
	}
}
