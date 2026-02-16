// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public static class Extensions
{
	extension(Type @this)
	{
		public string AssemblyName => @this.Assembly.GetName().Name ?? string.Empty;

		public IReadOnlySet<Attribute> DeclaredAttributes => TypeStore.Attributes[@this.TypeHandle];

		public ClrType ClrType => @this switch
		{
			{ IsEnum: true } => Reflection.ClrType.Enum,
			{ IsValueType: true } => Reflection.ClrType.Struct,
			{ IsInterface: true } => Reflection.ClrType.Interface,
			_ when @this.IsAssignableTo<Delegate>() => Reflection.ClrType.Delegate,
			_ => Reflection.ClrType.Class,
		};

		public string BaseName => @this.IsGenericType ? @this.Name.Substring(0, @this.Name.IndexOf(TypeStore.GENERIC_TICKMARK)) : @this.Name;

		public string CodeName => TypeStore.CodeNames[@this.TypeHandle];

		public CollectionType CollectionType => TypeStore.CollectionTypes[@this.TypeHandle];

		public ConstructorSet Constructors => TypeStore.Constructors[@this.TypeHandle];

		public PropertyIndexerEntity? DefaultIndexer => TypeStore.PropertyIndexers[@this.TypeHandle] switch
		{
			{ Count: 1 } propertyIndexers => propertyIndexers.First().Value,
			_ => null
		};

		public IReadOnlyDictionary<string, FieldEntity> Fields => TypeStore.Fields[@this.TypeHandle];

		public IReadOnlySet<RuntimeTypeHandle> Interfaces => TypeStore.Interfaces[@this.TypeHandle];

		/// <remarks>
		/// <c>=&gt; @this.IsPointer &amp;&amp; !@this.IsByRef &amp;&amp; !@this.IsByRefLike;</c>
		/// </remarks>
		internal bool IsInvokable => !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

		/// <remarks>
		/// <c>=&gt; @this.IsClass || @this.IsPointer || @this.Is(<see langword="typeof"/>(Nullable&lt;&gt;));</c>
		/// </remarks>
		public bool IsNullable => @this.IsClass || @this.IsPointer || @this.Is(typeof(Nullable<>));

		public IReadOnlyDictionary<string, Literal> Literals => TypeStore.Literals[@this.TypeHandle];

		public IReadOnlyDictionary<string, MethodSet<MethodEntity>> Methods => TypeStore.Methods[@this.TypeHandle];

		public ObjectType ObjectType => TypeStore.ObjectTypes[@this.TypeHandle];

		public IReadOnlyDictionary<string, PropertyEntity> Properties => TypeStore.Properties[@this.TypeHandle];

		public IReadOnlyDictionary<string, PropertyIndexerEntity> PropertyIndexers => TypeStore.PropertyIndexers[@this.TypeHandle];

		public ScalarType ScalarType => @this.IsEnum ? Reflection.ScalarType.Enum : TypeStore.ScalarTypes.GetValue(@this.TypeHandle, Reflection.ScalarType.None);

		public IReadOnlyDictionary<string, StaticFieldEntity> StaticFields => TypeStore.StaticFields[@this.TypeHandle];

		public IReadOnlyDictionary<string, MethodSet<StaticMethodEntity>> StaticMethods => TypeStore.StaticMethods[@this.TypeHandle];

		public IReadOnlyDictionary<string, StaticPropertyEntity> StaticProperties => TypeStore.StaticProperties[@this.TypeHandle];

		public IReadOnlyDictionary<string, StaticPropertyIndexerEntity> StaticPropertyIndexers => TypeStore.StaticPropertyIndexers[@this.TypeHandle];

		/// <exception cref="MissingMethodException"></exception>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public object? Create()
			=> @this.Constructors.FindDefault()?.Create();

		/// <param name="arguments">Constructor parameter arguments</param>
		/// <exception cref="MissingMethodException"></exception>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public object? Create(object?[] arguments)
			=> @this.Constructors.Find(arguments)?.Create(arguments);

		/// <param name="arguments">Constructor parameter arguments</param>
		/// <exception cref="MissingMethodException"></exception>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public object? Create(ITuple arguments)
			=> @this.Constructors.Find(arguments)?.Create(arguments);

		/// <remarks>
		/// Supports generic type definitions as well.  For example:
		/// <list type="table">
		/// <item><c><see langword="typeof"/>(List&lt;Int32&gt;).Implements(<see langword="typeof"/>(IList&lt;Int32&gt;)) // <see langword="true"/></c></item>
		/// <item><c><see langword="typeof"/>(List&lt;String&gt;).Implements(<see langword="typeof"/>(IList&lt;&gt;)) // <see langword="true"/></c></item>
		/// <item><c><see langword="typeof"/>(List&lt;&gt;).Implements(<see langword="typeof"/>(IList&lt;Int32&gt;)) // <see langword="false"/></c></item>
		/// </list>
		/// </remarks>
		public bool Implements(Type type)
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
		/// <c>=&gt; @this == <see langword="typeof"/>(<typeparamref name="T"/>);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool Is<T>()
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
		public bool Is(Type type)
			=> (@this, type) switch
			{
				({ IsGenericType: true, IsGenericTypeDefinition: false }, { IsGenericTypeDefinition: true })
					=> @this.GetGenericTypeDefinition() == type,
				({ IsGenericTypeDefinition: true }, { IsGenericType: true, IsGenericTypeDefinition: false })
					=> @this == type.GetGenericTypeDefinition(),
				_ => @this == type
			};

		/// <summary>
		/// <c>=&gt; <paramref name="types"/>.Any(@this.Is);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAny(Type[] types)
			=> types.Any(@this.Is);

		/// <summary>
		/// <c>=&gt; @this.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>)]);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAny<T1, T2>()
			=> @this.IsAny([typeof(T1), typeof(T2)]);

		/// <summary>
		/// <c>=&gt; @this.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>),
		/// <see langword="typeof"/>(<typeparamref name="T3"/>)]);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAny<T1, T2, T3>()
			=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3)]);

		/// <summary>
		/// <c>=&gt; @this.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>),
		/// <see langword="typeof"/>(<typeparamref name="T3"/>),
		/// <see langword="typeof"/>(<typeparamref name="T4"/>)]);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAny<T1, T2, T3, T4>()
			=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3), typeof(T4)]);

		/// <summary>
		/// <c>=&gt; @this.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>),
		/// <see langword="typeof"/>(<typeparamref name="T3"/>),
		/// <see langword="typeof"/>(<typeparamref name="T4"/>),
		/// <see langword="typeof"/>(<typeparamref name="T5"/>)]);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAny<T1, T2, T3, T4, T5>()
			=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)]);

		/// <summary>
		/// <c>=&gt; @this.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>),
		/// <see langword="typeof"/>(<typeparamref name="T3"/>),
		/// <see langword="typeof"/>(<typeparamref name="T4"/>),
		/// <see langword="typeof"/>(<typeparamref name="T5"/>),
		/// <see langword="typeof"/>(<typeparamref name="T6"/>)]);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAny<T1, T2, T3, T4, T5, T6>()
			=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)]);

		/// <inheritdoc cref="Type.IsAssignableFrom(Type?)"/>
		/// <summary>
		/// <c>=&gt; @this.IsAssignableFrom(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAssignableFrom<T>()
			=> @this.IsAssignableFrom(typeof(T));

		/// <inheritdoc cref="Type.IsAssignableTo(Type?)"/>
		/// <remarks>
		/// <c>=&gt; @this.IsAssignableTo(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAssignableTo<T>()
			=> @this.IsAssignableTo(typeof(T));

		/// <summary>
		/// Whether the type implements <c><see cref="IConvertible"/></c>.<br/>
		/// If the type is <c><see cref="Nullable{T}"/></c>, then whether the type parameter <c>T</c> implements <c><see cref="IConvertible"/></c>.
		/// </summary>
		[DebuggerHidden]
		public bool IsConvertible()
			=> @this.IsAssignableTo<IConvertible>()
				|| (@this.Is(typeof(Nullable<>)) && @this.GenericTypeArguments.First().IsAssignableTo<IConvertible>());

		/// <exception cref="ArgumentNullException"/>
		public bool IsConvertibleTo(Type targetType)
		{
			@this.ThrowIfNull();
			targetType.ThrowIfNull();

			return @this.ScalarType.IsConvertibleTo(targetType.ScalarType);
		}

		/// <summary>
		/// <c>=&gt; @this.IsAssignableTo&lt;<see cref="System.Collections.IEnumerable"/>&gt;();</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsEnumerable()
			=> @this.IsAssignableTo<System.Collections.IEnumerable>();

		/// <summary>
		/// <c>=&gt; @this.IsAssignableTo&lt;<see cref="IEnumerable{T}"/>&gt;();</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsEnumerableOf<T>()
			=> @this.IsAssignableTo<IEnumerable<T>>();
	}

	extension(ConstructorInfo @this)
	{
		public ConstructorEntity? ConstructorEntity => @this.DeclaringType?.Constructors.FirstOrDefault(_ => _.Handle == @this.MethodHandle);
	}

	extension(MethodInfo @this)
	{
		/// <remarks>
		/// <c>!@this.ReturnType.IsAny([<see langword="typeof"/>(Task), <see langword="typeof"/>(ValueTask), <see langword="typeof"/>(<see langword="void"/>)]);</c>
		/// </remarks>
		public bool HasReturnValue => !@this.ReturnType.IsAny([typeof(Task), typeof(ValueTask), typeof(void)]);

		public MethodEntity? MethodEntity => !@this.IsGenericMethodDefinition && !@this.IsStatic
			? @this.DeclaringType?.Methods[@this.Name].FirstOrDefault(_ => _.Handle == @this.MethodHandle)
			: null;

		public StaticMethodEntity? StaticMethodEntity => !@this.IsGenericMethodDefinition && @this.IsStatic
			? @this.DeclaringType?.StaticMethods[@this.Name].FirstOrDefault(_ => _.Handle == @this.MethodHandle)
			: null;

		public bool IsCallableWith(object?[] arguments)
			=> @this.GetParameters() switch
			{
				null or [] => @this.IsCallableWithNoArguments(),
				var parameterInfos when arguments.Length > parameterInfos.Length => false,
				var parameterInfos => parameterInfos.All(_ => _ switch
				{
					_ when _.IsOut => false,
					_ when _.Position >= arguments.Length => _.HasDefaultValue || _.IsOptional,
					_ when arguments[_.Position] is not null => _.ParameterType.IsAssignableFrom(arguments[_.Position]!.GetType()),
					_ => _.ParameterType.IsNullable
				})
			};

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsCallableWith(ITuple arguments)
			=> @this.IsCallableWith(arguments.ToArray());

		public bool IsCallableWith(Type[] argumentTypes)
			=> argumentTypes switch
			{
				null or [] => @this.IsCallableWithNoArguments(),
				_ when argumentTypes.Length > @this.GetParameters().Length => false,
				_ => @this.GetParameters().All(_ => _ switch
				{
					_ when _.IsOut || _.IsRetval => false,
					_ when _.Position >= argumentTypes.Length => _.HasDefaultValue || _.IsOptional,
					_ => _.ParameterType.IsAssignableFrom(argumentTypes[_.Position])
				})
			};

		public bool IsCallableWithNoArguments()
			=> @this.GetParameters().Length is 0 || @this.GetParameters().All(_ => _.HasDefaultValue || _.IsOptional);

		/// <remarks>
		/// <c>=&gt; ((<see cref="MethodBase"/>)@this).IsInvokable() &amp;&amp; @this.ReturnType.IsInvokable();</c>
		/// </remarks>
		[DebuggerHidden]
		internal bool IsInvokable()
			=> @this.GetParameters().All(parameterInfo => !parameterInfo.IsOut && parameterInfo.ParameterType.IsInvokable)
				&& @this.ReturnType.IsInvokable;

		/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
		/// <remarks>
		/// <c>=&gt; @this.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodInfo MakeGenericMethod<T1>()
			=> @this.MakeGenericMethod(typeof(T1));

		/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
		/// <remarks>
		/// <c>=&gt; @this.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodInfo MakeGenericMethod<T1, T2>()
			=> @this.MakeGenericMethod(typeof(T1), typeof(T2));

		/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
		/// <remarks>
		/// <c>=&gt; @this.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>),
		/// <see langword="typeof"/>(<typeparamref name="T3"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodInfo MakeGenericMethod<T1, T2, T3>()
			=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3));

		/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
		/// <remarks>
		/// <c>=&gt; @this.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>),
		/// <see langword="typeof"/>(<typeparamref name="T3"/>),
		/// <see langword="typeof"/>(<typeparamref name="T4"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodInfo MakeGenericMethod<T1, T2, T3, T4>()
			=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

		/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
		/// <remarks>
		/// <c>=&gt; @this.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>),
		/// <see langword="typeof"/>(<typeparamref name="T3"/>),
		/// <see langword="typeof"/>(<typeparamref name="T4"/>),
		/// <see langword="typeof"/>(<typeparamref name="T5"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5>()
			=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

		/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
		/// <remarks>
		/// <c>=&gt; @this.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>),
		/// <see langword="typeof"/>(<typeparamref name="T3"/>),
		/// <see langword="typeof"/>(<typeparamref name="T4"/>),
		/// <see langword="typeof"/>(<typeparamref name="T5"/>),
		/// <see langword="typeof"/>(<typeparamref name="T6"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5, T6>()
			=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

		/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
		/// <remarks>
		/// <c>=&gt; @this.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
		/// <see langword="typeof"/>(<typeparamref name="T2"/>),
		/// <see langword="typeof"/>(<typeparamref name="T3"/>),
		/// <see langword="typeof"/>(<typeparamref name="T4"/>),
		/// <see langword="typeof"/>(<typeparamref name="T5"/>),
		/// <see langword="typeof"/>(<typeparamref name="T6"/>),
		/// <see langword="typeof"/>(<typeparamref name="T7"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5, T6, T7>()
			=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
	}

	extension(FieldInfo @this)
	{
		public FieldEntity? FieldEntity => @this.DeclaringType?.Fields.Values.FirstOrDefault(_ => _.Handle == @this.FieldHandle);

		public StaticFieldEntity? StaticFieldEntity => @this.DeclaringType?.StaticFields.Values.FirstOrDefault(_ => _.Handle == @this.FieldHandle);
	}

	extension(RuntimeFieldHandle @this)
	{
		/// <inheritdoc cref="FieldInfo.GetFieldFromHandle(RuntimeFieldHandle)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="FieldInfo"/>.GetFieldFromHandle(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public FieldInfo ToFieldInfo()
			=> FieldInfo.GetFieldFromHandle(@this);

		/// <inheritdoc cref="FieldInfo.GetFieldFromHandle(RuntimeFieldHandle, RuntimeTypeHandle)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="FieldInfo"/>.GetFieldFromHandle(@this, <paramref name="typeHandle"/>);</c>
		/// </remarks>
		/// <param name="typeHandle"><see cref="RuntimeTypeHandle"/> is needed when <see cref="RuntimeFieldHandle"/> is a field whose type is a generic parameter of its declared type.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public FieldInfo ToFieldInfo(RuntimeTypeHandle typeHandle)
			=> FieldInfo.GetFieldFromHandle(@this, typeHandle);
	}

	extension(RuntimeMethodHandle @this)
	{
		/// <inheritdoc cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@this)
		/// ?? <see langword="throw new"/> MissingMethodException(Invariant($"MethodBase.GetMethodFromHandle({@this}) returned null, method may depend on generic class parameter."));</c>
		/// </remarks>
		/// <exception cref="MissingMethodException"></exception>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodBase ToMethodBase()
			=> MethodBase.GetMethodFromHandle(@this) ?? throw new MissingMethodException(Invariant($"MethodBase.GetMethodFromHandle({@this}) returned null, method may depend on generic class parameter."));

		/// <inheritdoc cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle, RuntimeTypeHandle)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@this, <paramref name="typeHandle"/>)
		/// ?? <see langword="throw new"/> MissingMethodException(Invariant($"MethodBase.GetMethodFromHandle({@this}, {<paramref name="typeHandle"/>}) returned null."));</c>
		/// </remarks>
		/// <param name="typeHandle"><see cref="RuntimeTypeHandle"/> is needed when <see cref="RuntimeMethodHandle"/> is a method or constructor using a generic parameter of its declared type.</param>
		/// <exception cref="UnreachableException"></exception>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodBase ToMethodBase(RuntimeTypeHandle typeHandle)
			=> MethodBase.GetMethodFromHandle(@this, typeHandle) ?? throw new UnreachableException(Invariant($"MethodBase.GetMethodFromHandle({@this}, {typeHandle}) returned null."));
	}

	extension(RuntimeTypeHandle @this)
	{
		/// <inheritdoc cref="Type.GetTypeFromHandle(RuntimeTypeHandle)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Type"/>.GetTypeFromHandle(@this)
		/// ?? <see langword="throw new"/> UnreachableException("Type.GetTypeFromHandle(@this) returned null.");</c>
		/// </remarks>
		/// <exception cref="UnreachableException"></exception>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		[return: NotNull]
		public Type ToType()
			=> Type.GetTypeFromHandle(@this) ?? throw new UnreachableException(Invariant($"Type.GetTypeFromHandle({@this}) returned null."));
	}

	extension<T>(T)
		where T : notnull
	{
		/// <summary>
		/// Get method overloads for <paramref name="methodName"/>.
		/// </summary>
		public static (T, MethodSet<MethodEntity>) operator |(T instance, string methodName)
			=> (instance, Type<T>.Methods[methodName]);

		/// <summary>
		/// Get the value for matching field or property with name <paramref name="memberName"/>.
		/// </summary>
		public static object? operator &(T instance, string memberName)
			=> Type<T>.Properties.TryGetValue(memberName, out var property) ? property.GetValue(instance)
				: (Type<T>.Fields.TryGetValue(memberName, out var field) ? field.GetValue(instance) : null);

		/// <summary>
		/// Set the value for matching named field or property.
		/// </summary>
		public static bool operator &(T instance, (string MemberName, object? Value) _)
		{
			if (Type<T>.Properties.TryGetValue(_.MemberName, out var property))
			{
				property.SetValue(instance, _.Value);
				return true;
			}

			if (Type<T>.Fields.TryGetValue(_.MemberName, out var field))
			{
				field.SetValue(instance, _.Value);
				return true;
			}

			return false;
		}
	}

	extension(Type)
	{
		/// <summary>
		/// Construct generic <c><see cref="Type"/></c>.
		/// </summary>
		public static Type operator &(Type type, Type[] genericTypeArguments)
			=> type.MakeGenericType(genericTypeArguments);

		/// <summary>
		/// Finds constructor with <paramref name="arguments"/>.
		/// </summary>
		public static ConstructorEntity? operator |(Type type, ITuple arguments)
			=> type.Constructors.Find(arguments);

		/// <summary>
		/// Invokes constructor with <paramref name="arguments"/>.
		/// </summary>
		public static object? operator &(Type type, ITuple arguments)
			=> type.Constructors.Find(arguments)?.Create(arguments);

		/// <summary>
		/// Get static method overloads for <paramref name="methodName"/>.
		/// </summary>
		public static MethodSet<StaticMethodEntity> operator |(Type type, string methodName)
			=> type.StaticMethods[methodName];
	}

	extension<T>(MethodEntity)
		where T : notnull
	{
		public static (MethodEntity, T) operator |(MethodEntity method, T instance)
			=> (method, instance);
	}

	extension<T>((MethodEntity, T))
		where T : notnull
	{
		/// <summary>
		/// Invokes method with <paramref name="arguments"/>.
		/// </summary>
		public static object? operator &((MethodEntity Method, T Instance) _, ITuple arguments)
			=> _.Method.Invoke(_.Instance, arguments);
	}

	extension(StaticMethodEntity)
	{
		/// <summary>
		/// Invokes static method with <paramref name="arguments"/>.
		/// </summary>
		public static object? operator &(StaticMethodEntity method, ITuple arguments)
			=> method.Invoke(arguments);
	}

	extension(MethodSet<MethodEntity>)
	{
		/// <summary>
		/// Include generic method type arguments.
		/// </summary>
		public static (MethodSet<MethodEntity>, Type[]) operator |(MethodSet<MethodEntity> methods, Type[] genericTypeArguments)
			=> (methods, genericTypeArguments);

		/// <summary>
		/// Finds matching method with <paramref name="arguments"/>.
		/// </summary>
		public static MethodEntity? operator |(MethodSet<MethodEntity> methods, ITuple arguments)
			=> methods.Find(arguments);
	}

	extension<T>(MethodSet<MethodEntity>)
		where T : notnull
	{
		/// <summary>
		/// Finds matching method with <paramref name="arguments"/>.
		/// </summary>
		public static (MethodSet<MethodEntity>, T) operator |(MethodSet<MethodEntity> methods, T instance)
			=> (methods, instance);
	}

	extension<T>((MethodSet<MethodEntity>, T))
		where T : notnull
	{
		/// <summary>
		/// Invokes matching method with <paramref name="arguments"/>.
		/// </summary>
		public static object? operator &((MethodSet<MethodEntity> Methods, T Instance) _, ITuple arguments)
			=> _.Methods.Find(arguments)?.Invoke(_.Instance!, arguments);
	}

	extension((MethodSet<MethodEntity>, Type[]))
	{
		/// <summary>
		/// Invokes matching generic method overload with <paramref name="arguments"/>.
		/// </summary>
		public static MethodEntity? operator |((MethodSet<MethodEntity> Methods, Type[] GenericTypeArguments) _, ITuple arguments)
			=> _.Methods.Find(_.GenericTypeArguments, arguments);
	}

	extension<T>((MethodSet<MethodEntity>, Type[]))
		where T : notnull
	{
		/// <summary>
		/// Finds matching generic method overload with <paramref name="arguments"/>.
		/// </summary>
		public static (MethodSet<MethodEntity>, Type[], T) operator |((MethodSet<MethodEntity> Methods, Type[] GenericTypeArguments) _, T instance)
			=> (_.Methods, _.GenericTypeArguments, instance);
	}

	extension<T>((MethodSet<MethodEntity>, Type[], T))
		where T : notnull
	{
		/// <summary>
		/// Invokes matching method with <paramref name="arguments"/>.
		/// </summary>
		public static object? operator &((MethodSet<MethodEntity> Methods, Type[] GenericTypeArguments, T Instance) _, ITuple arguments)
			=> _.Methods.Find(_.GenericTypeArguments, arguments)?.Invoke(_.Instance!, arguments);
	}

	extension(MethodSet<StaticMethodEntity>)
	{
		/// <summary>
		/// Include generic static method type arguments.
		/// </summary>
		public static (MethodSet<StaticMethodEntity>, Type[]) operator |(MethodSet<StaticMethodEntity> methods, Type[] genericTypeArguments)
			=> (methods, genericTypeArguments);

		/// <summary>
		/// Finds matching static method overload with <paramref name="arguments"/>.
		/// </summary>
		public static StaticMethodEntity? operator |(MethodSet<StaticMethodEntity> methods, ITuple arguments)
			=> methods.Find(arguments);

		/// <summary>
		/// Invokes matching static method overload with <paramref name="arguments"/>.
		/// </summary>
		public static object? operator &(MethodSet<StaticMethodEntity> methods, ITuple arguments)
			=> methods.Find(arguments)?.Invoke(arguments);
	}

	extension((MethodSet<StaticMethodEntity>, Type[]))
	{
		/// <summary>
		/// Finds matching generic static method overload with <paramref name="arguments"/>.
		/// </summary>
		public static StaticMethodEntity? operator |((MethodSet<StaticMethodEntity> Methods, Type[] GenericTypeArguments) _, ITuple arguments)
			=> _.Methods.Find(_.GenericTypeArguments, arguments);

		/// <summary>
		/// Invokes matching generic static method overload with <paramref name="arguments"/>.
		/// </summary>
		public static object? operator &((MethodSet<StaticMethodEntity> Methods, Type[] GenericTypeArguments) _, ITuple arguments)
			=> _.Methods.Find(_.GenericTypeArguments, arguments)?.Invoke(arguments);
	}
}
