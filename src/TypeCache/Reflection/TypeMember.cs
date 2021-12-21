// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

public readonly struct TypeMember
	: IMember, IEquatable<TypeMember>
{
	static TypeMember()
	{
		Cache = new LazyDictionary<RuntimeTypeHandle, TypeMember>(typeHandle => new TypeMember(typeHandle.ToType()));
	}

	internal static IReadOnlyDictionary<RuntimeTypeHandle, TypeMember> Cache { get; }

	internal TypeMember(Type type)
	{
		var kind = type.GetKind();
		var systemType = type.GetSystemType();

		this.Attributes = type.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.Name = this.Attributes.First<NameAttribute>()?.Name ?? type.Name;
		this.Handle = type.TypeHandle;
		this.Kind = kind;
		this.Nullable = kind is not Kind.Struct || systemType is SystemType.Nullable;
		this.Ref = type.IsByRef || type.IsByRefLike;
		this.SystemType = systemType;
		this.Internal = !type.IsVisible;
		this.Public = type.IsPublic;

		this._BaseType = new Lazy<TypeMember>(() => kind switch
		{
			Kind.Enum => typeof(Enum).GetTypeMember(),
			Kind.Struct or Kind.Pointer => typeof(ValueType).GetTypeMember(),
			_ when type.BaseType is not null => type.BaseType.GetTypeMember(),
			_ => typeof(object).GetTypeMember()
		}, false);
		this._EnclosedType = new Lazy<TypeMember?>(() => systemType switch
		{
			_ when type.HasElementType => type.GetElementType()?.GetTypeMember(),
			SystemType.Dictionary or SystemType.ImmutableDictionary or SystemType.ImmutableSortedDictionary or SystemType.SortedDictionary
				=> typeof(KeyValuePair<,>).MakeGenericType(type.GenericTypeArguments).GetTypeMember(),
			_ when type.GenericTypeArguments.Length == 1 => type.GenericTypeArguments[0].GetTypeMember(),
			_ => null
		}, false);
		this._GenericTypes = new Lazy<IImmutableList<TypeMember>>(() => type.GenericTypeArguments.To(_ => _.GetTypeMember()).ToImmutableArray(), false);
		this._InterfaceTypes = new Lazy<IImmutableList<TypeMember>>(() => type.GetInterfaces().To(_ => _.GetTypeMember()).ToImmutableArray(), false);

		this._Constructors = new Lazy<IImmutableList<ConstructorMember>>(() => type.TypeHandle.CreateConstructorMembers().ToImmutableArray(), false);
		this._Events = new Lazy<IImmutableDictionary<string, EventMember>>(() => type.TypeHandle.CreateEventMembers().ToImmutableDictionary(), false);
		this._Fields = this.Kind switch
		{
			Kind.Delegate => new Lazy<IImmutableDictionary<string, FieldMember>>(() => ImmutableDictionary<string, FieldMember>.Empty, false),
			_ => new Lazy<IImmutableDictionary<string, FieldMember>>(() => type.TypeHandle.CreateFieldMembers().ToImmutableDictionary(), false)
		};
		this._Methods = new Lazy<IImmutableDictionary<string, IImmutableList<MethodMember>>>(() =>
			type.TypeHandle.CreateMethodMembers().ToDictionary(pair => pair.Key, pair => (IImmutableList<MethodMember>)pair.Value.ToImmutableArray()).ToImmutableDictionary(), false);
		this._Properties = new Lazy<IImmutableDictionary<string, PropertyMember>>(() => type.TypeHandle.CreatePropertyMembers().ToImmutableDictionary(), false);
	}

	private readonly Lazy<TypeMember> _BaseType;

	private readonly Lazy<TypeMember?> _EnclosedType;

	private readonly Lazy<IImmutableList<TypeMember>> _GenericTypes;

	private readonly Lazy<IImmutableList<TypeMember>> _InterfaceTypes;

	private readonly Lazy<IImmutableList<ConstructorMember>> _Constructors;

	private readonly Lazy<IImmutableDictionary<string, EventMember>> _Events;

	private readonly Lazy<IImmutableDictionary<string, FieldMember>> _Fields;

	private readonly Lazy<IImmutableDictionary<string, IImmutableList<MethodMember>>> _Methods;

	private readonly Lazy<IImmutableDictionary<string, PropertyMember>> _Properties;

	public IImmutableList<Attribute> Attributes { get; }

	public string Name { get; }

	public IImmutableList<ConstructorMember> Constructors => this._Constructors.Value;

	public IImmutableDictionary<string, EventMember> Events => this._Events.Value;

	public IImmutableDictionary<string, FieldMember> Fields => this._Fields.Value;

	public IImmutableDictionary<string, IImmutableList<MethodMember>> Methods => this._Methods.Value;

	public IImmutableDictionary<string, PropertyMember> Properties => this._Properties.Value;

	public TypeMember BaseType => this._BaseType.Value;

	public TypeMember? EnclosedType => this._EnclosedType.Value;

	public IImmutableList<TypeMember> GenericTypes => this._GenericTypes.Value;

	public RuntimeTypeHandle Handle { get; }

	public IImmutableList<TypeMember> InterfaceTypes => this._InterfaceTypes.Value;

	public Kind Kind { get; }

	public bool Nullable { get; }

	public bool Ref { get; }

	public SystemType SystemType { get; }

	public bool Internal { get; }

	public bool Public { get; }

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<see langword="this"/>.Constructors.TryFirst(constructor =&gt; constructor.Parameters.IsCallableWith(<paramref name="parameters"/>), <see langword="out var"/> constructor))<br/>
	/// <see langword="    return"/> constructor.Create(<paramref name="parameters"/>);<br/>
	/// <see langword="throw new"/> <see cref="ArgumentException"/>($"{<see langword="this"/>.Name}.{<see langword="nameof"/>(Create)}(...): no constructor found that takes the {<paramref name="parameters"/>?.Length ?? 0} provided {<see langword="nameof"/>(<paramref name="parameters"/>)}.");
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object Create(params object?[]? parameters)
	{
		if (this.Constructors.TryFirst(constructor => constructor.Parameters.IsCallableWith(parameters), out var constructor))
			return constructor.Create(parameters);
		throw new ArgumentException($"{this.Name}.{nameof(Create)}(...): no constructor found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
	}

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Constructors.To(constructor =&gt; constructor.Method).First&lt;<typeparamref name="D"/>&gt;();</c>
	/// </summary>
	public D? GetConstructor<D>()
		where D : Delegate
		=> this.Constructors.To(constructor => constructor.Method).First<D>();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Methods.Get(<paramref name="name"/>).TryFirst(<see langword="out var"/> methods) ? methods.If(method =&gt; method.Static == <paramref name="isStatic"/>).To(method =&gt; method!.Method).First&lt;<typeparamref name="D"/>&gt;() : <see langword="null"/>;</c>
	/// </summary>
	public D? GetMethod<D>(string name, bool isStatic = false)
		where D : Delegate
		=> this.Methods.Get(name).TryFirst(out var methods) ? methods.If(method => method.Static == isStatic).To(method => method!.Method).First<D>() : null;

	/// <summary>
	/// <c>=&gt; ((<see cref="Type"/>)<see langword="this"/>).Implements&lt;<typeparamref name="T"/>&gt;();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Implements<T>()
		=> ((Type)this).Implements<T>();

	/// <summary>
	/// <c>=&gt; ((<see cref="Type"/>)<see langword="this"/>).Implements(<paramref name="type"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Implements(Type type)
		=> ((Type)this).Implements(type);

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<see langword="this"/>.Methods.Get(<paramref name="name"/>).TryFirst(<see langword="out var"/> methods)<br/>
	/// <see langword="    "/>&amp;&amp; methods.TryFirst(method =&gt; !method.IsStatic &amp;&amp; method.Parameters.IsCallableWith(<paramref name="parameters"/>), <see langword="out var"/> method))<br/>
	/// <see langword="    return"/> method.Invoke(<paramref name="instance"/>, <paramref name="parameters"/>);<br/>
	/// <see langword="throw new"/> <see cref="ArgumentException"/>($"{<see langword="this"/>.Name}.{<see langword="nameof"/>(InvokeMethod)}(...): no method found that takes the {<paramref name="parameters"/>?.Length ?? 0} provided {<see langword="nameof"/>(<paramref name="parameters"/>)}.");
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? InvokeMethod(string name, object instance, params object?[]? parameters)
	{
		if (this.Methods.Get(name).TryFirst(out var methods)
			&& methods.TryFirst(method => !method.Static && method!.Parameters.IsCallableWith(parameters), out var method))
			return method.Invoke(instance, parameters);
		throw new ArgumentException($"{this.Name}.{nameof(InvokeMethod)}(...): no method found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<see langword="this"/>.Methods.Get(<paramref name="name"/>).TryFirst(<see langword="out var"/> methods)<br/>
	/// <see langword="    "/>&amp;&amp; methods.TryFirst(method =&gt; method.IsStatic &amp;&amp; method.Parameters.IsCallableWith(<paramref name="parameters"/>), <see langword="out var"/> method))<br/>
	/// <see langword="    return"/> method.Invoke(<see langword="null"/>, <paramref name="parameters"/>);<br/>
	/// <see langword="throw new"/> <see cref="ArgumentException"/>($"{<see langword="this"/>.Name}.{<see langword="nameof"/>(InvokeStaticMethod)}(...): no method found that takes the {<paramref name="parameters"/>?.Length ?? 0} provided {<see langword="nameof"/>(<paramref name="parameters"/>)}.");
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? InvokeStaticMethod(string name, params object?[]? parameters)
	{
		if (this.Methods.Get(name).TryFirst(out var methods)
			&& methods.TryFirst(method => method.Static && method.Parameters.IsCallableWith(parameters), out var method))
			return method.Invoke(null, parameters);
		throw new ArgumentException($"{this.Name}.{nameof(InvokeStaticMethod)}(...): no method found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<see langword="this"/>.Methods.Get(<paramref name="name"/>).TryFirst(<see langword="out var"/> methods)<br/>
	/// <see langword="    "/>&amp;&amp; methods.TryFirst(method =&gt; !method.IsStatic &amp;&amp; method.Parameters.IsCallableWith(<paramref name="parameters"/>), <see langword="out var"/> method))<br/>
	/// <see langword="    return"/> method.InvokeGeneric(<paramref name="instance"/>, <paramref name="genericTypes"/>, <paramref name="parameters"/>);<br/>
	/// <see langword="throw new"/> <see cref="ArgumentException"/>($"{<see langword="this"/>.Name}.{<see langword="nameof"/>(InvokeGenericMethod)}(...): no method found that takes the {<paramref name="parameters"/>?.Length ?? 0} provided {<see langword="nameof"/>(<paramref name="parameters"/>)}.");
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? InvokeGenericMethod(string name, Type[] genericTypes, object instance, params object?[]? parameters)
	{
		if (this.Methods.Get(name).TryFirst(out var methods)
			&& methods.TryFirst(method => !method.Static && method!.Parameters.IsCallableWith(parameters), out var method))
			return method.InvokeGeneric(instance, genericTypes, parameters);
		throw new ArgumentException($"{this.Name}.{nameof(InvokeGenericMethod)}(...): no method found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<see langword="this"/>.Methods.Get(<paramref name="name"/>).TryFirst(<see langword="out var"/> methods)<br/>
	/// <see langword="    "/>&amp;&amp; methods.TryFirst(method =&gt; method.IsStatic &amp;&amp; method.Parameters.IsCallableWith(<paramref name="parameters"/>), <see langword="out var"/> method))<br/>
	/// <see langword="    return"/> method.InvokeGeneric(<see langword="null"/>, <paramref name="genericTypes"/>, <paramref name="parameters"/>);<br/>
	/// <see langword="throw new"/> <see cref="ArgumentException"/>($"{<see langword="this"/>.Name}.{<see langword="nameof"/>(InvokeGenericStaticMethod)}(...): no method found that takes the {<paramref name="parameters"/>?.Length ?? 0} provided {<see langword="nameof"/>(<paramref name="parameters"/>)}.");
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? InvokeGenericStaticMethod(string name, Type[] genericTypes, params object?[]? parameters)
	{
		if (this.Methods.Get(name).TryFirst(out var methods)
			&& methods.TryFirst(method => method.Static && method!.Parameters.IsCallableWith(parameters), out var method) is true)
			return method.InvokeGeneric(null, genericTypes, parameters);
		throw new ArgumentException($"{this.Name}.{nameof(InvokeGenericMethod)}(...): no method found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
	}

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Handle.Is&lt;<typeparamref name="V"/>&gt;();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Is<V>()
		=> this.Handle.Is<V>();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Handle.Is(<paramref name="type"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Is(Type type)
		=> this.Handle.Is(type);

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.Handle.ToType();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static implicit operator Type(TypeMember member)
		=> member.Handle.ToType();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Handle.Equals(<paramref name="other"/>.Handle);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Equals(TypeMember other)
		=> this.Handle.Equals(other.Handle);

	/// <summary>
	/// <c>=&gt; <paramref name="value"/> <see langword="is"/> <see cref="TypeMember"/> typeMember
	/// ? <see langword="this"/>.Handle.Equals(typeMember.Handle)
	/// : <see langword="false"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public override bool Equals(object? value)
		=> value is TypeMember typeMember ? this.Handle.Equals(typeMember.Handle) : false;

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Handle.GetHashCode();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public override int GetHashCode()
		=> this.Handle.GetHashCode();

	/// <summary>
	/// <c>=&gt; <paramref name="typeMember1"/>.Equals(<paramref name="typeMember2"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool operator ==(TypeMember typeMember1, TypeMember typeMember2)
		=> typeMember1.Equals(typeMember2);

	/// <summary>
	/// <c>=&gt; !<paramref name="typeMember1"/>.Equals(<paramref name="typeMember2"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool operator !=(TypeMember typeMember1, TypeMember typeMember2)
		=> !typeMember1.Equals(typeMember2);
}
