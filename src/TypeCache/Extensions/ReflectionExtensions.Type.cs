// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Utilities;
using static System.Reflection.BindingFlags;
using static System.StringComparison;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	/// <exception cref="MissingMethodException"></exception>
	public static object? Create(this Type @this, params object?[]? parameters)
		=> @this.FindConstructor(parameters) switch
		{
			null when @this.IsValueType && parameters?.Any() is not true => TypeStore.DefaultValueTypeConstructorInvokes[@this.TypeHandle].Invoke(),
			null => throw new MissingMethodException(@this.Name(), @this.Name),
			var constructorInfo => constructorInfo.InvokeMethod(parameters)
		};

	public static ConstructorInfo? FindConstructor(this Type @this, params object?[]? arguments)
		=> @this.GetConstructors(INSTANCE_BINDING_FLAGS)
			.FirstOrDefault(constructor => constructor.IsCallableWith(arguments));

	public static MethodInfo? FindMethod(this Type @this, string name, Type[] argumentTypes, bool nameIgnoreCase = false)
		=> @this.GetMethod(name, nameIgnoreCase ? INSTANCE_BINDING_FLAGS | IgnoreCase : INSTANCE_BINDING_FLAGS, argumentTypes);

	public static MethodInfo? FindMethod(this Type @this, string name, object?[]? arguments, bool nameIgnoreCase = false)
		=> @this.GetMethods(INSTANCE_BINDING_FLAGS).FirstOrDefault(method =>
			method.Name().Is(name, nameIgnoreCase ? OrdinalIgnoreCase : Ordinal) && method.IsCallableWith(arguments));

	public static MethodInfo? FindStaticMethod(this Type @this, string name, object?[]? arguments, bool nameIgnoreCase = false)
		=> @this.GetMethods(STATIC_BINDING_FLAGS).FirstOrDefault(method =>
			method.Name().Is(name, nameIgnoreCase ? OrdinalIgnoreCase : Ordinal) && method.IsCallableWith(arguments));

	public static object? GetFieldValue(this Type @this, string name, object instance, bool nameIgnoreCase = false)
		=> @this.GetField(name, nameIgnoreCase ? INSTANCE_BINDING_FLAGS | IgnoreCase : INSTANCE_BINDING_FLAGS)?
			.GetValue(instance);

	/// <inheritdoc cref="Type.GetFields(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Instance"/></c>
	/// </remarks>
	/// <param name="includeNonPublic">Includes <c><see cref="NonPublic"/></c> among the bindings.</param>
	/// <param name="nameIgnoreCase">Includes <c><see cref="IgnoreCase"/></c> among the bindings.</param>
	public static FieldInfo[] GetInstanceFields(this Type @this, bool includeNonPublic = false, bool nameIgnoreCase = false)
	{
		var binding = FlattenHierarchy | Public | Instance;
		if (includeNonPublic)
			binding |= NonPublic;
		if (nameIgnoreCase)
			binding |= IgnoreCase;
		return @this.GetFields(binding);
	}

	/// <inheritdoc cref="Type.GetMethods(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Instance"/></c>
	/// </remarks>
	/// <param name="includeNonPublic">Includes <c><see cref="NonPublic"/></c> among the bindings.</param>
	/// <param name="nameIgnoreCase">Includes <c><see cref="IgnoreCase"/></c> among the bindings.</param>
	public static MethodInfo[] GetInstanceMethods(this Type @this, bool includeNonPublic = false, bool nameIgnoreCase = false)
	{
		var binding = FlattenHierarchy | Public | Instance;
		if (includeNonPublic)
			binding |= NonPublic;
		if (nameIgnoreCase)
			binding |= IgnoreCase;
		return @this.GetMethods(binding);
	}

	/// <inheritdoc cref="Type.GetProperties(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Instance"/></c>
	/// </remarks>
	/// <param name="includeNonPublic">Includes <c><see cref="NonPublic"/></c> among the bindings.</param>
	/// <param name="nameIgnoreCase">Includes <c><see cref="IgnoreCase"/></c> among the bindings.</param>
	public static PropertyInfo[] GetInstanceProperties(this Type @this, bool includeNonPublic = false, bool nameIgnoreCase = false)
	{
		var binding = FlattenHierarchy | Public | Instance;
		if (includeNonPublic)
			binding |= NonPublic;
		if (nameIgnoreCase)
			binding |= IgnoreCase;
		return @this.GetProperties(binding);
	}

	/// <inheritdoc cref="Type.GetFields(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/></c>
	/// </remarks>
	/// <param name="includeNonPublic">Includes <c><see cref="NonPublic"/></c> among the bindings.</param>
	/// <param name="nameIgnoreCase">Includes <c><see cref="IgnoreCase"/></c> among the bindings.</param>
	public static FieldInfo[] GetStaticFields(this Type @this, bool includeNonPublic = false, bool nameIgnoreCase = false)
	{
		var binding = FlattenHierarchy | Public | Static;
		if (includeNonPublic)
			binding |= NonPublic;
		if (nameIgnoreCase)
			binding |= IgnoreCase;
		return @this.GetFields(binding);
	}

	/// <inheritdoc cref="Type.GetMethods(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/></c>
	/// </remarks>
	/// <param name="includeNonPublic">Includes <c><see cref="NonPublic"/></c> among the bindings.</param>
	/// <param name="nameIgnoreCase">Includes <c><see cref="IgnoreCase"/></c> among the bindings.</param>
	public static MethodInfo[] GetStaticMethods(this Type @this, bool includeNonPublic = false, bool nameIgnoreCase = false)
	{
		var binding = FlattenHierarchy | Public | Static;
		if (includeNonPublic)
			binding |= NonPublic;
		if (nameIgnoreCase)
			binding |= IgnoreCase;
		return @this.GetMethods(binding);
	}

	/// <inheritdoc cref="Type.GetProperties(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/></c>
	/// </remarks>
	/// <param name="includeNonPublic">Includes <c><see cref="NonPublic"/></c> among the bindings.</param>
	/// <param name="nameIgnoreCase">Includes <c><see cref="IgnoreCase"/></c> among the bindings.</param>
	public static PropertyInfo[] GetStaticProperties(this Type @this, bool includeNonPublic = false, bool nameIgnoreCase = false)
	{
		var binding = FlattenHierarchy | Public | Static;
		if (includeNonPublic)
			binding |= NonPublic;
		if (nameIgnoreCase)
			binding |= IgnoreCase;
		return @this.GetProperties(binding);
	}

	/// <exception cref="ArgumentException"></exception>
	public static Kind GetKind(this Type @this)
		=> @this switch
		{
			{ IsPointer: true } => Kind.Pointer,
			{ IsInterface: true } => Kind.Interface,
			{ IsClass: true } => Kind.Class,
			{ IsValueType: true } => Kind.Struct,
			_ => throw new ArgumentException(Invariant($"{nameof(GetKind)}({nameof(Type)}): [{@this?.Name() ?? "null"}] is not supported."))
		};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ObjectType GetObjectType(this Type @this)
		=> TypeStore.ObjectTypes[@this.TypeHandle];

	public static object? GetPropertyValue(this Type @this, string name, object instance, bool nameIgnoreCase = false, params object?[]? index)
		=> @this.GetProperty(name, nameIgnoreCase ? INSTANCE_BINDING_FLAGS | IgnoreCase : INSTANCE_BINDING_FLAGS)?
			.GetPropertyValue(instance, index);

	public static object? GetStaticFieldValue(this Type @this, string name, bool nameIgnoreCase = false)
		=> @this.GetField(name, nameIgnoreCase ? STATIC_BINDING_FLAGS | IgnoreCase : STATIC_BINDING_FLAGS)?
			.GetFieldValue(null);

	public static object? GetStaticPropertyValue(this Type @this, string name, bool nameIgnoreCase = false, params object?[]? index)
		=> @this.GetProperty(name, nameIgnoreCase ? STATIC_BINDING_FLAGS | IgnoreCase : STATIC_BINDING_FLAGS)?
			.GetPropertyValue(null, index);

	public static SystemType GetSystemType(this Type @this)
		=> @this switch
		{
			{ IsArray: true } => SystemType.Array,
			{ IsEnum: true } => TypeStore.SystemTypes[@this.GetEnumUnderlyingType().TypeHandle],
			_ when TypeStore.SystemTypes.TryGetValue(@this.ToGenericType()?.TypeHandle ?? @this.TypeHandle, out var systemType) => systemType,
			_ => SystemType.None
		};

	public static bool Implements(this Type @this, Type type)
	{
		return type switch
		{
			{ IsGenericTypeDefinition: false } => @this.IsAssignableTo(type),
			{ IsInterface: true } => @this.GetInterfaces().Any(_ => type.Is(_.ToGenericType())),
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

	public static object? InvokeMethod(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.InvokeMethod(arguments?.Any() is true ? arguments.Prepend(instance).ToArray() : new[] { instance });

	public static object? InvokeMethod(this Type @this, string name, Type[] genericTypes, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod(genericTypes)
			.InvokeMethod(arguments?.Any() is true ? arguments.Prepend(instance).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1>()
			.InvokeMethod(arguments?.Any() is true ? arguments.Prepend(instance).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1, T2>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2>()
			.InvokeMethod(arguments?.Any() is true ? arguments.Prepend(instance).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1, T2, T3>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3>()
			.InvokeMethod(arguments?.Any() is true ? arguments.Prepend(instance).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1, T2, T3, T4>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4>()
			.InvokeMethod(arguments?.Any() is true ? arguments.Prepend(instance).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1, T2, T3, T4, T5>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5>()
			.InvokeMethod(arguments?.Any() is true ? arguments.Prepend(instance).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1, T2, T3, T4, T5, T6>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5, T6>()
			.InvokeMethod(arguments?.Any() is true ? arguments.Prepend(instance).ToArray() : new[] { instance });

	public static object? InvokeStaticMethod(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod(this Type @this, string name, Type[] genericTypes, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod(genericTypes)
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1>()
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1, T2>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2>()
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1, T2, T3>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3>()
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1, T2, T3, T4>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4>()
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1, T2, T3, T4, T5>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5>()
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1, T2, T3, T4, T5, T6>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5, T6>()
			.InvokeMethod(null, arguments);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> == <see langword="typeof"/>(<typeparamref name="T"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Is<T>(this Type? @this)
		=> @this == typeof(T);

	[DebuggerHidden]
	public static bool Is(this Type? @this, Type? type)
		=> (@this?.IsGenericTypeDefinition is true || type?.IsGenericTypeDefinition is true) ? @this.ToGenericType() == type.ToGenericType() : @this == type;

	/// <summary>
	/// <c>=&gt; <paramref name="types"/>.Any(@<paramref name="this"/>.Is);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny(this Type @this, params Type[] types)
		=> types.Any(@this.Is);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1>(this Type @this)
		=> @this.IsAny(typeof(T1));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2>(this Type @this)
		=> @this.IsAny(typeof(T1), typeof(T2));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3>(this Type @this)
		=> @this.IsAny(typeof(T1), typeof(T2), typeof(T3));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4>(this Type @this)
		=> @this.IsAny(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4, T5>(this Type @this)
		=> @this.IsAny(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>),
	/// <see langword="typeof"/>(<typeparamref name="T6"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4, T5, T6>(this Type @this)
		=> @this.IsAny(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

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

	[DebuggerHidden]
	public static bool IsConvertible(this Type @this)
		=> @this.IsAssignableTo<IConvertible>()
			|| (@this.ToGenericType() == typeof(Nullable<>) && @this.GenericTypeArguments.First().IsAssignableTo<IConvertible>());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAssignableTo&lt;<see cref="IEnumerable{T}"/>&gt;();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsEnumerableOf<T>(this Type @this)
		=> @this.IsAssignableTo<IEnumerable<T>>();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetSystemType().IsEnumUnderlyingType();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsEnumUnderlyingType(this Type @this)
		=> @this.GetSystemType().IsEnumUnderlyingType();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsPointer &amp;&amp; !@<paramref name="this"/>.IsByRef &amp;&amp; !@<paramref name="this"/>.IsByRefLike;</c>
	/// </remarks>
	[DebuggerHidden]
	internal static bool IsInvokable(this Type @this)
		=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsClass || @<paramref name="this"/>.IsPointer || <see langword="typeof"/>(Nullable&lt;&gt;) == @<paramref name="this"/>.ToGenericType();</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsNullable(this Type @this)
		=> @this.IsClass || @this.IsPointer || typeof(Nullable<>) == @this.ToGenericType();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Is(<paramref name="type"/>) || @<paramref name="this"/>.Implements(<paramref name="type"/>);</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsOrImplements(this Type @this, Type type)
		=> @this.Is(type) || @this.Implements(type);

	[DebuggerHidden]
	public static void SetFieldValue(this Type @this, string name, object instance, object? value, bool nameIgnoreCase = false)
		=> @this.GetField(name, nameIgnoreCase ? INSTANCE_BINDING_FLAGS | IgnoreCase : INSTANCE_BINDING_FLAGS)?
			.SetFieldValue(instance, value);

	[DebuggerHidden]
	public static void SetPropertyValue(this Type @this, string name, object instance, object? value, bool nameIgnoreCase = false, params object?[]? index)
		=> @this.GetProperty(name, nameIgnoreCase ? INSTANCE_BINDING_FLAGS | IgnoreCase : INSTANCE_BINDING_FLAGS)?
			.SetPropertyValue(instance, value, index);

	[DebuggerHidden]
	public static void SetStaticPropertyValue(this Type @this, string name, object? value, bool nameIgnoreCase = false, params object?[]? index)
		=> @this.GetProperty(name, nameIgnoreCase ? STATIC_BINDING_FLAGS | IgnoreCase : STATIC_BINDING_FLAGS)?
			.SetPropertyValue(null, value, index);

	[DebuggerHidden]
	public static void SetStaticFieldValue(this Type @this, string name, object? value, bool nameIgnoreCase = false)
		=> @this.GetField(name, nameIgnoreCase ? STATIC_BINDING_FLAGS | IgnoreCase : STATIC_BINDING_FLAGS)?
			.SetFieldValue(null, value);

	/// <inheritdoc cref="Expression.Default(Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Default(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DefaultExpression ToDefaultExpression(this Type @this)
		=> Expression.Default(@this);

	public static Type? ToGenericType(this Type? @this)
		=> @this switch
		{
			{ IsGenericTypeDefinition: true } => @this,
			{ IsGenericType: true } => @this.GetGenericTypeDefinition(),
			_ => null
		};

	/// <inheritdoc cref="Expression.Label(Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LabelTarget ToLabelTarget(this Type @this)
		=> Expression.Label(@this);

	/// <inheritdoc cref="Expression.Label(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LabelTarget ToLabelTarget(this Type @this, string? name)
		=> Expression.Label(@this, name);

	/// <inheritdoc cref="Expression.New(Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static NewExpression ToNewExpression(this Type @this)
		=> Expression.New(@this);

	/// <inheritdoc cref="Expression.Field(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToStaticFieldExpression(this Type @this, string name)
		=> Expression.Field(null, @this, name);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <see cref="Type.EmptyTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression ToStaticMethodCallExpression(this Type @this, string method, params Expression[]? arguments)
		=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <paramref name="genericTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression ToStaticMethodCallExpression(this Type @this, string method, Type[]? genericTypes, params Expression[]? arguments)
		=> Expression.Call(@this, method, genericTypes, arguments);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToStaticPropertyExpression(this Type @this, string name)
		=> Expression.Property(null, @this, name);
}
