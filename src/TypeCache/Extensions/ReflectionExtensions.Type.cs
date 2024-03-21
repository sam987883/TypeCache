// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Utilities;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	/// <exception cref="MissingMethodException"></exception>
	public static object? Create(this Type @this, object?[]? parameters = null)
		=> @this.FindConstructor(parameters) switch
		{
			null when @this.IsValueType && parameters?.Any() is not true => TypeStore.DefaultValueTypeConstructorFuncs[@this.TypeHandle].Invoke(),
			null => throw new MissingMethodException(@this.Name, "Constructor"),
			var constructorInfo => constructorInfo.InvokeMethod(parameters)
		};

	[DebuggerHidden]
	public static ConstructorInfo? FindConstructor(this Type @this, object?[]? arguments = null)
		=> @this.GetConstructors(INSTANCE_BINDING_FLAGS)
			.FirstOrDefault(constructor => constructor.IsCallableWith(arguments));

	/// <inheritdoc cref="Type.GetMethod(string, BindingFlags, Type[])"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo? FindMethod(this Type @this, string name, Type[] argumentTypes)
		=> @this.GetMethod(name, INSTANCE_BINDING_FLAGS, argumentTypes);

	/// <inheritdoc cref="Type.GetMethods(BindingFlags)"/>
	[DebuggerHidden]
	public static MethodInfo? FindMethod(this Type @this, string name, object?[]? arguments = null)
		=> @this.GetMethods(INSTANCE_BINDING_FLAGS)
			.FirstOrDefault(method => method.Name.EqualsIgnoreCase(name) && method.IsCallableWith(arguments));

	/// <inheritdoc cref="Type.GetMethods(BindingFlags)"/>
	[DebuggerHidden]
	public static MethodInfo? FindMethod(this Type @this, string name, Type[] genericTypes, object?[]? arguments = null)
		=> @this.GetMethods(INSTANCE_BINDING_FLAGS)
			.Where(method => method.Name.EqualsIgnoreCase(name) && method.IsGenericMethod && method.GetGenericArguments().Length == genericTypes.Length)
			.FirstOrDefault(method => method.MakeGenericMethod(genericTypes)?.IsCallableWith(arguments) is true);

	/// <inheritdoc cref="Type.GetMethod(string, BindingFlags, Type[])"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo? FindStaticMethod(this Type @this, string name, Type[] argumentTypes)
		=> @this.GetMethod(name, STATIC_BINDING_FLAGS, argumentTypes);

	/// <inheritdoc cref="Type.GetMethods(BindingFlags)"/>
	[DebuggerHidden]
	public static MethodInfo? FindStaticMethod(this Type @this, string name, object?[]? arguments = null)
		=> @this.GetMethods(STATIC_BINDING_FLAGS)
			.FirstOrDefault(method => method.Name.EqualsIgnoreCase(name) && method.IsCallableWith(arguments));

	/// <inheritdoc cref="Type.GetField(string, BindingFlags)"/>
	[DebuggerHidden]
	public static object? GetFieldValue(this Type @this, string name, object instance)
		=> @this.GetField(name, INSTANCE_BINDING_FLAGS)?
			.GetFieldValue(instance);

	/// <summary>
	/// <c>=&gt; <see cref="TypeStore.CollectionTypes"/>[@<paramref name="this"/>.TypeHandle];</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static CollectionType GetCollectionType(this Type @this)
		=> TypeStore.CollectionTypes[@this.IsGenericType ? @this.GetGenericTypeDefinition().TypeHandle : @this.TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ObjectType GetObjectType(this Type @this)
		=> TypeStore.ObjectTypes[@this.IsGenericType ? @this.GetGenericTypeDefinition().TypeHandle : @this.TypeHandle];

	public static ScalarType GetScalarType(this Type @this)
		=> @this switch
		{
			{ IsGenericTypeDefinition: true } => ScalarType.None,
			{ IsEnum: true } => ScalarType.Enum,
			{ IsGenericType: true } when @this.IsNullable() => @this.GenericTypeArguments[0] switch
			{
				{ IsEnum: true } => ScalarType.Enum,
				Type type when TypeStore.DataTypes.TryGetValue(type.TypeHandle, out var scalarType) => scalarType,
				_ => ScalarType.None
			},
			_ when TypeStore.DataTypes.TryGetValue(@this.TypeHandle, out var scalarType) => scalarType,
			_ => ScalarType.None
		};

	/// <inheritdoc cref="Type.GetFields(BindingFlags)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetFields(<see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Instance"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static FieldInfo[] GetPublicFields(this Type @this)
		=> @this.GetFields(FlattenHierarchy | Public | Instance);

	/// <inheritdoc cref="Type.GetMethods(BindingFlags)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetMethods(<see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Instance"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo[] GetPublicMethods(this Type @this)
		=> @this.GetMethods(FlattenHierarchy | Public | Instance);

	/// <inheritdoc cref="Type.GetProperties(BindingFlags)"/>
	[DebuggerHidden]
	public static PropertyInfo[] GetPublicProperties(this Type @this)
		=> @this.GetProperties(FlattenHierarchy | Public | Instance)
			.Where(propertyInfo => propertyInfo.GetMethod?.IsStatic is not true && propertyInfo.SetMethod?.IsStatic is not true)
			.ToArray();

	/// <inheritdoc cref="Type.GetFields(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/></c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static FieldInfo[] GetPublicStaticFields(this Type @this)
		=> @this.GetFields(FlattenHierarchy | Public | Static);

	/// <inheritdoc cref="Type.GetMethods(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/></c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo[] GetPublicStaticMethods(this Type @this)
		=> @this.GetMethods(FlattenHierarchy | Public | Static);

	/// <inheritdoc cref="Type.GetProperties(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/></c>
	/// </remarks>
	[DebuggerHidden]
	public static PropertyInfo[] GetPublicStaticProperties(this Type @this)
		=> @this.GetProperties(FlattenHierarchy | Public | Static)
			.Where(propertyInfo => propertyInfo.GetMethod?.IsStatic is true || propertyInfo.SetMethod?.IsStatic is true)
			.ToArray();

	public static object? GetPropertyValue(this Type @this, string name, object instance, object?[]? index = null)
		=> @this.GetProperty(name, INSTANCE_BINDING_FLAGS)?
			.GetPropertyValue(instance, index);

	public static object? GetStaticFieldValue(this Type @this, string name)
		=> @this.GetField(name, STATIC_BINDING_FLAGS)?
			.GetFieldValue(null);

	public static object? GetStaticPropertyValue(this Type @this, string name, object?[]? index = null)
		=> @this.GetProperty(name, STATIC_BINDING_FLAGS)?
			.GetPropertyValue(null, index);

	public static bool Implements(this Type @this, Type type)
	{
		return type switch
		{
			_ when @this.Is(type) => true,
			{ IsValueType: true } or { IsSealed: true } => false,
			{ IsGenericTypeDefinition: false } => @this.IsAssignableTo(type),
			{ IsInterface: true } => type.IsAny(@this.GetInterfaces()),
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

	public static object? InvokeMethod(this Type @this, string name, object instance, object?[]? arguments = null)
		=> @this.FindMethod(name, arguments)?
			.InvokeMethod(arguments?.Prepend(instance).ToArray() ?? new[] { instance });

	public static object? InvokeMethod(this Type @this, string name, Type[] genericTypes, object instance, object?[]? arguments = null)
		=> @this.FindMethod(name, genericTypes, arguments)?
			.MakeGenericMethod(genericTypes)
			.InvokeMethod(arguments?.Prepend(instance).ToArray() ?? new[] { instance });

	public static object? InvokeMethod<T1>(this Type @this, string name, object instance, object?[]? arguments = null)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1>()
			.InvokeMethod(arguments?.Prepend(instance).ToArray() ?? new[] { instance });

	public static object? InvokeMethod<T1, T2>(this Type @this, string name, object instance, object?[]? arguments = null)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2>()
			.InvokeMethod(arguments?.Prepend(instance).ToArray() ?? new[] { instance });

	public static object? InvokeMethod<T1, T2, T3>(this Type @this, string name, object instance, object?[]? arguments = null)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3>()
			.InvokeMethod(arguments?.Prepend(instance).ToArray() ?? new[] { instance });

	public static object? InvokeMethod<T1, T2, T3, T4>(this Type @this, string name, object instance, object?[]? arguments = null)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4>()
			.InvokeMethod(arguments?.Prepend(instance).ToArray() ?? new[] { instance });

	public static object? InvokeMethod<T1, T2, T3, T4, T5>(this Type @this, string name, object instance, object?[]? arguments = null)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5>()
			.InvokeMethod(arguments?.Prepend(instance).ToArray() ?? new[] { instance });

	public static object? InvokeMethod<T1, T2, T3, T4, T5, T6>(this Type @this, string name, object instance, object?[]? arguments = null)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5, T6>()
			.InvokeMethod(arguments?.Prepend(instance).ToArray() ?? new[] { instance });

	public static object? InvokeStaticMethod(this Type @this, string name, object?[]? arguments = null)
		=> @this.FindStaticMethod(name, arguments)?
			.InvokeMethod(arguments);

	public static object? InvokeStaticMethod(this Type @this, string name, Type[] genericTypes, object?[]? arguments = null)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod(genericTypes)
			.InvokeMethod(arguments);

	public static object? InvokeStaticMethod<T1>(this Type @this, string name, object?[]? arguments = null)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1>()
			.InvokeMethod(arguments);

	public static object? InvokeStaticMethod<T1, T2>(this Type @this, string name, object?[]? arguments = null)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2>()
			.InvokeMethod(arguments);

	public static object? InvokeStaticMethod<T1, T2, T3>(this Type @this, string name, object?[]? arguments = null)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3>()
			.InvokeMethod(arguments);

	public static object? InvokeStaticMethod<T1, T2, T3, T4>(this Type @this, string name, object?[]? arguments = null)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4>()
			.InvokeMethod(arguments);

	public static object? InvokeStaticMethod<T1, T2, T3, T4, T5>(this Type @this, string name, object?[]? arguments = null)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5>()
			.InvokeMethod(arguments);

	public static object? InvokeStaticMethod<T1, T2, T3, T4, T5, T6>(this Type @this, string name, object?[]? arguments = null)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5, T6>()
			.InvokeMethod(arguments);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> == <see langword="typeof"/>(<typeparamref name="T"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Is<T>(this Type @this)
		=> @this == typeof(T);

	[DebuggerHidden]
	public static bool Is(this Type @this, Type type)
		=> (@this, type) switch
		{
			({ IsGenericType: true, IsGenericTypeDefinition: false }, { IsGenericTypeDefinition: true })
				or ({ IsGenericTypeDefinition: true }, { IsGenericType: true, IsGenericTypeDefinition: false })
				=> @this.GetGenericTypeDefinition() == type.GetGenericTypeDefinition(),
			_ => @this == type
		};

	/// <summary>
	/// <c>=&gt; <paramref name="types"/>.Any(@<paramref name="this"/>.Is);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny(this Type @this, Type[] types)
		=> types.Any(@this.Is);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Is([<see langword="typeof"/>(<typeparamref name="T1"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1>(this Type @this)
		=> @this.Is(typeof(T1));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2>(this Type @this)
		=> @this.IsAny(new[] { typeof(T1), typeof(T2) });

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3>(this Type @this)
		=> @this.IsAny(new[] { typeof(T1), typeof(T2), typeof(T3) });

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4>(this Type @this)
		=> @this.IsAny(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4, T5>(this Type @this)
		=> @this.IsAny(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });

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
		=> @this.IsAny(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });

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
		@this.AssertNotNull();
		targetType.AssertNotNull();

		return @this.GetScalarType().IsConvertibleTo(targetType.GetScalarType());
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAssignableTo&lt;<see cref="IEnumerable"/>&gt;();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsEnumerable(this Type @this)
		=> @this.IsAssignableTo<IEnumerable>();

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
		=> @this.GetScalarType().IsEnumUnderlyingType();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsPointer &amp;&amp; !@<paramref name="this"/>.IsByRef &amp;&amp; !@<paramref name="this"/>.IsByRefLike;</c>
	/// </remarks>
	[DebuggerHidden]
	internal static bool IsInvokable(this Type @this)
		=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsClass || @<paramref name="this"/>.IsPointer || @<paramref name="this"/>.Is(<see langword="typeof"/>(Nullable&lt;&gt;));</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsNullable(this Type @this)
		=> @this.IsClass || @this.IsPointer || @this.Is(typeof(Nullable<>));

	[DebuggerHidden]
	public static void SetFieldValue(this Type @this, string name, object instance, object? value)
		=> @this.GetField(name, INSTANCE_BINDING_FLAGS)?
			.SetFieldValue(instance, value);

	[DebuggerHidden]
	public static void SetPropertyValue(this Type @this, string name, object instance, object? value, object?[]? index = null)
		=> @this.GetProperty(name, INSTANCE_BINDING_FLAGS)?
			.SetPropertyValue(instance, value, index);

	[DebuggerHidden]
	public static void SetStaticFieldValue(this Type @this, string name, object? value)
		=> @this.GetField(name, STATIC_BINDING_FLAGS)?
			.SetFieldValue(null, value);

	[DebuggerHidden]
	public static void SetStaticPropertyValue(this Type @this, string name, object? value, object?[]? index = null)
		=> @this.GetProperty(name, STATIC_BINDING_FLAGS)?
			.SetPropertyValue(null, value, index);

	/// <inheritdoc cref="Expression.Default(Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Default(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DefaultExpression ToDefaultExpression(this Type @this)
		=> Expression.Default(@this);

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

	/// <inheritdoc cref="Expression.New(ConstructorInfo, Expression[])"/>
	[DebuggerHidden]
	public static NewExpression ToNewExpression(this Type @this, Expression[]? parameters = null)
		=> parameters switch
		{
			null or { Length: 0 } => Expression.New(@this),
			_ => @this.GetConstructor(parameters.Select(parameter => parameter.Type).ToArray())!.ToExpression(parameters)
		};

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
	public static MethodCallExpression ToStaticMethodCallExpression(this Type @this, string method, Expression[]? arguments = null)
		=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <paramref name="genericTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression ToStaticMethodCallExpression(this Type @this, string method, Type[]? genericTypes, Expression[]? arguments = null)
		=> Expression.Call(@this, method, genericTypes, arguments);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToStaticPropertyExpression(this Type @this, string name)
		=> Expression.Property(null, @this, name);
}
