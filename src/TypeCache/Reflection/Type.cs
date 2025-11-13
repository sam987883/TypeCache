// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Type<{CodeName}>")]
public static class Type<T>
{
	private static readonly Lazy<Func<T?>> _CreateValue = new(typeof(T).IsValueType ? Expression.New(typeof(T)).Lambda<Func<T?>>().Compile() : () => default);

	static Type()
	{
		Handle = typeof(T).TypeHandle;
		AssemblyName = typeof(T).Assembly.GetName().Name ?? string.Empty;
		ClrType = typeof(T).ClrType;
		CollectionType = TypeStore.CollectionTypes[Handle];
		IsGeneric = typeof(T).IsGenericType;
		IsPublic = typeof(T).IsPublic;
		Name = typeof(T).IsGenericType ? typeof(T).Name.Substring(typeof(T).Name.IndexOf(TypeStore.GENERIC_TICKMARK)) : typeof(T).Name;
		Namespace = typeof(T).Namespace ?? string.Empty;
		ObjectType = TypeStore.ObjectTypes[Handle];
		ScalarType = typeof(T).ScalarType;
	}

	public static string AssemblyName { get; }

	public static IReadOnlySet<Attribute> Attributes => field ??= TypeStore.Attributes[Handle];

	public static ClrType ClrType { get; }

	public static string CodeName => field ??= TypeStore.CodeNames[Handle];

	public static CollectionType CollectionType { get; }

	public static ConstructorSet Constructors => field ??= TypeStore.Constructors[Handle];

	public static PropertyIndexerEntity? DefaultIndexer => PropertyIndexers switch
	{
		{ Count: 1 } propertyIndexers => propertyIndexers.First().Value,
		_ => null
	};

	public static IReadOnlyDictionary<string, FieldEntity> Fields => field ??= TypeStore.Fields[Handle];

	public static Type[] GenericTypes => typeof(T).GenericTypeArguments;

	public static RuntimeTypeHandle Handle { get; }

	public static IReadOnlySet<RuntimeTypeHandle> Interfaces => field ??= TypeStore.Interfaces[Handle];

	public static bool IsGeneric { get; }

	public static bool IsPublic { get; }

	public static IReadOnlyDictionary<string, MethodSet<MethodEntity>> Methods => field ??= TypeStore.Methods[Handle];

	public static string Name { get; }

	public static string Namespace { get; }

	public static ObjectType ObjectType { get; }

	public static IReadOnlyDictionary<string, PropertyEntity> Properties => field ??= TypeStore.Properties[Handle];

	public static IReadOnlyDictionary<string, PropertyIndexerEntity> PropertyIndexers => field ??= TypeStore.PropertyIndexers[Handle];

	public static ScalarType ScalarType { get; }

	public static IReadOnlyDictionary<string, StaticFieldEntity> StaticFields => field ??= TypeStore.StaticFields[Handle];

	public static IReadOnlyDictionary<string, MethodSet<StaticMethodEntity>> StaticMethods => field ??= TypeStore.StaticMethods[Handle];

	public static IReadOnlyDictionary<string, StaticPropertyEntity> StaticProperties => field ??= TypeStore.StaticProperties[Handle];

	public static IReadOnlyDictionary<string, StaticPropertyIndexerEntity> StaticPropertyIndexers => field ??= TypeStore.StaticPropertyIndexers[Handle];

	public static T? Create()
		=> (T?)Constructors.FindDefault()?.Create() ?? _CreateValue.Value();

	/// <param name="arguments">Constructor parameter arguments</param>
	public static T? Create(object?[] arguments)
		=> (T?)Constructors.Find(arguments)?.Create(arguments);

	/// <param name="arguments">Constructor parameter arguments</param>
	public static T? Create(ITuple arguments)
		=> (T?)Constructors.Find(arguments)?.Create(arguments);

	public static IDictionary<string, object?> GetFieldValues(T instance)
		=> Fields
			.Select(_ => (_.Key, _.Value.GetValue(instance!)))
			.ToDictionary(Fields.Keys.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

	public static IDictionary<string, object?> GetPropertyValues(T instance)
		=> Properties
			.Where(_ => _.Value.GetMethod is not null)
			.Select(_ => (_.Key, _.Value.GetValue(instance!)))
			.ToDictionary(Properties.Keys.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
}
