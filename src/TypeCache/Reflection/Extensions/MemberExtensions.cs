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

		public static bool Supports(this TypeMember @this, Type type)
		{
			if (@this.Handle.Equals(type.TypeHandle))
				return true;

			var parameterType = @this.Handle.ToType();
			if (type.IsSubclassOf(parameterType))
				return true;

			if (type.IsEnum)
				type = type.GetEnumUnderlyingType();

			if (parameterType.IsEnum)
				parameterType = parameterType.GetEnumUnderlyingType();

			return type == parameterType;
		}
	}
}
