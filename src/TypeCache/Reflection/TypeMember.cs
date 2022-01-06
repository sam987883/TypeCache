// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static System.Threading.LazyThreadSafetyMode;
using static TypeCache.Default;

namespace TypeCache.Reflection;

public class TypeMember : Member, IEquatable<TypeMember>
{
	static TypeMember()
	{
		Cache = new LazyDictionary<RuntimeTypeHandle, TypeMember>(typeHandle => new TypeMember(typeHandle.ToType()));
	}

	internal static IReadOnlyDictionary<RuntimeTypeHandle, TypeMember> Cache { get; }

	internal TypeMember(Type type) : base(type)
	{
		this.GenericHandle = type.IsGenericType && !type.IsGenericTypeDefinition ? type.GetGenericTypeDefinition().TypeHandle : null;
		this.Handle = type.TypeHandle;
		this.Kind = type.GetKind();
		this.SystemType = type.GetSystemType();
		this.Nullable = this.SystemType is SystemType.Nullable || this.Kind is not Kind.Struct;
		this.Ref = type.IsByRef || type.IsByRefLike;

		this._BaseType = type.BaseType is not null ? new Lazy<TypeMember?>(() => this.Handle.ToType().BaseType.GetTypeMember(), ExecutionAndPublication) : Lazy.Null<TypeMember>();
		this._ElementType = type.HasElementType ? new Lazy<TypeMember?>(() => this.Handle.ToType().GetElementType()!.GetTypeMember(), ExecutionAndPublication) : Lazy.Null<TypeMember>();

		if (type.GenericTypeArguments.Any())
			this._GenericTypes = new Lazy<IImmutableList<TypeMember>>(() =>
				this.Handle.ToType().GenericTypeArguments
					.Map(_ => _.GetTypeMember())
					.ToImmutableArray(), ExecutionAndPublication);
		else
			this._GenericTypes = Lazy.Value<IImmutableList<TypeMember>>(ImmutableArray<TypeMember>.Empty);
 
		if (type.GetInterfaces().Any())
			this._InterfaceTypes = new Lazy<IImmutableList<TypeMember>>(() =>
				this.Handle.ToType().GetInterfaces()
					.Map(_ => _.GetTypeMember())
					.ToImmutableArray(), ExecutionAndPublication);
		else
			this._InterfaceTypes = Lazy.Value<IImmutableList<TypeMember>>(ImmutableArray<TypeMember>.Empty);

		if (type.GetConstructors(INSTANCE_BINDING_FLAGS).Any(constructorInfo => constructorInfo.IsInvokable()))
			this._Constructors = new Lazy<IImmutableList<ConstructorMember>>(() =>
				this.Handle.ToType().GetConstructors(INSTANCE_BINDING_FLAGS)
					.If(constructorInfo => constructorInfo.IsInvokable())
					.Map(constructorInfo => new ConstructorMember(constructorInfo, this))
					.ToImmutableArray(), ExecutionAndPublication);
		else
			this._Constructors = Lazy.Value<IImmutableList<ConstructorMember>>(ImmutableArray<ConstructorMember>.Empty);

		if (type.GetEvents(BINDING_FLAGS).Any())
			this._Events = new Lazy<IImmutableDictionary<string, EventMember>>(() =>
				this.Handle.ToType().GetEvents(BINDING_FLAGS)
					.Map(eventInfo => KeyValuePair.Create(eventInfo.Name, new EventMember(eventInfo, this)))
					.ToImmutableDictionary(NAME_STRING_COMPARISON), ExecutionAndPublication);
		else
			this._Events = Lazy.Value<IImmutableDictionary<string, EventMember>>(ImmutableDictionary<string, EventMember>.Empty);

		this._Fields = this.Kind switch
		{
			Kind.Delegate => Lazy.Value<IImmutableDictionary<string, FieldMember>>(ImmutableDictionary<string, FieldMember>.Empty),
			_ when type.GetFields(BINDING_FLAGS).Any(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike) =>
				new Lazy<IImmutableDictionary<string, FieldMember>>(() =>
					this.Handle.ToType().GetFields(BINDING_FLAGS)
						.If(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike)
						.Map(fieldInfo => KeyValuePair.Create(fieldInfo.Name, new FieldMember(fieldInfo, this)))
						.ToImmutableDictionary(NAME_STRING_COMPARISON), ExecutionAndPublication),
			_ => Lazy.Value<IImmutableDictionary<string, FieldMember>>(ImmutableDictionary<string, FieldMember>.Empty)
		};

		if (this.Handle.ToType().GetMethods(BINDING_FLAGS).Any(methodInfo => !methodInfo.IsSpecialName && methodInfo.IsInvokable()))
			this._Methods = new Lazy<IImmutableDictionary<string, IImmutableList<MethodMember>>>(() =>
				this.Handle.ToType().GetMethods(BINDING_FLAGS)
					.If(methodInfo => !methodInfo.IsSpecialName && methodInfo.IsInvokable())
					.Map(methodInfo => new MethodMember(methodInfo, this))
					.Group(method => method.Name, NAME_STRING_COMPARISON.ToStringComparer())
					.ToImmutableDictionary(_ => _.Key, _ => (IImmutableList<MethodMember>)_.Value.ToImmutableArray(), NAME_STRING_COMPARISON), ExecutionAndPublication);
		else
			this._Methods = Lazy.Value<IImmutableDictionary<string, IImmutableList<MethodMember>>>(ImmutableDictionary<string, IImmutableList<MethodMember>>.Empty);

		if (this.Handle.ToType().GetProperties(BINDING_FLAGS).Any(propertyInfo => propertyInfo.PropertyType.IsInvokable()))
			this._Properties = new Lazy<IImmutableDictionary<string, PropertyMember>>(() =>
				this.Handle.ToType().GetProperties(BINDING_FLAGS)
					.If(propertyInfo => propertyInfo.PropertyType.IsInvokable())
					.Map(propertyInfo => KeyValuePair.Create(propertyInfo.Name, new PropertyMember(propertyInfo, this)))
					.ToImmutableDictionary(NAME_STRING_COMPARISON), ExecutionAndPublication);
		else
			this._Properties = Lazy.Value<IImmutableDictionary<string, PropertyMember>>(ImmutableDictionary<string, PropertyMember>.Empty);
	}

