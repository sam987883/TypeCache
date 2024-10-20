using GraphQL.Types;

namespace GraphQL.Execution;

/// <summary>
/// Represents information about directive that has been provided in the GraphQL query request.
/// </summary>
public class DirectiveInfo
{
	/// <summary>
	/// Creates an instance of <see cref="DirectiveInfo"/> with the specified
	/// directive definition and directive arguments.
	/// </summary>
	public DirectiveInfo(Directive directive, IDictionary<string, ArgumentValue> arguments)
	{
		Directive = directive;
		Arguments = arguments;
	}

	/// <summary>
	/// Directive definition.
	/// </summary>
	public Directive Directive { get; }

	/// <summary>
	/// Dictionary of directive arguments.
	/// </summary>
	public IDictionary<string, ArgumentValue> Arguments { get; }
}
