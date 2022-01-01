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
	/// <code>
	/// @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> => TypeOf&lt;<typeparamref name="T"/>&gt;.Member,<br/>
	///	<see langword="    "/><see cref="Type"/> type => type.TypeHandle.GetTypeMember(),<br/>
	///	<see langword="    "/><see cref="MemberInfo"/> memberInfo => memberInfo.DeclaringType!.TypeHandle.GetTypeMember(),<br/>
	///	<see langword="    "/>_ => @<paramref name="this"/>.GetType().TypeHandle.GetTypeMember()<br/>
	/// }<br/>
	/// </code>
	/// </summary>
	public static TypeMember GetTypeMember<T>(this T @this)
		=> @this switch
		{
			null => TypeOf<T>.Member,
			Type type => type.TypeHandle.GetTypeMember(),
			MemberInfo memberInfo => memberInfo.DeclaringType!.TypeHandle.GetTypeMember(),
			_ => @this.GetType().TypeHandle.GetTypeMember()
		};

	internal static bool IsCallableWith(this IEnumerable<MethodParameter> @this, params object?[]? arguments)
	{
		if (!arguments.Any())
			return !@this.Any() || @this.All(parameter => parameter!.HasDefaultValue || parameter.IsOptional);

		var argumentEnumerator = arguments.GetEnumerator();
		return @this.All(parameter => argumentEnumerator.TryNext(out var argument)
			? (argument is not null ? parameter.Type.Supports(argument.GetType()) : parameter.Type.Nullable)
			: parameter.HasDefaultValue || parameter.IsOptional) && !argumentEnumerator.MoveNext();
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
