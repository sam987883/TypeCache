// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public static class ReflectionExtensions
{
	public static string AssemblyName(this Type @this)
		=> @this.Assembly.GetName().Name ?? string.Empty;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlySet<Attribute> Attributes(this Type @this)
		=> TypeStore.Attributes[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ClrType ClrType(this Type @this)
		=> TypeStore.ClrTypes[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string CodeName(this Type @this)
		=> TypeStore.CodeNames[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static CollectionType CollectionType(this Type @this)
		=> TypeStore.CollectionTypes[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ConstructorSet Constructors(this Type @this)
		=> TypeStore.Constructors[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<string, FieldEntity> Fields(this Type @this)
		=> TypeStore.Fields[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<string, GenericMethodSet> GenericMethods(this Type @this)
		=> TypeStore.GenericMethods[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlySet<RuntimeTypeHandle> Interfaces(this Type @this)
		=> TypeStore.Interfaces[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<string, MethodSet> Methods(this Type @this)
		=> TypeStore.Methods[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ObjectType ObjectType(this Type @this)
		=> TypeStore.ObjectTypes[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<string, PropertyEntity> Properties(this Type @this)
		=> TypeStore.Properties[@this.TypeHandle];

	public static ScalarType ScalarType(this Type @this)
		=> @this.IsEnum ? Reflection.ScalarType.Enum : TypeStore.ScalarTypes.GetValue(@this.TypeHandle, Reflection.ScalarType.None);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<string, StaticFieldEntity> StaticFields(this Type @this)
		=> TypeStore.StaticFields[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<string, StaticGenericMethodSet> StaticGenericMethods(this Type @this)
		=> TypeStore.StaticGenericMethods[@this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<string, StaticMethodSet> StaticMethods(this Type @this)
		=> TypeStore.StaticMethods[@this.TypeHandle];

	/// <exception cref="MissingMethodException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? Create(this Type @this)
		=> @this.Constructors().Invoke();

	/// <param name="arguments">Constructor parameter arguments</param>
	/// <exception cref="MissingMethodException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? Create(this Type @this, object?[] arguments)
		=> @this.Constructors().Invoke(arguments);

	/// <param name="arguments">Constructor parameter arguments</param>
	/// <exception cref="MissingMethodException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? Create(this Type @this, ITuple arguments)
		=> @this.Constructors().Invoke(arguments);

	/// <remarks>
	/// <c>@<paramref name="this"/>.ReturnType.IsAny([<see langword="typeof"/>(Task), <see langword="typeof"/>(ValueTask), <see langword="typeof"/>(void)]);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasReturnValue(this MethodInfo @this)
		=> !@this.ReturnType.IsAny([typeof(Task), typeof(ValueTask), typeof(void)]);

	/// <inheritdoc cref="Type.IsAssignableTo(Type)"/>
	/// <remarks>
	/// Supports generic type definitions as well.  For example:
	/// <list type="table">
	/// <item><c><see langword="typeof"/>(List&lt;Int32&gt;).Implements(<see langword="typeof"/>(IList&lt;Int32&gt;)) // <see langword="true"/></c></item>
	/// <item><c><see langword="typeof"/>(List&lt;String&gt;).Implements(<see langword="typeof"/>(IList&lt;&gt;)) // <see langword="true"/></c></item>
	/// <item><c><see langword="typeof"/>(List&lt;&gt;).Implements(<see langword="typeof"/>(IList&lt;Int32&gt;)) // <see langword="false"/></c></item>
	/// </list>
	/// </remarks>
	public static bool Implements(this Type @this, Type type)
	{
		return type switch
		{
			_ when @this.Is(type) => true,
			{ IsArray: true } or { IsPointer: true } or { IsSealed: true } or { IsValueType: true } => false,
			{ IsGenericTypeDefinition: false } => @this.IsAssignableTo(type),
			{ IsInterface: true } => type.IsAny(@this.GetInterfaces().Where(_ => _.IsGenericType).ToArray()),
			_ => isDescendantOf(@this.BaseType, type)
		};

		static bool isDescendantOf(Type? baseType, Type type)
		{
			while (baseType is not null)
			{
				if (baseType.Is(type))
					return true;

				baseType = baseType.BaseType;
			}

			return false;
		}
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> == <see langword="typeof"/>(<typeparamref name="T"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Is<T>(this Type @this)
		=> @this == typeof(T);

	/// <remarks>
	/// Supports generic type definitions as well.  For example:
	/// <list type="table">
	/// <item><c><see langword="typeof"/>(List&lt;Int32&gt;).Is(<see langword="typeof"/>(List&lt;Int32&gt;)) // <see langword="true"/></c></item>
	/// <item><c><see langword="typeof"/>(List&lt;Int32&gt;).Is(<see langword="typeof"/>(IList&lt;Int32&gt;)) // <see langword="false"/></c></item>
	/// <item><c><see langword="typeof"/>(List&lt;String&gt;).Is(<see langword="typeof"/>(List&lt;&gt;)) // <see langword="true"/></c></item>
	/// <item><c><see langword="typeof"/>(List&lt;&gt;).Is(<see langword="typeof"/>(List&lt;Int32&gt;)) // <see langword="true"/></c></item>
	/// </list>
	/// </remarks>
	public static bool Is(this Type @this, Type type)
		=> (@this, type) switch
		{
			({ IsGenericType: true, IsGenericTypeDefinition: false }, { IsGenericTypeDefinition: true })
				=> @this.GetGenericTypeDefinition() == type,
			({ IsGenericTypeDefinition: true }, { IsGenericType: true, IsGenericTypeDefinition: false })
				=> @this == type.GetGenericTypeDefinition(),
			_ => @this == type
		};

	/// <summary>
	/// <c>=&gt; <paramref name="types"/>.Any(@<paramref name="this"/>.Is);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny(this Type @this, Type[] types)
		=> types.Any(@this.Is);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2>(this Type @this)
		=> @this.IsAny([typeof(T1), typeof(T2)]);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3>(this Type @this)
		=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3)]);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4>(this Type @this)
		=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3), typeof(T4)]);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4, T5>(this Type @this)
		=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)]);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>),
	/// <see langword="typeof"/>(<typeparamref name="T6"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4, T5, T6>(this Type @this)
		=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)]);

	/// <inheritdoc cref="Type.IsAssignableFrom(Type?)"/>
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAssignableFrom(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAssignableFrom<T>(this Type @this)
		=> @this.IsAssignableFrom(typeof(T));

	/// <inheritdoc cref="Type.IsAssignableTo(Type?)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsAssignableTo(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAssignableTo<T>(this Type @this)
		=> @this.IsAssignableTo(typeof(T));

	/// <summary>
	/// Whether the type implements <c><see cref="IConvertible"/></c>.<br/>
	/// If the type is <c><see cref="Nullable{T}"/></c>, then whether the type parameter <c>T</c> implements <c><see cref="IConvertible"/></c>.
	/// </summary>
	[DebuggerHidden]
	public static bool IsConvertible(this Type @this)
		=> @this.IsAssignableTo<IConvertible>()
			|| (@this.Is(typeof(Nullable<>)) && @this.GenericTypeArguments.First().IsAssignableTo<IConvertible>());

	/// <exception cref="ArgumentNullException"/>
	public static bool IsConvertibleTo(this Type @this, Type targetType)
	{
		@this.ThrowIfNull();
		targetType.ThrowIfNull();

		return @this.ScalarType().IsConvertibleTo(targetType.ScalarType());
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAssignableTo&lt;<see cref="System.Collections.IEnumerable"/>&gt;();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsEnumerable(this Type @this)
		=> @this.IsAssignableTo<System.Collections.IEnumerable>();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAssignableTo&lt;<see cref="IEnumerable{T}"/>&gt;();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsEnumerableOf<T>(this Type @this)
		=> @this.IsAssignableTo<IEnumerable<T>>();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsPointer &amp;&amp; !@<paramref name="this"/>.IsByRef &amp;&amp; !@<paramref name="this"/>.IsByRefLike;</c>
	/// </remarks>
	[DebuggerHidden]
	internal static bool IsInvokable(this Type @this)
		=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

	/// <remarks>
	/// <c>=&gt; ((<see cref="MethodBase"/>)@<paramref name="this"/>).IsInvokable() &amp;&amp; @<paramref name="this"/>.ReturnType.IsInvokable();</c>
	/// </remarks>
	[DebuggerHidden]
	internal static bool IsInvokable(this MethodInfo @this)
		=> @this.GetParameters().All(parameterInfo => !parameterInfo.IsOut && parameterInfo.ParameterType.IsInvokable())
			&& @this.ReturnType.IsInvokable();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsClass || @<paramref name="this"/>.IsPointer || @<paramref name="this"/>.Is(<see langword="typeof"/>(Nullable&lt;&gt;));</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsNullable(this Type @this)
		=> @this.IsClass || @this.IsPointer || @this.Is(typeof(Nullable<>));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>),
	/// <see langword="typeof"/>(<typeparamref name="T6"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5, T6>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>),
	/// <see langword="typeof"/>(<typeparamref name="T6"/>),
	/// <see langword="typeof"/>(<typeparamref name="T7"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5, T6, T7>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

	public static ConstructorEntity? ToConstructorEntity(this ConstructorInfo @this)
		=> @this.DeclaringType?.Constructors().FirstOrDefault(_ => _.Handle == @this.MethodHandle);

	public static FieldEntity? ToFieldEntity(this FieldInfo @this)
		=> @this.DeclaringType?.Fields().Values.FirstOrDefault(_ => _.Handle == @this.FieldHandle);

	public static MethodEntity? ToMethodEntity(this MethodInfo @this)
		=> !@this.IsGenericMethod && !@this.IsStatic ? @this.DeclaringType?.Methods()[@this.Name].FirstOrDefault(_ => _.Handle == @this.MethodHandle) : null;

	public static StaticFieldEntity? ToStaticFieldEntity(this FieldInfo @this)
		=> @this.DeclaringType?.StaticFields().Values.FirstOrDefault(_ => _.Handle == @this.FieldHandle);

	public static StaticMethodEntity? ToStaticMethodEntity(this MethodInfo @this)
		=> !@this.IsGenericMethod && @this.IsStatic ? @this.DeclaringType?.StaticMethods()[@this.Name].FirstOrDefault(_ => _.Handle == @this.MethodHandle) : null;

	/// <inheritdoc cref="FieldInfo.GetFieldFromHandle(RuntimeFieldHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="FieldInfo"/>.GetFieldFromHandle(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static FieldInfo ToFieldInfo(this RuntimeFieldHandle @this)
		=> FieldInfo.GetFieldFromHandle(@this);

	/// <inheritdoc cref="FieldInfo.GetFieldFromHandle(RuntimeFieldHandle, RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="FieldInfo"/>.GetFieldFromHandle(@<paramref name="this"/>, <paramref name="typeHandle"/>);</c>
	/// </remarks>
	/// <param name="typeHandle"><see cref="RuntimeTypeHandle"/> is needed when <see cref="RuntimeFieldHandle"/> is a field whose type is a generic parameter of its declared type.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static FieldInfo ToFieldInfo(this RuntimeFieldHandle @this, RuntimeTypeHandle typeHandle)
		=> FieldInfo.GetFieldFromHandle(@this, typeHandle);

	/// <inheritdoc cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@<paramref name="this"/>)
	/// ?? <see langword="throw new"/> MissingMethodException(Invariant($"MethodBase.GetMethodFromHandle({@<paramref name="this"/>}) returned null, method may depend on generic class parameter."));</c>
	/// </remarks>
	/// <exception cref="MissingMethodException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodBase ToMethodBase(this RuntimeMethodHandle @this)
		=> MethodBase.GetMethodFromHandle(@this) ?? throw new MissingMethodException(Invariant($"MethodBase.GetMethodFromHandle({@this}) returned null, method may depend on generic class parameter."));

	/// <inheritdoc cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle, RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@<paramref name="this"/>, <paramref name="typeHandle"/>)
	/// ?? <see langword="throw new"/> MissingMethodException(Invariant($"MethodBase.GetMethodFromHandle({@<paramref name="this"/>}, {<paramref name="typeHandle"/>}) returned null."));</c>
	/// </remarks>
	/// <param name="typeHandle"><see cref="RuntimeTypeHandle"/> is needed when <see cref="RuntimeMethodHandle"/> is a method or constructor using a generic parameter of its declared type.</param>
	/// <exception cref="UnreachableException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodBase ToMethodBase(this RuntimeMethodHandle @this, RuntimeTypeHandle typeHandle)
		=> MethodBase.GetMethodFromHandle(@this, typeHandle) ?? throw new UnreachableException(Invariant($"MethodBase.GetMethodFromHandle({@this}, {typeHandle}) returned null."));

	/// <inheritdoc cref="Type.GetTypeFromHandle(RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Type"/>.GetTypeFromHandle(@<paramref name="this"/>)
	/// ?? <see langword="throw new"/> UnreachableException("Type.GetTypeFromHandle(@<paramref name="this"/>) returned null.");</c>
	/// </remarks>
	/// <exception cref="UnreachableException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	[return: NotNull]
	public static Type ToType(this RuntimeTypeHandle @this)
		=> Type.GetTypeFromHandle(@this) ?? throw new UnreachableException(Invariant($"Type.GetTypeFromHandle({@this}) returned null."));
}
