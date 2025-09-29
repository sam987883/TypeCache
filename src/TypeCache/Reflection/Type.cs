// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Type<{CodeName}>")]
public static class Type<T>
{
	private static readonly Lazy<IReadOnlySet<Attribute>> _Attributes = new(() => TypeStore.Attributes[Handle]);
	private static readonly Lazy<ClrType> _ClrType = new(() => TypeStore.ClrTypes[Handle]);
	private static readonly Lazy<string> _CodeName = new(() => TypeStore.CodeNames[Handle]);
	private static readonly Lazy<CollectionType> _CollectionType = new(() => TypeStore.CollectionTypes[Handle]);
	private static readonly Lazy<ConstructorSet> _Constructors = new(() => TypeStore.Constructors[Handle]);
	private static readonly Lazy<IReadOnlyDictionary<string, FieldEntity>> _Fields = new(() => TypeStore.Fields[Handle]);
	private static readonly Lazy<IReadOnlySet<RuntimeTypeHandle>> _Interfaces = new(() => TypeStore.Interfaces[Handle]);
	private static readonly Lazy<IReadOnlyDictionary<string, MethodSet<MethodEntity>>> _Methods = new(() => TypeStore.Methods[Handle]);
	private static readonly Lazy<ObjectType> _ObjectType = new(() => TypeStore.ObjectTypes[Handle]);
	private static readonly Lazy<IReadOnlyDictionary<string, PropertyEntity>> _Properties = new(() => TypeStore.Properties[Handle]);
	private static readonly Lazy<ScalarType> _ScalarType = new(() => Handle.ToType() switch
	{
		{ IsEnum: true } => ScalarType.Enum,
		_ when TypeStore.ScalarTypes.TryGetValue(Handle, out var value) => value,
		_ => ScalarType.None
	});
	private static readonly Lazy<IReadOnlyDictionary<string, StaticFieldEntity>> _StaticFields = new(() => TypeStore.StaticFields[Handle]);
	private static readonly Lazy<IReadOnlyDictionary<string, MethodSet<StaticMethodEntity>>> _StaticMethods = new(() => TypeStore.StaticMethods[Handle]);

	public static string AssemblyName { get; } = typeof(T).Assembly.GetName().Name ?? string.Empty;

	public static IReadOnlySet<Attribute> Attributes => _Attributes.Value;

	public static ClrType ClrType => _ClrType.Value;

	public static string CodeName => _CodeName.Value;

	public static CollectionType CollectionType => _CollectionType.Value;

	public static ConstructorSet Constructors => _Constructors.Value;

	public static IReadOnlyDictionary<string, FieldEntity> Fields => _Fields.Value;

	public static Type[] GenericTypes => typeof(T).GenericTypeArguments;

	public static RuntimeTypeHandle Handle { get; } = typeof(T).TypeHandle;

	public static IReadOnlySet<RuntimeTypeHandle> Interfaces => _Interfaces.Value;

	public static bool IsGeneric { get; } = typeof(T).IsGenericType;

	public static bool IsPublic { get; } = typeof(T).IsPublic;

	public static IReadOnlyDictionary<string, MethodSet<MethodEntity>> Methods => _Methods.Value;

	public static string Name { get; } = typeof(T).Name;

	public static string Namespace { get; } = typeof(T).Namespace ?? string.Empty;

	public static ObjectType ObjectType => _ObjectType.Value;

	public static IReadOnlyDictionary<string, PropertyEntity> Properties => _Properties.Value;

	public static ScalarType ScalarType => _ScalarType.Value;

	public static IReadOnlyDictionary<string, StaticFieldEntity> StaticFields => _StaticFields.Value;

	public static IReadOnlyDictionary<string, MethodSet<StaticMethodEntity>> StaticMethods => _StaticMethods.Value;

	/// <exception cref="MissingMethodException"></exception>
	public static T? Create()
		=> (T?)Constructors.Invoke();

	/// <param name="arguments">Constructor parameter arguments</param>
	/// <exception cref="MissingMethodException"></exception>
	public static T? Create(object?[] arguments)
		=> (T?)Constructors.Invoke(arguments);

	/// <param name="arguments">Constructor parameter arguments</param>
	/// <exception cref="MissingMethodException"></exception>
	public static T? Create(ITuple arguments)
		=> (T?)Constructors.Invoke(arguments);

	public static IDictionary<string, object?> GetFieldValues(T instance)
		=> _Fields.Value
			.Select(_ => (_.Key, _.Value.GetValue(instance!)))
			.ToDictionary(_Fields.Value.Keys.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

	public static IDictionary<string, object?> GetPropertyValues(T instance)
		=> _Properties.Value
			.Where(_ => _.Value.CanRead && !_.Value.IsStaticGet)
			.Select(_ => (_.Key, _.Value.GetValue(instance!)))
			.ToDictionary(_Properties.Value.Keys.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
}
