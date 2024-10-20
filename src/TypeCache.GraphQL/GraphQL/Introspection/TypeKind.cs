using System.ComponentModel;

namespace GraphQL.Introspection;

/// <summary>
/// An enumeration representing a kind of GraphQL type.
/// </summary>
public enum TypeKind
{
	/// <summary>
	/// Indicates this type is a scalar.
	/// </summary>
	[Description("Indicates this type is a scalar.")]
	SCALAR = 0,

	/// <summary>
	/// Indicates this type is an object. `fields` and `possibleTypes` are valid fields.
	/// </summary>
	[Description("Indicates this type is an object. `fields` and `possibleTypes` are valid fields.")]
	OBJECT = 1,

	/// <summary>
	/// Indicates this type is an interface. `fields` and `possibleTypes` are valid fields.
	/// </summary>
	[Description("Indicates this type is an interface. `fields` and `possibleTypes` are valid fields.")]
	INTERFACE = 2,

	/// <summary>
	/// Indicates this type is a union. `possibleTypes` is a valid field.
	/// </summary>
	[Description("Indicates this type is a union. `possibleTypes` is a valid field.")]
	UNION = 3,

	/// <summary>
	/// Indicates this type is an enum. `enumValues` is a valid field.
	/// </summary>
	[Description("Indicates this type is an enum. `enumValues` is a valid field.")]
	ENUM = 4,

	/// <summary>
	/// Indicates this type is an input object. `inputFields` is a valid field.
	/// </summary>
	[Description("Indicates this type is an input object. `inputFields` is a valid field.")]
	INPUT_OBJECT = 5,

	/// <summary>
	/// Indicates this type is a list. `ofType` is a valid field.
	/// </summary>
	[Description("Indicates this type is a list. `ofType` is a valid field.")]
	LIST = 6,

	/// <summary>
	/// Indicates this type is a non-null. `ofType` is a valid field.
	/// </summary>
	[Description("Indicates this type is a non-null. `ofType` is a valid field.")]
	NON_NULL = 7
}
