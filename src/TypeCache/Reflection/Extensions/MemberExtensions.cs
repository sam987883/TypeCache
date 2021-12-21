// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection.Extensions;

public static class MemberExtensions
{
	/// <summary>
	/// <c><see cref="MethodMember"/>.Cache[(@<paramref name="this"/>.MethodHandle, @<paramref name="this"/>.DeclaringType!.TypeHandle)]</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodMember GetMethodMember(this MethodInfo @this)
		=> MethodMember.Cache[(@this.MethodHandle, @this.DeclaringType!.TypeHandle)];

	/// <summary>
	/// <code>
	/// @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///		<see cref="Type"/> type => type.TypeHandle.GetTypeMember(),<br/>
	///		<see cref="MemberInfo"/> memberInfo => memberInfo.DeclaringType!.TypeHandle.GetTypeMember(),<br/>
	///		<see langword="null"/> => TypeOf&lt;<typeparamref name="T"/>&gt;.Member,<br/>
	///		_ => @<paramref name="this"/>.GetType().TypeHandle.GetTypeMember()<br/>
	/// }<br/>
	/// </code>
	/// </summary>
	public static TypeMember GetTypeMember<T>(this T @this)
		=> @this switch
		{
			Type type => type.TypeHandle.GetTypeMember(),
			MemberInfo memberInfo => memberInfo.DeclaringType!.TypeHandle.GetTypeMember(),
			null => TypeOf<T>.Member,
			_ => @this.GetType().TypeHandle.GetTypeMember()
		};

	internal static bool IsCallableWith(this IEnumerable<MethodParameter> @this, params object?[]? arguments)
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
		return !@this.Any() || @this.All(parameter => parameter!.HasDefaultValue || parameter.IsOptional);
	}

	/// <summary>
	/// <c>@<paramref name="this"/>.Implements&lt;<see cref="IConvertible"/>&gt;()
	/// || (@<paramref name="this"/>.SystemType <see langword="is"/> <see cref="SystemType.Nullable"/>
	/// &amp;&amp; @<paramref name="this"/>.EnclosedType?.Implements&lt;<see cref="IConvertible"/>&gt;() <see langword="is true"/>)</c>
	/// </summary>
	public static bool IsConvertible(this TypeMember @this)
		=> @this.Implements<IConvertible>() || (@this.SystemType is SystemType.Nullable && @this.EnclosedType?.Implements<IConvertible>() is true);

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
