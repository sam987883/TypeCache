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
		public static object? GetValue(this PropertyMember @this, object instance)
			=> @this.Getter?.Invoke(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object? GetValue(this StaticPropertyMember @this)
			=> @this.Getter?.Invoke();

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
				for (var i = 0; i < @this.Count; ++i)
				{
					var parameter = @this[i];
					if (argumentEnumerator.MoveNext())
					{
						if (argumentEnumerator.Current != null)
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
		{
			if (@this.Count != parameters.Count)
				return false;

			for (var i = 0; i < @this.Count; ++i)
			{
				var parameter1 = @this[i];
				var parameter2 = parameters[i];
				if (!parameter1.Name.Is(parameter2.Name, true) || parameter1.Type != parameter2.Type)
					return false;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object? SetValue(this PropertyMember @this, object instance, object? value)
			=> @this.Setter?.Invoke(instance, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object? SetValue(this StaticPropertyMember @this, object? value)
			=> @this.Setter?.Invoke(value);

		public static bool Supports(this TypeMember @this, Type type)
			=> @this.Handle.Equals(type.TypeHandle) || type.IsSubclassOf(@this.Handle.ToType());
	}
}
