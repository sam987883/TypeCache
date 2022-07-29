// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("TypeOf<{Name,nq}>", Name = "TypeOf<{Name,nq}>")]
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
		this.Namespace = type.Namespace!;
		this.SystemType = type.GetSystemType();
		this.Nullable = this.SystemType is SystemType.Nullable || this.Kind is not Kind.Struct;
		this.Ref = type.IsByRef || type.IsByRefLike;

#nullable disable
		this._BaseType = type.BaseType is not null ? Lazy.Create(() => this.Handle.ToType().BaseType.GetTypeMember()) : Lazy.Null<TypeMember>();
		this._ElementType = type.HasElementType ? Lazy.Create(() => this.Handle.ToType().GetElementType()!.GetTypeMember()) : Lazy.Null<TypeMember>();
#nullable enable

		if (type.GenericTypeArguments.Any())
			this._GenericTypes = Lazy.Create<IReadOnlyList<TypeMember>>(() =>
				this.Handle.ToType().GenericTypeArguments.Map(_ => _.GetTypeMember()).ToImmutableArray());
		else
			this._GenericTypes = Lazy.Value<IReadOnlyList<TypeMember>>(ImmutableArray<TypeMember>.Empty);
 
		if (type.GetInterfaces().Any())
			this._InterfaceTypes = Lazy.Create<IReadOnlyList<TypeMember>>(() =>
				this.Handle.ToType().GetInterfaces().Map(_ => _.GetTypeMember()).ToImmutableArray());
		else
			this._InterfaceTypes = Lazy.Value<IReadOnlyList<TypeMember>>(ImmutableArray<TypeMember>.Empty);

		var anyConstructors = type.GetConstructors(INSTANCE_BINDING_FLAGS).Any(constructorInfo => constructorInfo.IsInvokable());
		if (anyConstructors)
			this._Constructors = Lazy.Create<IReadOnlyList<ConstructorMember>>(() =>
				this.Handle.ToType().GetConstructors(INSTANCE_BINDING_FLAGS)
					.If(constructorInfo => constructorInfo.IsInvokable())
					.Map(constructorInfo => new ConstructorMember(constructorInfo, this))
					.ToImmutableArray());
		else
			this._Constructors = Lazy.Value<IReadOnlyList<ConstructorMember>>(ImmutableArray<ConstructorMember>.Empty);

		if (!anyConstructors && this.Kind is Kind.Struct && this.SystemType is not SystemType.Void)
			this._CreateValueType = type.New().As<object>().Lambda<Func<object>>().Compile();
		else
			this._CreateValueType = null;

		if (type.GetEvents(BINDING_FLAGS).Any())
			this._Events = Lazy.Create<IReadOnlyList<EventMember>>(() =>
				this.Handle.ToType().GetEvents(BINDING_FLAGS)
					.Map(eventInfo => new EventMember(eventInfo, this))
					.ToImmutableArray());
		else
			this._Events = Lazy.Value<IReadOnlyList<EventMember>>(ImmutableArray<EventMember>.Empty);

		this._Fields = this.Kind switch
		{
			Kind.Delegate => Lazy.Value<IReadOnlyList<FieldMember>>(ImmutableArray<FieldMember>.Empty),
			_ when type.GetFields(BINDING_FLAGS).Any(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike) =>
				Lazy.Create<IReadOnlyList<FieldMember>>(() =>
					this.Handle.ToType().GetFields(BINDING_FLAGS)
						.If(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike)
						.Map(fieldInfo => new FieldMember(fieldInfo, this))
						.ToImmutableArray()),
			_ => Lazy.Value<IReadOnlyList<FieldMember>>(ImmutableArray<FieldMember>.Empty)
		};

		if (this.Handle.ToType().GetMethods(BINDING_FLAGS).Any(methodInfo => !methodInfo.IsSpecialName && methodInfo.IsInvokable()))
			this._Methods = Lazy.Create<IReadOnlyList<MethodMember>>(() =>
				this.Handle.ToType().GetMethods(BINDING_FLAGS)
					.If(methodInfo => !methodInfo.IsSpecialName && methodInfo.IsInvokable())
					.Map(methodInfo => new MethodMember(methodInfo, this))
					.ToImmutableArray());
		else
			this._Methods = Lazy.Value<IReadOnlyList<MethodMember>>(ImmutableArray<MethodMember>.Empty);

		if (this.Handle.ToType().GetProperties(BINDING_FLAGS).Any(propertyInfo => propertyInfo.PropertyType.IsInvokable()))
			this._Properties = Lazy.Create<IReadOnlyList<PropertyMember>>(() =>
				this.Handle.ToType().GetProperties(BINDING_FLAGS)
					.If(propertyInfo => propertyInfo.PropertyType.IsInvokable())
					.Map(propertyInfo => new PropertyMember(propertyInfo, this))
					.ToImmutableArray());
		else
			this._Properties = Lazy.Value<IReadOnlyList<PropertyMember>>(ImmutableArray<PropertyMember>.Empty);
	}

	private readonly Lazy<TypeMember?> _BaseType;

	private readonly Func<object>? _CreateValueType;

	private readonly Lazy<TypeMember?> _ElementType;

	private readonly Lazy<IReadOnlyList<TypeMember>> _GenericTypes;

	private readonly Lazy<IReadOnlyList<TypeMember>> _InterfaceTypes;

	private readonly Lazy<IReadOnlyList<ConstructorMember>> _Constructors;

	private readonly Lazy<IReadOnlyList<EventMember>> _Events;

	private readonly Lazy<IReadOnlyList<FieldMember>> _Fields;

	private readonly Lazy<IReadOnlyList<MethodMember>> _Methods;

	private readonly Lazy<IReadOnlyList<PropertyMember>> _Properties;

	public IReadOnlyList<ConstructorMember> Constructors => this._Constructors.Value;

	public IReadOnlyList<EventMember> Events => this._Events.Value;

	public IReadOnlyList<FieldMember> Fields => this._Fields.Value;

	public IReadOnlyList<MethodMember> Methods => this._Methods.Value;

	public IReadOnlyList<PropertyMember> Properties => this._Properties.Value;

	public TypeMember? BaseType => this._BaseType.Value;

	public TypeMember? ElementType => this._ElementType.Value;

	public RuntimeTypeHandle? GenericHandle { get; }

	public IReadOnlyList<TypeMember> GenericTypes => this._GenericTypes.Value;

	public RuntimeTypeHandle Handle { get; }

	public IReadOnlyList<TypeMember> InterfaceTypes => this._InterfaceTypes.Value;

	public Kind Kind { get; }

	public string Namespace { get; }

	public bool Nullable { get; }

	public bool Ref { get; }

	public SystemType SystemType { get; }

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<see langword="this"/>.Constructors.IfFirst(constructor =&gt; constructor.Parameters.IsCallableWith(<paramref name="parameters"/>), <see langword="out var"/> constructor))<br/>
	/// <see langword="    return"/> constructor.Create(<paramref name="parameters"/>);<br/>
	/// <see langword="else if"/> (<see langword="this"/>.Kind <see langword="is"/> <see cref="Kind.Struct"/> &amp;&amp; <see langword="this"/>._CreateValueType <see langword="is not null"/> &amp;&amp; !<paramref name="parameters"/>.Any())<br/>
	/// <see langword="    return"/> <see langword="this"/>._CreateValueType();<br/>
	/// <see langword="return null"/>;<br/>
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? Create(params object?[]? parameters)
	{
		if (this.Constructors.IfFirst(constructor => constructor.Parameters.IsCallableWith(parameters), out var constructor))
			return constructor.Create(parameters);
		else if (this.Kind is Kind.Struct && this._CreateValueType is not null && !parameters.Any())
			return this._CreateValueType();
		return null;
	}

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Constructors.To(constructor =&gt; constructor.Method).First&lt;<typeparamref name="D"/>&gt;();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public D? GetConstructor<D>()
		where D : Delegate
		=> this.Constructors.Map(constructor => constructor.Method).First<D>();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Constructors.First(constructor =&gt; constructor.Handle == <paramref name="handle"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public ConstructorMember? GetConstructor(RuntimeMethodHandle handle)
		=> this.Constructors.First(constructor => constructor.Handle == handle);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Fields.First(field =&gt; field.Name.Is(<paramref name="name"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public FieldMember? GetField(string name, StringComparison comparison = StringComparison.Ordinal)
		=> this.Fields.First(field => field.Name.Is(name, comparison));

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Methods.Get(<paramref name="name"/>).IfFirst(<see langword="out var"/> methods) ? methods.If(method =&gt; method.Static == <paramref name="isStatic"/>).To(method =&gt; method!.Method).First&lt;<typeparamref name="D"/>&gt;() : <see langword="null"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public D? GetMethod<D>(string name, bool isStatic = false, StringComparison comparison = StringComparison.Ordinal)
		where D : Delegate
		=> this.Methods.If(method => method.Static == isStatic && method.Name.Is(name, comparison)).Map(method => method!.Method).First<D>();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Methods.Values.Gather().First(method =&gt; method.Handle == <paramref name="handle"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public MethodMember? GetMethod(RuntimeMethodHandle handle)
		=> this.Methods.First(method => method.Handle == handle);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Properties.First(property =&gt; property.Name.Is(<paramref name="name"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public PropertyMember? GetProperty(string name, StringComparison comparison = StringComparison.Ordinal)
		=> this.Properties.First(property => property.Name.Is(name, comparison));

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> method = <see langword="this"/>.Methods<br/>
	/// <see langword="        "/>.If(method =&gt; !method.Static &amp;&amp; method.Name.Is(name, <paramref name="comparison"/>) &amp;&amp; method!.Parameters.IsCallableWith(parameters))<br/>
	/// <see langword="        "/>.First();<br/>
	/// <see langword="    "/>method.AssertNotNull();<br/>
	/// <see langword="    return"/> method.Invoke(<paramref name="instance"/>, <paramref name="parameters"/>);<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? InvokeMethod(string name, object instance, object?[]? parameters, StringComparison comparison = StringComparison.Ordinal)
	{
		var method = this.Methods
			.If(method => !method.Static && method.Name.Is(name, comparison) && method!.Parameters.IsCallableWith(parameters))
			.First();
		method.AssertNotNull();
		return method.Invoke(instance, parameters);
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> staticMethod = <see langword="this"/>.Methods<br/>
	/// <see langword="        "/>.If(staticMethod =&gt; method.Static &amp;&amp; method.Name.Is(name, <paramref name="comparison"/>) &amp;&amp; method!.Parameters.IsCallableWith(parameters))<br/>
	/// <see langword="        "/>.First();<br/>
	/// <see langword="    "/>staticMethod.AssertNotNull();<br/>
	/// <see langword="    return"/> staticMethod.Invoke(<paramref name="parameters"/>);<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? InvokeStaticMethod(string name, object?[]? parameters, StringComparison comparison = StringComparison.Ordinal)
	{
		var staticMethod = this.Methods
			.If(method => method.Static && method.Name.Is(name, comparison) && method!.Parameters.IsCallableWith(parameters))
			.First();
		staticMethod.AssertNotNull();
		return staticMethod.Invoke(parameters);
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> genericMethod = <see langword="this"/>.Methods<br/>
	/// <see langword="        "/>.If(genericMethod =&gt; !method.Static &amp;&amp; method.Name.Is(name, <paramref name="comparison"/>) &amp;&amp; method!.Parameters.IsCallableWith(parameters))<br/>
	/// <see langword="        "/>.First();<br/>
	/// <see langword="    "/>genericMethod.AssertNotNull();<br/>
	/// <see langword="    return"/> genericMethod.Invoke(<paramref name="instance"/>, <paramref name="genericTypes"/>, <paramref name="parameters"/>);<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? InvokeGenericMethod(string name, Type[] genericTypes, object instance, object?[]? parameters, StringComparison comparison = StringComparison.Ordinal)
	{
		var genericMethod = this.Methods
			.If(method => !method.Static && method.Name.Is(name, comparison) && method!.Parameters.IsCallableWith(parameters))
			.First();
		genericMethod.AssertNotNull();
		return genericMethod.Invoke(instance, genericTypes, parameters);
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<see langword="this"/>.Methods.Get(<paramref name="name"/>).IfFirst(<see langword="out var"/> methods)<br/>
	/// <see langword="    "/>&amp;&amp; methods.IfFirst(method =&gt; method.IsStatic &amp;&amp; method.Parameters.IsCallableWith(<paramref name="parameters"/>), <see langword="out var"/> method))<br/>
	/// <see langword="    return"/> method.InvokeGeneric(<see langword="null"/>, <paramref name="genericTypes"/>, <paramref name="parameters"/>);<br/>
	/// <see langword="throw new"/> <see cref="ArgumentException"/>($"{<see langword="this"/>.Name}.{<see langword="nameof"/>(<see cref="InvokeGenericStaticMethod"/>)}(...): no method found that takes the {<paramref name="parameters"/>?.Length ?? 0} provided {<see langword="nameof"/>(<paramref name="parameters"/>)}.");
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? InvokeGenericStaticMethod(string name, Type[] genericTypes, object?[]? parameters, StringComparison comparison = StringComparison.Ordinal)
	{
		var genericStaticMethod = this.Methods
			.If(method => method.Static && method.Name.Is(name, comparison) && method!.Parameters.IsCallableWith(parameters))
			.First();
		genericStaticMethod.AssertNotNull();
		return genericStaticMethod.Invoke(genericTypes, parameters);
	}

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Handle == <paramref name="other"/>?.Handle;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(TypeMember? other)
		=> this.Handle == other?.Handle;

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Equals(<paramref name="item"/> <see langword="as"/> <see cref="TypeMember"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as TypeMember);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Handle.GetHashCode();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Handle.GetHashCode();

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.Handle.ToType();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static implicit operator Type(TypeMember member)
		=> member.Handle.ToType();
}
