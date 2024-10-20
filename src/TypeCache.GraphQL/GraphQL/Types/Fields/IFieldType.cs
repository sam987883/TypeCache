using GraphQLParser.AST;
using TypeCache.Extensions;

namespace GraphQL.Types;

/// <summary>
/// Represents a field of a graph type.
/// </summary>
public interface IFieldType : IEquatable<IFieldType>, IHaveDefaultValue, IProvideMetadata, IProvideDescription, IProvideDeprecationReason
{
	/// <summary>
	/// Gets or sets the name of the field.
	/// </summary>
	string Name { get; set; }

	/// <summary>
	/// Gets or sets a list of arguments for the field.
	/// </summary>
	QueryArgument[] Arguments { get; set; }

	Type? Type { get; }

	QueryArgument? this[string argument] => this.Arguments.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(argument));

	QueryArgument? this[GraphQLName argument] => this.Arguments.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(argument.StringValue));

}
