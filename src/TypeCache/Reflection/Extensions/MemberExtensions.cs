// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection.Extensions
{
	public static class MemberExtensions
	{
		public static TypeMember GetTypeMember<T>(this T @this)
			=> @this switch
			{
				Type type => type.TypeHandle.GetTypeMember(),
				MemberInfo memberInfo => memberInfo.DeclaringType!.TypeHandle.GetTypeMember(),
				null => TypeOf<T>.Member,
				_ => @this.GetType().TypeHandle.GetTypeMember()
			};

		internal static bool IsCallableWith(this IImmutableList<MethodParameter> @this, params object?[]? arguments)
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
						else if (!parameter.Type.Nullable)
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
		/// <list type="table">
		/// <item><c><see cref="SystemType.Boolean"/></c></item>
		/// <item><c><see cref="SystemType.SByte"/>, <see cref="SystemType.Byte"/></c></item>
		/// <item><c><see cref="SystemType.Int16"/>, <see cref="SystemType.UInt16"/></c></item>
		/// <item><c><see cref="SystemType.Int32"/>, <see cref="SystemType.UInt32"/></c></item>
		/// <item><c><see cref="SystemType.NInt"/>, <see cref="SystemType.NUInt"/></c></item>
		/// <item><c><see cref="SystemType.Int64"/>, <see cref="SystemType.UInt64"/></c></item>
		/// <item><c><see cref="SystemType.Single"/>, <see cref="SystemType.Double"/>, <see cref="SystemType.Decimal"/></c></item>
		/// <item><c><see cref="SystemType.Char"/>, <see cref="SystemType.String"/></c></item>
		/// </list>
		/// </summary>
		public static bool IsConvertible(this SystemType @this)
			=> @this switch
			{
				SystemType.Boolean
				or SystemType.SByte or SystemType.Byte
				or SystemType.Int16 or SystemType.UInt16
				or SystemType.Int32 or SystemType.UInt32
				or SystemType.NInt or SystemType.NUInt
				or SystemType.Int64 or SystemType.UInt64
				or SystemType.Single or SystemType.Double or SystemType.Decimal
				or SystemType.Char or SystemType.String => true,
				_ => false
			};

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.SystemType.IsConvertible()
		/// || (@<paramref name="this"/>.SystemType is <see cref="SystemType.Nullable"/> &amp;&amp; @<paramref name="this"/>.EnclosedType?.SystemType.IsConvertible() is <see langword="true"/>)
		/// </code>
		/// </summary>
		public static bool IsConvertible(this TypeMember @this)
			=> @this.SystemType.IsConvertible() || (@this.SystemType is SystemType.Nullable && @this.EnclosedType?.SystemType.IsConvertible() is true);

		/// <summary>
		/// <code>
		/// ((<see cref="Type"/>)@<paramref name="this"/>).IsSubclassOf((<see cref="Type"/>)<paramref name="typeMember"/>)
		/// </code>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsSubclassOf(this TypeMember @this, TypeMember typeMember)
			=> ((Type)@this).IsSubclassOf((Type)typeMember);

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.Supports(<paramref name="type"/>.GetTypeMember()
		/// </code>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool Supports(this TypeMember @this, Type type)
			=> @this.Supports(type.GetTypeMember());

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.Handle.Equals(<paramref name="typeMember"/>.Handle)
		/// || <paramref name="typeMember"/>.IsSubclassOf(@<paramref name="this"/>)
		/// || (@<paramref name="this"/>.IsConvertible() &amp;&amp; <paramref name="typeMember"/>.IsConvertible())
		/// </code>
		/// </summary>
		public static bool Supports(this TypeMember @this, TypeMember typeMember)
			=> @this.Handle.Equals(typeMember.Handle) || typeMember.IsSubclassOf(@this) || (@this.IsConvertible() && typeMember.IsConvertible());
	}
}
