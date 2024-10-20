using GraphQL.Utilities;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace GraphQL.Types;

/// <summary>
/// Directives are used by the GraphQL runtime as a way of modifying execution
/// behavior. Type system creators will usually not create these directly.
/// </summary>
public class Directive : MetadataProvider, INamedType, IProvideDescription
{
	/// <summary>
	/// Initializes a new instance with the specified name.
	/// </summary>
	/// <param name="name">The directive name within the GraphQL schema.</param>
	internal Directive(string name)
	{
		this.Name = name;
	}

	/// <summary>
	/// Initializes a new instance with the specified parameters.
	/// </summary>
	/// <param name="name">The directive name within the GraphQL schema.</param>
	/// <param name="locations">A list of locations where the directive can be applied.</param>
	public Directive(string name, IEnumerable<DirectiveLocation> locations)
	{
		locations.ThrowIfNull();
		if (!this.Locations.Any())
			throw new ArgumentException("Directive must have locations", nameof(locations));

		this.Name = name;
		this.Locations.AddRange(locations);
	}

	/// <summary>
	/// Initializes a new instance with the specified parameters.
	/// </summary>
	/// <param name="name">The directive name within the GraphQL schema.</param>
	/// <param name="locations">A list of locations where the directive can be applied.</param>
	public Directive(string name, params DirectiveLocation[] locations)
	{
		locations.ThrowIfNull();
		if (locations.Length == 0)
			throw new ArgumentException("Directive must have locations", nameof(locations));

		this.Name = name;
		this.Locations.AddRange(locations);
	}

	public QueryArgument? this[string argument] => this.Arguments.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(argument));

	public QueryArgument? this[GraphQLName argument] => this.Arguments.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(argument.StringValue));

	/// <inheritdoc/>
	public string Name { get; set; }

	/// <inheritdoc />
	public override string ToString() => Name;

	/// <summary>
	/// Gets or sets the description of the directive.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Indicates whether the directive and its usages for schema elements should return in response
	/// to an introspection request. By default (null) if the directive has all its locations of
	/// type ExecutableDirectiveLocation, only then it will be present in the introspection response.
	/// </summary>
	public virtual bool? Introspectable => null;

	/// <summary>
	/// Indicates if the directive may be used repeatedly at a single location.
	/// <br/><br/>
	/// Repeatable directives are often useful when the same directive
	/// should be used with different arguments at a single location,
	/// especially in cases where additional information needs to be
	/// provided to a type or schema extension via a directive
	/// </summary>
	public bool Repeatable { get; set; }

	/// <summary>
	/// Gets or sets a list of arguments for the directive.
	/// </summary>
	public QueryArgument[] Arguments { get; set; } = Array<QueryArgument>.Empty;

	/// <summary>
	/// Returns a list of locations where the directive can be applied.
	/// </summary>
	public List<DirectiveLocation> Locations { get; } = new List<DirectiveLocation>();

	/// <summary>
	/// Validates given <paramref name="applied"/> directive against this directive graph type.
	/// </summary>
	public virtual void Validate(AppliedDirective applied)
	{
	}
}
