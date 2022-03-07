// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection.Extensions;

public static class MemberExtensions
{
	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/>?.SystemType <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see langword="null"/>,<br/>
	///	<see langword="    "/><see cref="SystemType.Array"/> =&gt; @<paramref name="this"/>.ElementType,<br/>
	///	<see langword="    "/><see cref="SystemType.Dictionary"/> <see langword="or"/> <see cref="SystemType.SortedDictionary"/> <see langword="or"/> <see cref="SystemType.ImmutableDictionary"/> <see langword="or"/> <see cref="SystemType.ImmutableSortedDictionary"/><br/>
	///	<see langword="        "/>=&gt; <see langword="typeof"/>(<see cref="KeyValuePair"/>&lt;,&gt;).MakeGenericType(@<paramref name="this"/>.GenericTypes.To(_ =&gt; (<see cref="Type"/>)_).ToArray()).GetTypeMember(),<br/>
	///	<see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.SystemType.IsCollection() =&gt; @<paramref name="this"/>.GenericTypes.First()<br/>
	///	<see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// };<br/>
	/// </code>
	/// </summary>
	public static TypeMember? CollectionType(this TypeMember? @this)
		=> @this?.SystemType switch
		{
			null => null,
			SystemType.Array => @this.ElementType,
			SystemType.Dictionary or SystemType.SortedDictionary or SystemType.ImmutableDictionary or SystemType.ImmutableSortedDictionary
				=> typeof(KeyValuePair<,>).MakeGenericType(@this.GenericTypes.Map(_ => (Type)_).ToArray()).GetTypeMember(),
			_ when @this.SystemType.IsCollection() => @this.GenericTypes.First(),
			_ => null
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="TypeOf{T}.Member"/>,<br/>
	///	<see langword="    "/><see cref="Type"/> type =&gt; type.TypeHandle.GetTypeMember(),<br/>
	///	<see langword="    "/><see cref="MemberInfo"/> memberInfo =&gt; memberInfo.DeclaringType!.TypeHandle.GetTypeMember(),<br/>
	///	<see langword="    "/>_ =&gt; @<paramref name="this"/>.GetType().TypeHandle.GetTypeMember()<br/>
	/// };<br/>
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

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Implements(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Implements<T>(this TypeMember? @this)
		=> @this.Implements(typeof(T));

	/// <summary>
	/// <code>
	/// <paramref name="type"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	/// <see langword="    return false"/>;<br/>
	/// <br/>
	/// <see langword="var"/> handle = <paramref name="type"/>.TypeHandle;<br/>
	/// <see langword="var"/> baseType = <see langword="this"/>.BaseType;<br/>
	/// <see langword="if"/> (<paramref name="type"/>.IsGenericTypeDefinition)<br/>
	/// {<br/>
	/// <see langword="    if"/> (<paramref name="type"/>.IsInterface &amp;&amp; <see langword="this"/>.InterfaceTypes.Any(_ =&gt; _.GenericHandle.Equals(handle)))<br/>
	/// <see langword="        return true"/>;<br/>
	/// <br/>
	/// <see langword="    while"/> (baseType <see langword="is not null"/>)<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        if"/> (baseType.GenericHandle.Equals(handle))<br/>
	/// <see langword="             return true"/>;<br/>
	/// <see langword="        "/>baseType = baseType.BaseType;<br/>
	/// <see langword="    "/>}<br/>
	/// }<br/>
	/// <see langword="else"/><br/>
	/// {<br/>
	/// <see langword="    if"/> (<paramref name="type"/>.IsInterface &amp;&amp; <see langword="this"/>.InterfaceTypes.Any(_ =&gt; _.Handle.Equals(handle)))<br/>
	/// <see langword="        return true"/>;<br/>
	/// <br/>
	/// <see langword="    while"/> (baseType <see langword="is not null"/>)<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        if"/> (baseType.Handle.Equals(handle))<br/>
	/// <see langword="             return true"/>;<br/>
	/// <see langword="        "/>baseType = baseType.BaseType;<br/>
	/// <see langword="    "/>}<br/>
	/// }<br/>
	/// <see langword="return false"/>;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Implements(this TypeMember? @this, Type type)
	{
		type.AssertNotNull();

		if (@this is null)
			return false;

		var handle = type.TypeHandle;
		var baseType = @this.BaseType;
		if (type.IsGenericTypeDefinition)
		{
			if (type.IsInterface && @this.InterfaceTypes.Any(_ => _.GenericHandle.Equals(handle)))
				return true;

			while (baseType is not null)
			{
				if (baseType.GenericHandle.Equals(handle))
					return true;
				baseType = baseType.BaseType;
			}
		}
		else
		{
			if (type.IsInterface && @this.InterfaceTypes.Any(_ => _.Handle.Equals(handle)))
				return true;

			while (baseType is not null)
			{
				if (baseType.Handle.Equals(handle))
					return true;
				baseType = baseType.BaseType;
			}
		}
		return false;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> &amp;&amp; @<paramref name="this"/>.Handle.Equals(<see langword="typeof"/>(<typeparamref name="V"/>).TypeHandle);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Is<V>(this TypeMember? @this)
		=> @this is not null && @this.Handle.Equals(typeof(V).TypeHandle);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> &amp;&amp;
	/// (@<paramref name="this"/>.Handle.Equals(<paramref name="type"/>.TypeHandle)
	///		|| @<paramref name="this"/>.GenericHandle.Equals(<paramref name="type"/>.TypeHandle));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Is(this TypeMember? @this, Type type)
		=> @this is not null && (@this.Handle.Equals(type.TypeHandle) || @this.GenericHandle.Equals(type.TypeHandle));

	/// <summary>
	/// <code>
	/// <see langword="if"/> (!<paramref name="arguments"/>.Any())<br/>
	/// <see langword="    return"/> !@<paramref name="this"/>.Any() || @<paramref name="this"/>.All(parameter =&gt; parameter!.HasDefaultValue || parameter.IsOptional);<br/>
	/// <br/>
	/// <see langword="var"/> argumentEnumerator = arguments.GetEnumerator();<br/>
	/// <see langword="return"/> @<paramref name="this"/>.All(parameter =&gt; argumentEnumerator.TryNext(<see langword="out var"/> argument)<br/>
	/// <see langword="    "/>? (argument <see langword="is not null"/> ? parameter.Type.Supports(argument.GetType()) : parameter.Type.Nullable)<br/>
	/// <see langword="    "/>: parameter.HasDefaultValue || parameter.IsOptional) &amp;&amp; !argumentEnumerator.MoveNext();
	/// </code>
	/// </summary>
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
	/// <c>=&gt; @<paramref name="this"/>.Implements&lt;<see cref="IConvertible"/>&gt;()
	/// || (@<paramref name="this"/>.SystemType <see langword="is"/> <see cref="SystemType.Nullable"/>
	/// &amp;&amp; @<paramref name="this"/>.GenericTypes.First()!.Implements&lt;<see cref="IConvertible"/>&gt;());</c>
	/// </summary>
	public static bool IsConvertible(this TypeMember @this)
		=> @this.Implements<IConvertible>() || (@this.SystemType is SystemType.Nullable && @this.GenericTypes.First()!.Implements<IConvertible>());

	/// <summary>
	/// <c>=&gt; ((<see cref="Type"/>)@<paramref name="this"/>).IsSubclassOf((<see cref="Type"/>)<paramref name="typeMember"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsSubclassOf(this TypeMember @this, TypeMember typeMember)
		=> ((Type)@this).IsSubclassOf((Type)typeMember);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Supports(<paramref name="type"/>.GetTypeMember();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Supports(this TypeMember @this, Type type)
		=> @this.Supports(type.GetTypeMember());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Handle.Equals(<paramref name="typeMember"/>.Handle)
	/// || <paramref name="typeMember"/>.IsSubclassOf(@<paramref name="this"/>)
	/// || (@<paramref name="this"/>.IsConvertible() &amp;&amp; <paramref name="typeMember"/>.IsConvertible());</c>
	/// </summary>
	public static bool Supports(this TypeMember @this, TypeMember typeMember)
		=> @this.Handle.Equals(typeMember.Handle) || typeMember.IsSubclassOf(@this) || (@this.IsConvertible() && typeMember.IsConvertible());
}