	private readonly Lazy<TypeMember?> _BaseType;

	private readonly Lazy<TypeMember?> _ElementType;

	private readonly Lazy<IImmutableList<TypeMember>> _GenericTypes;

	private readonly Lazy<IImmutableList<TypeMember>> _InterfaceTypes;

	private readonly Lazy<IImmutableList<ConstructorMember>> _Constructors;

	private readonly Lazy<IImmutableDictionary<string, EventMember>> _Events;

	private readonly Lazy<IImmutableDictionary<string, FieldMember>> _Fields;

	private readonly Lazy<IImmutableDictionary<string, IImmutableList<MethodMember>>> _Methods;

	private readonly Lazy<IImmutableDictionary<string, PropertyMember>> _Properties;

	public IImmutableList<ConstructorMember> Constructors => this._Constructors.Value;

	public IImmutableDictionary<string, EventMember> Events => this._Events.Value;

	public IImmutableDictionary<string, FieldMember> Fields => this._Fields.Value;

	public IImmutableDictionary<string, IImmutableList<MethodMember>> Methods => this._Methods.Value;

	public IImmutableDictionary<string, PropertyMember> Properties => this._Properties.Value;

	public TypeMember? BaseType => this._BaseType.Value;

	public TypeMember? ElementType => this._ElementType.Value;

	public RuntimeTypeHandle? GenericHandle { get; }

	public IImmutableList<TypeMember> GenericTypes => this._GenericTypes.Value;

	public RuntimeTypeHandle Handle { get; }

	public IImmutableList<TypeMember> InterfaceTypes => this._InterfaceTypes.Value;

	public Kind Kind { get; }

	public bool Nullable { get; }

	public bool Ref { get; }

	public SystemType SystemType { get; }

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
		=> this.Constructors.Map(constructor => constructor.Method).First<D>();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Constructors.First(constructor =&gt; constructor.Handle == <paramref name="handle"/>);</c>
	/// </summary>
	public ConstructorMember? GetConstructorMember(RuntimeMethodHandle handle)
		=> this.Constructors.First(constructor => constructor.Handle == handle);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Methods.Get(<paramref name="name"/>).TryFirst(<see langword="out var"/> methods) ? methods.If(method =&gt; method.Static == <paramref name="isStatic"/>).To(method =&gt; method!.Method).First&lt;<typeparamref name="D"/>&gt;() : <see langword="null"/>;</c>
	/// </summary>
	public D? GetMethod<D>(string name, bool isStatic = false)
		where D : Delegate
		=> this.Methods.Get(name).TryFirst(out var methods) ? methods.If(method => method.Static == isStatic).Map(method => method!.Method).First<D>() : null;

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Methods.Values.Gather().First(method =&gt; method.Handle == <paramref name="handle"/>);</c>
	/// </summary>
	public MethodMember? GetMethodMember(RuntimeMethodHandle handle)
		=> this.Methods.Values.Gather().First(method => method.Handle == handle);

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<see langword="this"/>.Methods.Get(<paramref name="name"/>).TryFirst(<see langword="out var"/> methods)<br/>
	/// <see langword="    "/>&amp;&amp; methods.TryFirst(method =&gt; !method.IsStatic &amp;&amp; method.Parameters.IsCallableWith(<paramref name="parameters"/>), <see langword="out var"/> method))<br/>
	/// <see langword="    return"/> method.Invoke(<paramref name="instance"/>, <paramref name="parameters"/>);<br/>
	/// <see langword="throw new"/> <see cref="ArgumentException"/>($"{<see langword="this"/>.Name}.{<see langword="nameof"/>(<see cref="InvokeMethod"/>)}(...): no method found that takes the {<paramref name="parameters"/>?.Length ?? 0} provided {<see langword="nameof"/>(<paramref name="parameters"/>)}.");
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
	/// <see langword="throw new"/> <see cref="ArgumentException"/>($"{<see langword="this"/>.Name}.{<see langword="nameof"/>(<see cref="InvokeStaticMethod"/>)}(...): no method found that takes the {<paramref name="parameters"/>?.Length ?? 0} provided {<see langword="nameof"/>(<paramref name="parameters"/>)}.");
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
	/// <see langword="throw new"/> <see cref="ArgumentException"/>($"{<see langword="this"/>.Name}.{<see langword="nameof"/>(<see cref="InvokeGenericMethod"/>)}(...): no method found that takes the {<paramref name="parameters"/>?.Length ?? 0} provided {<see langword="nameof"/>(<paramref name="parameters"/>)}.");
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
	/// <see langword="throw new"/> <see cref="ArgumentException"/>($"{<see langword="this"/>.Name}.{<see langword="nameof"/>(<see cref="InvokeGenericStaticMethod"/>)}(...): no method found that takes the {<paramref name="parameters"/>?.Length ?? 0} provided {<see langword="nameof"/>(<paramref name="parameters"/>)}.");
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
	/// <c>=&gt; <see langword="this"/>.Handle == <paramref name="other"/>?.Handle;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Equals(TypeMember? other)
		=> this.Handle == other?.Handle;

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.Handle.ToType();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static implicit operator Type(TypeMember member)
		=> member.Handle.ToType();
}
