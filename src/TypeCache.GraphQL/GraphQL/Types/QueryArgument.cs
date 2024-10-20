using GraphQL.Utilities;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;

namespace GraphQL.Types;

/// <summary>
/// Represents an argument to a field or directive.
/// </summary>
[DebuggerDisplay("{Name,nq}: {ResolvedType,nq}")]
public class QueryArgument : MetadataProvider, IHaveDefaultValue, IProvideDescription, IProvideDeprecationReason
{
	/// <summary>
	/// Initializes a new instance of the argument.
	/// </summary>
	/// <param name="resolvedType">The resolved graph type of the argument.</param>
	public QueryArgument(string name, IGraphType resolvedType, object? defaultValue = null)
	{
		name.ThrowIfBlank();
		resolvedType.ThrowIfNull();

		NameValidator.ValidateName(name, NamedElement.Argument);

		if (!resolvedType.IsGraphQLTypeReference() && !resolvedType.IsInputType())
			throw new ArgumentOutOfRangeException(nameof(this.ResolvedType),
				$"'{resolvedType.GetType().GetTypeName()}' is not a valid input type. {nameof(QueryArgument)} must be one of the input types: ScalarGraphType, EnumerationGraphType or IInputObjectGraphType.");

		this.Name = name;
		this.ResolvedType = resolvedType;
		this.DefaultValue = defaultValue;
	}

	/// <summary>
	/// Initializes a new instance of the argument.
	/// </summary>
	/// <param name="type">The graph type of the argument.</param>
	public QueryArgument(string name, Type type, object? defaultValue = null)
	{
		name.ThrowIfBlank();
		type.ThrowIfNull();

		NameValidator.ValidateName(name, NamedElement.Argument);

		if (!typeof(IGraphType).IsAssignableFrom(type))
			throw new ArgumentOutOfRangeException(nameof(type), Invariant($"{nameof(QueryArgument)} type is required and must derive from {nameof(IGraphType)}."));

		this.Name = name;
		this.Type = type;
		this.DefaultValue = defaultValue;
	}

	public override string ToString()
		=> this.DefaultValue is not null
			? $"{this.Name}: {this.ResolvedType}"
			: $"{this.Name}: {this.ResolvedType} = {this.ResolvedType!.ToString(this.DefaultValue)}";

	/// <summary>
	/// Gets or sets the name of the argument.
	/// </summary>
	public string Name { get; internal set; }

	/// <summary>
	/// Gets or sets the description of the argument.
	/// </summary>
	public string? Description { get; set; }

	/// <inheritdoc/>
	public string? DeprecationReason
	{
		get => this.GetDeprecationReason();
		set => this.SetDeprecationReason(value);
	}

	/// <summary>
	/// Gets or sets the default value of the argument.
	/// </summary>
	public object? DefaultValue { get; set; }

	/// <summary>
	/// Returns the graph type of this argument.
	/// </summary>
	public IGraphType? ResolvedType { get; set; }

	/// <inheritdoc/>
	public Type? Type { get; internal set; }
}
