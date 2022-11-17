// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("TypeOf<{Name,nq}>", Name = "TypeOf<{Name,nq}>")]
public sealed class TypeMember : IMember, IEquatable<TypeMember>
{
	static TypeMember()
	{
		Cache = new LazyDictionary<RuntimeTypeHandle, TypeMember>(typeHandle => new TypeMember(typeHandle.ToType()));
	}

	internal static IReadOnlyDictionary<RuntimeTypeHandle, TypeMember> Cache { get; }

	internal TypeMember(Type type)
	{
		this.Attributes = type.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.BaseTypeHandle = type.BaseType?.TypeHandle;
		this.ElementTypeHandle = type.HasElementType ? type.GetElementType()?.TypeHandle : null;
		this.GenericHandle = type.IsGenericType && !type.IsGenericTypeDefinition ? type.GetGenericTypeDefinition().TypeHandle : null;
		this.GenericTypeHandles = type.GenericTypeArguments.Any() ? type.GenericTypeArguments.Map(_ => _.TypeHandle).ToImmutableArray() : ImmutableArray<RuntimeTypeHandle>.Empty;
		this.InterfaceTypeHandles = type.GetInterfaces().Any() ? type.GetInterfaces().Map(_ => _.TypeHandle).ToImmutableArray() : ImmutableArray<RuntimeTypeHandle>.Empty;
		this.Internal = !type.IsVisible;
		this.Kind = type.GetKind();
		this.Name = type.Name();
		this.Namespace = type.Namespace!;
		this.SystemType = type.GetSystemType();
		this.Nullable = this.SystemType is SystemType.Nullable || this.Kind is not Kind.Struct;
		this.Public = type.IsPublic;
		this.Ref = type.IsByRef || type.IsByRefLike;
		this.TypeHandle = type.TypeHandle;
 
		var anyConstructors = type.GetConstructors(INSTANCE_BINDING_FLAGS).Any(constructorInfo => constructorInfo.IsInvokable());
		if (anyConstructors)
			this.Constructors = type.GetConstructors(INSTANCE_BINDING_FLAGS)
				.If(constructorInfo => constructorInfo.IsInvokable())
				.Map(constructorInfo => new ConstructorMember(constructorInfo, this))
				.ToImmutableArray();
		else
			this.Constructors = ImmutableArray<ConstructorMember>.Empty;

		if (!anyConstructors && this.Kind is Kind.Struct && this.SystemType is not SystemType.Void)
			this._CreateValueType = type.New().As<object>().Lambda<Func<object>>().Compile();
		else
			this._CreateValueType = null;

		if (type.GetEvents(BINDING_FLAGS).Any())
			this.Events = type.GetEvents(BINDING_FLAGS)
				.Map(eventInfo => new EventMember(eventInfo, this))
				.ToImmutableArray();
		else
			this.Events = ImmutableArray<EventMember>.Empty;

		this.Fields = this.Kind switch
		{
			Kind.Delegate => ImmutableArray<FieldMember>.Empty,
			_ when type.GetFields(BINDING_FLAGS).Any(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike) =>
				type.GetFields(BINDING_FLAGS)
					.If(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike)
					.Map(fieldInfo => new FieldMember(fieldInfo, this))
					.ToImmutableArray(),
			_ => ImmutableArray<FieldMember>.Empty
		};

		if (type.GetMethods(BINDING_FLAGS).Any(methodInfo => !methodInfo.IsSpecialName && methodInfo.IsInvokable()))
			this.Methods = type.GetMethods(BINDING_FLAGS)
				.If(methodInfo => !methodInfo.IsSpecialName && methodInfo.IsInvokable())
				.Map(methodInfo => new MethodMember(methodInfo, this))
				.ToImmutableArray();
		else
			this.Methods = ImmutableArray<MethodMember>.Empty;

		if (type.GetProperties(BINDING_FLAGS).Any(propertyInfo => propertyInfo.PropertyType.IsInvokable()))
			this.Properties = type.GetProperties(BINDING_FLAGS)
				.If(propertyInfo => propertyInfo.PropertyType.IsInvokable())
				.Map(propertyInfo => new PropertyMember(propertyInfo, this))
				.ToImmutableArray();
		else
			this.Properties = ImmutableArray<PropertyMember>.Empty;
	}

	private readonly Func<object>? _CreateValueType;

	/// <inheritdoc/>
	public IReadOnlyList<Attribute> Attributes { get; }

	public TypeMember? BaseType => this.BaseTypeHandle.HasValue ? this.BaseTypeHandle.Value.GetTypeMember() : null;

	public RuntimeTypeHandle? BaseTypeHandle { get; }

	public IReadOnlyList<ConstructorMember> Constructors { get; }

	public TypeMember? ElementType => this.ElementTypeHandle.HasValue ? this.ElementTypeHandle.Value.GetTypeMember() : null;

	public RuntimeTypeHandle? ElementTypeHandle { get; }

	public IReadOnlyList<EventMember> Events { get; }

	public IReadOnlyList<FieldMember> Fields { get; }

	public RuntimeTypeHandle? GenericHandle { get; }

	public IReadOnlyList<RuntimeTypeHandle> GenericTypeHandles { get; }

	public IReadOnlyList<TypeMember> GenericTypes => this.GenericTypeHandles.Map(handle => handle.GetTypeMember());

