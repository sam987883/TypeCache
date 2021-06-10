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
		/// <summary>
		/// <c>@<paramref name="this"/>.Implements(typeof(<typeparamref name="T"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Implements<T>(this TypeMember @this)
			where T : class
			=> @this.Implements(typeof(T));

		/// <summary>
		/// <c>@<paramref name="this"/>.BaseTypeHandle.Equals(<paramref name="handle"/>) || @<paramref name="this"/>.InterfaceTypeHandles.Any(<paramref name="handle"/>.Equals)</c>
		/// </summary>
		public static bool Implements(this TypeMember @this, RuntimeTypeHandle handle)
			=> @this.BaseTypeHandle.Equals(handle) || @this.InterfaceTypeHandles.Any(handle.Equals);

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.BaseTypeHandle.Equals(<paramref name="type"/>.TypeHandle)
		/// || (<see cref="Type.IsInterface"/> &amp;&amp; @<paramref name="this"/>.InterfaceTypeHandles.Any(<see cref="RuntimeTypeHandle.Equals(RuntimeTypeHandle)"/>))
		/// </code>
		/// </summary>
		public static bool Implements(this TypeMember @this, Type type)
			=> @this.BaseTypeHandle.Equals(type.TypeHandle) || (type.IsInterface && @this.InterfaceTypeHandles.Any(type.TypeHandle.Equals));

		/// <summary>
		/// <c><see cref="RuntimeTypeHandle.Equals(RuntimeTypeHandle)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this TypeMember @this)
			=> @this.Handle.Equals(typeof(T).TypeHandle);

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.BaseTypeHandle.Equals(<see cref="Type.TypeHandle"/>)
		/// || (<see cref="Type.IsGenericTypeDefinition"/> &amp;&amp; ((<see cref="Type"/>)@<paramref name="this"/>).ToGenericType() == <paramref name="type"/>)
		/// </code>
		/// </summary>
		public static bool Is(this TypeMember @this, Type type)
			=> @this.Handle.Equals(type.TypeHandle) || (type.IsGenericTypeDefinition && ((Type)@this).ToGenericType() == type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsCallableWith(this ConstructorMember @this, params object?[]? arguments)
			=> @this.Parameters.IsCallableWith(arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsCallableWith(this MethodMember @this, params object?[]? arguments)
			=> @this.Parameters.IsCallableWith(arguments);

		private static bool IsCallableWith(this IImmutableList<MethodParameter> @this, params object?[]? arguments)
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
						else if (!parameter.Type.IsNullable())
							return false;
					}
					else if (!parameter.HasDefaultValue && !parameter.IsOptional)
						return false;
				}
				return !argumentEnumerator.MoveNext();
			}
			return @this.Count == 0 || @this.All(parameter => parameter!.HasDefaultValue || parameter.IsOptional);
		}

		/// <summary>
		/// <c>@<paramref name="this"/>.Kind is not <see cref="Kind.Struct"/> || @<paramref name="this"/>.SystemType is <see cref="SystemType.Nullable"/></c>
		/// </summary>
		public static bool IsNullable(this TypeMember @this)
			=> @this.Kind is not Kind.Struct || @this.SystemType is SystemType.Nullable;

		/// <summary>
		/// <c>@<paramref name="this"/>.Count == <paramref name="parameters"/>.Count &amp;&amp; 0.Range(@<paramref name="this"/>.Count).All(i => @<paramref name="this"/>[i] == <paramref name="parameters"/>[i])</c>
		/// </summary>
		public static bool Match(this IReadOnlyList<MethodParameter> @this, IReadOnlyList<MethodParameter> parameters)
			=> @this.Count == parameters.Count && 0.Range(@this.Count).All(i => @this[i] == parameters[i]);

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="SystemType.Boolean"/></item>
		/// <item><see cref="SystemType.SByte"/>, <see cref="SystemType.Byte"/></item>
		/// <item><see cref="SystemType.Int16"/>, <see cref="SystemType.UInt16"/></item>
		/// <item><see cref="SystemType.Int32"/>, <see cref="SystemType.UInt32"/></item>
		/// <item><see cref="SystemType.Int64"/>, <see cref="SystemType.UInt64"/></item>
		/// <item><see cref="SystemType.NInt"/>, <see cref="SystemType.NUInt"/></item>
		/// <item><see cref="SystemType.Single"/></item>
		/// <item><see cref="SystemType.Double"/></item>
		/// <item><see cref="SystemType.Decimal"/></item>
		/// </list>
		/// </code>
		/// </summary>
		public static bool IsConvertible(this SystemType @this)
			=> @this switch
			{
				SystemType.Boolean or SystemType.Char or SystemType.String
				or SystemType.SByte or SystemType.Int16 or SystemType.Int32 or SystemType.Int64 or SystemType.NInt
				or SystemType.Byte or SystemType.UInt16 or SystemType.UInt32 or SystemType.UInt64 or SystemType.NUInt
				or SystemType.Single or SystemType.Double or SystemType.Decimal => true,
				_ => false
			};

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.Handle.Equals(<paramref name="type"/>.TypeHandle)
		/// || <paramref name="type"/>.IsSubclassOf(@<paramref name="this"/>)
		/// || (<paramref name="type"/>.GetSystemType().IsConvertible() &amp;&amp; @<paramref name="this"/>.SystemType.IsConvertible())
		/// </code>
		/// </summary>
		public static bool Supports(this TypeMember @this, Type type)
			=> @this.Handle.Equals(type.TypeHandle) || type.IsSubclassOf(@this) || (type.GetSystemType().IsConvertible() && @this.SystemType.IsConvertible());
	}
}
