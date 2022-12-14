// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Collections;
using TypeCache.Extensions;

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
		const BindingFlags BINDING_FLAGS = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
		const BindingFlags INSTANCE_BINDING_FLAGS = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		this.Attributes = type.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.BaseTypeHandle = type.BaseType?.TypeHandle;
		this.ElementTypeHandle = type.HasElementType ? type.GetElementType()?.TypeHandle : null;
		this.GenericHandle = type.IsGenericType && !type.IsGenericTypeDefinition ? type.GetGenericTypeDefinition().TypeHandle : null;
		this.GenericTypeHandles = type.GenericTypeArguments.Any() ? type.GenericTypeArguments.Select(_ => _.TypeHandle).ToImmutableArray() : ImmutableArray<RuntimeTypeHandle>.Empty;
		this.InterfaceTypeHandles = type.GetInterfaces().Any() ? type.GetInterfaces().Select(_ => _.TypeHandle).ToImmutableArray() : ImmutableArray<RuntimeTypeHandle>.Empty;
		this.Internal = !type.IsVisible;
		this.Kind = type.GetKind();
		this.Name = type.Name();
		this.Namespace = type.Namespace!;
		this.ObjectType = type.GetObjectType();
		this.SystemType = type.GetSystemType();
		this.Nullable = this.SystemType is SystemType.Nullable || this.Kind is not Kind.Struct;
		this.Public = type.IsPublic;
		this.Ref = type.IsByRef || type.IsByRefLike;
		this.TypeHandle = type.TypeHandle;
 
		var anyConstructors = type.GetConstructors(INSTANCE_BINDING_FLAGS).Any(constructorInfo => constructorInfo.IsInvokable());
		if (anyConstructors)
			this.Constructors = type.GetConstructors(INSTANCE_BINDING_FLAGS)
				.Where(constructorInfo => constructorInfo.IsInvokable())
				.Select(constructorInfo => new ConstructorMember(constructorInfo, this))
				.ToImmutableArray();
		else
			this.Constructors = ImmutableArray<ConstructorMember>.Empty;

		if (!anyConstructors && this.Kind is Kind.Struct && this.SystemType is not SystemType.Void)
			this._CreateValueType = type.New().As<object>().Lambda<Func<object>>().Compile();
		else
			this._CreateValueType = null;

		if (type.GetEvents(BINDING_FLAGS).Any())
			this.Events = type.GetEvents(BINDING_FLAGS).Select(eventInfo => new EventMember(eventInfo, this))
				.ToImmutableArray();
		else
			this.Events = ImmutableArray<EventMember>.Empty;

		this.Fields = this.ObjectType switch
		{
			ObjectType.Delegate => ImmutableArray<FieldMember>.Empty,
			_ when type.GetFields(BINDING_FLAGS).Any(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike) =>
				type.GetFields(BINDING_FLAGS)
					.Where(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike)
					.Select(fieldInfo => new FieldMember(fieldInfo, this))
					.ToImmutableArray(),
			_ => ImmutableArray<FieldMember>.Empty
		};

		if (type.GetMethods(BINDING_FLAGS).Any(methodInfo => !methodInfo.IsSpecialName && methodInfo.IsInvokable()))
			this.Methods = type.GetMethods(BINDING_FLAGS)
				.Where(methodInfo => !methodInfo.IsSpecialName && methodInfo.IsInvokable())
				.Select(methodInfo => new MethodMember(methodInfo, this))
				.ToImmutableArray();
		else
			this.Methods = ImmutableArray<MethodMember>.Empty;

		if (type.GetProperties(BINDING_FLAGS).Any(propertyInfo => propertyInfo.PropertyType.IsInvokable()))
			this.Properties = type.GetProperties(BINDING_FLAGS)
				.Where(propertyInfo => propertyInfo.PropertyType.IsInvokable())
				.Select(propertyInfo => new PropertyMember(propertyInfo, this))
				.ToImmutableArray();
		else
			this.Properties = ImmutableArray<PropertyMember>.Empty;
	}

	private readonly Func<object>? _CreateValueType;

	public IReadOnlyList<Attribute> Attributes { get; }

	/// <inheritdoc cref="Type.BaseType"/>
	public TypeMember? BaseType => this.BaseTypeHandle.HasValue ? this.BaseTypeHandle.Value.GetTypeMember() : null;

	public RuntimeTypeHandle? BaseTypeHandle { get; }

	public IReadOnlyList<ConstructorMember> Constructors { get; }

	public TypeMember? ElementType => this.ElementTypeHandle.HasValue ? this.ElementTypeHandle.Value.GetTypeMember() : null;

	public RuntimeTypeHandle? ElementTypeHandle { get; }

	public IReadOnlyList<EventMember> Events { get; }

	public IReadOnlyList<FieldMember> Fields { get; }

	public RuntimeTypeHandle? GenericHandle { get; }

	public IReadOnlyList<RuntimeTypeHandle> GenericTypeHandles { get; }

	public IReadOnlyList<TypeMember> GenericTypes => this.GenericTypeHandles.Select(handle => handle.GetTypeMember()).ToArray();

	public IReadOnlyList<RuntimeTypeHandle> InterfaceTypeHandles { get; }

	public IReadOnlyList<TypeMember> InterfaceTypes => this.InterfaceTypeHandles.Select(handle => handle.GetTypeMember()).ToArray();

	/// <inheritdoc cref="Type.IsVisible"/>
	public bool Internal { get; }

	public Kind Kind { get; }

	public IReadOnlyList<MethodMember> Methods { get; }

	public string Name { get; }

	/// <inheritdoc cref="Type.Namespace"/>
	public string Namespace { get; }

	public bool Nullable { get; }

	public ObjectType ObjectType { get; }

	public IReadOnlyList<PropertyMember> Properties { get; }

	/// <inheritdoc cref="Type.IsPublic"/>
	public bool Public { get; }

	/// <inheritdoc cref="Type.IsByRef"/>
	public bool Ref { get; }

	public SystemType SystemType { get; }

	/// <inheritdoc cref="Type.TypeHandle"/>
	public RuntimeTypeHandle TypeHandle { get; }

	/// <summary>
	/// <code>
	/// =&gt; <see langword="this"/>.FindConstructor(<paramref name="parameters"/>) <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/><see langword="null"/> <see langword="when this"/>._CreateValueType <see langword="is not null"/> &amp;&amp; <paramref name="parameters"/>?.Any() <see langword="is not true"/> =&gt; <see langword="this"/>._CreateValueType(),<br/>
	/// <see langword="    "/><see langword="null"/> =&gt; <see langword="throw new"/> <see cref="MissingMethodException"/>(<see langword="this"/>.Name, <see langword="this"/>.Name),<br/>
	/// <see langword="    var"/>_ constructor =&gt; constructor.Create(<paramref name="parameters"/>)<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public object? Create(params object?[]? parameters)
		=> this.FindConstructor(parameters) switch
		{
			null when this._CreateValueType is not null && parameters?.Any() is not true => this._CreateValueType(),
			null => throw new MissingMethodException(this.Name, this.Name),
			var constructor => constructor.Create(parameters)
		};

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Constructors.FirstOrDefault(constructor =&gt; constructor.Parameters.IsCallableWith(<paramref name="arguments"/>));</c>
	/// </summary>
	public ConstructorMember? FindConstructor(params object?[]? arguments)
		=> this.Constructors.FirstOrDefault(constructor => constructor.Parameters.IsCallableWith(arguments));

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Methods.FirstOrDefault(method =&gt; method.Name.Is(<paramref name="name"/>, <paramref name="comparison"/>)<br/>
	/// <see langword="    "/>&amp;&amp; method.Parameters.IsCallableWith(method.Static ? <paramref name="arguments"/> : <paramref name="arguments"/>?.Skip(1).ToArray()));</c>
	/// </summary>
	public MethodMember? FindMethod(string name, object?[]? arguments, StringComparison comparison = StringComparison.Ordinal)
		=> this.Methods.FirstOrDefault(method => method.Name.Is(name, comparison)
			&& method.Parameters.IsCallableWith(method.Static ? arguments : arguments?.Skip(1).ToArray()));

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Constructors.Select(constructor =&gt; constructor.Method).FirstOrDefault&lt;<typeparamref name="D"/>&gt;();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public D? GetConstructor<D>()
		where D : Delegate
		=> this.Constructors.Select(constructor => constructor.Method).FirstOrDefault<D>();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Constructors.FirstOrDefault(constructor =&gt; constructor.Handle == <paramref name="handle"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ConstructorMember? GetConstructor(RuntimeMethodHandle handle)
		=> this.Constructors.FirstOrDefault(constructor => constructor.MethodHandle == handle);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Fields.FirstOrDefault(field =&gt; field.Name.Is(<paramref name="name"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public FieldMember? GetField(string name, StringComparison comparison = StringComparison.Ordinal)
		=> this.Fields.FirstOrDefault(field => field.Name.Is(name, comparison));

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Methods<br/>
	/// <see langword="    "/>.Where(method =&gt; method.Static == <paramref name="isStatic"/> &amp;&amp; method.Name.Is(<paramref name="name"/>, <paramref name="comparison"/>))<br/>
	/// <see langword="    "/>.Select(method =&gt; method.Method)<br/>
	/// <see langword="    "/>.FirstOrDefault&lt;<typeparamref name="D"/>&gt();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public D? GetMethod<D>(string name, bool isStatic = false, StringComparison comparison = StringComparison.Ordinal)
		where D : Delegate
		=> this.Methods
			.Where(method => method.Static == isStatic && method.Name.Is(name, comparison))
			.Select(method => method!.Method)
			.FirstOrDefault<D>();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Methods.FirstOrDefault(method =&gt; method.Handle == <paramref name="handle"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public MethodMember? GetMethod(RuntimeMethodHandle handle)
		=> this.Methods.FirstOrDefault(method => method.Handle == handle);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Properties.FirstOrDefault(property =&gt; property.Name.Is(<paramref name="name"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public PropertyMember? GetProperty(string name, StringComparison comparison = StringComparison.Ordinal)
		=> this.Properties.FirstOrDefault(property => property.Name.Is(name, comparison));

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.FindMethod(<paramref name="name"/>, <paramref name="arguments"/>, <paramref name="comparison"/>)<br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see langword="throw new"/> <see cref="MissingMethodException"/>(<see langword="this"/>.Name, <paramref name="name"/>),<br/>
	/// <see langword="    var"/> method =&gt; method.Invoke(<paramref name="arguments"/>)<br/>
	/// }</c>
	/// </summary>
	/// <remarks>FirstOrDefault item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	/// <exception cref="MissingMethodException"></exception>
	[DebuggerHidden]
	public object? InvokeMethod(string name, object?[]? arguments, StringComparison comparison = StringComparison.Ordinal)
		=> this.FindMethod(name, arguments, comparison) switch
		{
			null => throw new MissingMethodException(this.Name, name),
			var method => method.Invoke(arguments)
		};

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.FindMethod(<paramref name="name"/>, <paramref name="arguments"/>, <paramref name="comparison"/>)<br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see langword="throw new"/> <see cref="MissingMethodException"/>(<see langword="this"/>.Name, <paramref name="name"/>),<br/>
	/// <see langword="    var"/> method =&gt; method.InvokeGeneric(<paramref name="genericTypes"/>, <paramref name="arguments"/>)<br/>
	/// }</c>
	/// </summary>
	/// <remarks>FirstOrDefault item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	/// <exception cref="MissingMethodException"></exception>
	public object? InvokeGenericMethod(string name, Type[] genericTypes, object?[]? arguments, StringComparison comparison = StringComparison.Ordinal)
		=> this.FindMethod(name, arguments, comparison) switch
		{
			null => throw new MissingMethodException(this.Name, name),
			var method => method.InvokeGeneric(genericTypes, arguments)
		};

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Handle == <paramref name="other"/>?.Handle;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals(TypeMember? other)
		=> this.TypeHandle == other?.TypeHandle;

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Equals(<paramref name="item"/> <see langword="as"/> <see cref="TypeMember"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as TypeMember);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Handle.GetHashCode();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override int GetHashCode()
		=> this.TypeHandle.GetHashCode();

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.Handle.ToType();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static implicit operator Type(TypeMember member)
		=> member.TypeHandle.ToType();
}