	public IReadOnlyList<RuntimeTypeHandle> InterfaceTypeHandles { get; }

	public IReadOnlyList<TypeMember> InterfaceTypes => this.InterfaceTypeHandles.Map(handle => handle.GetTypeMember());

	/// <inheritdoc cref="Type.IsVisible"/>
	public bool Internal { get; }

	public Kind Kind { get; }

	public IReadOnlyList<MethodMember> Methods { get; }

	/// <inheritdoc/>
	public string Name { get; }

	public string Namespace { get; }

	public bool Nullable { get; }

	public IReadOnlyList<PropertyMember> Properties { get; }

	/// <inheritdoc cref="Type.IsPublic"/>
	public bool Public { get; }

	public bool Ref { get; }

	public SystemType SystemType { get; }

	public RuntimeTypeHandle TypeHandle { get; }

	/// <summary>
	/// <code>
	/// =&gt; <see langword="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/>_ <see langword="when this"/>.Constructors.IfFirst(constructor =&gt; constructor.Parameters.IsCallableWith(<paramref name="parameters"/>), <see langword="out var"/> constructor) =&gt; constructor.Create(<paramref name="parameters"/>),<br/>
	/// <see langword="    "/>{ Kind: <see cref="Kind.Struct"/> } <see langword="when this"/>._CreateValueType <see langword="is not null"/> &amp;&amp; !<paramref name="parameters"/>.Any() =&gt; <see langword="this"/>._CreateValueType(),<br/>
	/// <see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? Create(params object?[]? parameters)
		=> this switch
		{
			_ when this.Constructors.IfFirst(constructor => constructor.Parameters.IsCallableWith(parameters), out var constructor) => constructor.Create(parameters),
			{ Kind: Kind.Struct } when this._CreateValueType is not null && !parameters.Any() => this._CreateValueType(),
			_ => null
		};

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Constructors.Map(constructor =&gt; constructor.Method).First&lt;<typeparamref name="D"/>&gt;();</c>
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
		=> this.Constructors.First(constructor => constructor.MethodHandle == handle);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Fields.First(field =&gt; field.Name.Is(<paramref name="name"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public FieldMember? GetField(string name, StringComparison comparison = StringComparison.Ordinal)
		=> this.Fields.First(field => field.Name.Is(name, comparison));

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Methods.Get(<paramref name="name"/>).IfFirst(<see langword="out var"/> methods) ? methods.If(method =&gt; method.Static == <paramref name="isStatic"/>).Map(method =&gt; method!.Method).First&lt;<typeparamref name="D"/>&gt;() : <see langword="null"/>;</c>
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
	/// <see langword="    var"/> foundMethodToInvoke = <see langword="this"/>.Methods<br/>
	/// <see langword="        "/>.If(method =&gt; !method.Static &amp;&amp; method.Name.Is(name, <paramref name="comparison"/>) &amp;&amp; method.Parameters.IsCallableWith(parameters))<br/>
	/// <see langword="        "/>.First();<br/>
	/// <see langword="    "/>foundMethodToInvoke.AssertNotNull();<br/>
	/// <see langword="    return"/> foundMethodToInvoke.Invoke(<paramref name="arguments"/>);<br/>
	/// }
	/// </code>
	/// </summary>
	/// <remarks>First item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	/// <exception cref="ArgumentException"></exception>
	public object? InvokeMethod(string name, object?[]? arguments, StringComparison comparison = StringComparison.Ordinal)
	{
		var foundMethodToInvoke = this.Methods
			.If(method => method.Name.Is(name, comparison) && method!.Parameters.IsCallableWith(!method.Static ? arguments.Skip(1).ToArray() : arguments))
			.First();
		foundMethodToInvoke.AssertNotNull();
		return foundMethodToInvoke.Invoke(arguments);
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> genericMethod = <see langword="this"/>.Methods<br/>
	/// <see langword="        "/>.If(genericMethod =&gt; !method.Static &amp;&amp; method.Name.Is(name, <paramref name="comparison"/>) &amp;&amp; method!.Parameters.IsCallableWith(parameters))<br/>
	/// <see langword="        "/>.First();<br/>
	/// <see langword="    "/>genericMethod.AssertNotNull();<br/>
	/// <see langword="    return"/> genericMethod.Invoke(<paramref name="genericTypes"/>, <paramref name="arguments"/>);<br/>
	/// }
	/// </code>
	/// </summary>
	/// <remarks>First item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	/// <exception cref="ArgumentException"></exception>
	public object? InvokeGenericMethod(string name, Type[] genericTypes, object?[]? arguments, StringComparison comparison = StringComparison.Ordinal)
	{
		var genericMethod = this.Methods
			.If(method => method.Name.Is(name, comparison) && method!.Parameters.IsCallableWith(!method.Static ? arguments.Skip(1).ToArray() : arguments))
			.First();
		genericMethod.AssertNotNull();
		return genericMethod.Invoke(genericTypes, arguments);
	}

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Handle == <paramref name="other"/>?.Handle;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(TypeMember? other)
		=> this.TypeHandle == other?.TypeHandle;

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
		=> this.TypeHandle.GetHashCode();

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.Handle.ToType();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static implicit operator Type(TypeMember member)
		=> member.TypeHandle.ToType();
}
