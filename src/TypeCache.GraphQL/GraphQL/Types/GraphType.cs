using GraphQL.Utilities;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;

namespace GraphQL.Types;

/// <summary>
/// Represents a graph type.
/// </summary>
public abstract class GraphType : MetadataProvider, IGraphType
{
	private string _name;
	private bool _initialized;

	protected GraphType()
	{
		if (!IsTypeModifier) // specification requires name must be null for these types
		{
			// GraphType must always have a valid name so set it to default name in constructor
			// and skip validation only for well-known types including introspection.
			// This name can be always changed later to any valid value.
			SetName(this.GetType().GraphQLName(), validate: this.GetType().Assembly != typeof(GraphType).Assembly);
		}
	}

	/// <inheritdoc/>
	public virtual void Initialize(ISchema schema)
	{
		if (_initialized)
			throw new InvalidOperationException($"This graph type '{GetType().GetTypeName()}' with name '{Name}' has already been initialized. Make sure that you do not use the same instance of a graph type in multiple schemas. It may be so if you registered this graph type as singleton; see https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/ for more info.");

		_initialized = true;
	}

	private bool IsTypeModifier => this is ListGraphType || this is NonNullGraphType; // lgtm [cs/type-test-of-this]

	internal void SetName(string name, bool validate)
	{
		if (_name != name)
		{
			if (validate)
			{
				NameValidator.ValidateName(name, NamedElement.Type);

				if (IsTypeModifier)
					throw new ArgumentOutOfRangeException(nameof(name), "A type modifier (List, NonNull) name must be null");
			}

			_name = name;
		}
	}

	/// <inheritdoc/>
	public string Name
	{
		get => _name;
		set => SetName(value, validate: true);
	}

	/// <inheritdoc/>
	public string? Description { get; set; }

	/// <inheritdoc/>
	public string? DeprecationReason
	{
		get => this.GetDeprecationReason();
		set => this.SetDeprecationReason(value);
	}

	/// <inheritdoc />
	public override string ToString() => Name;

	/// <summary>
	/// Determines if the name of the specified graph type is equal to the name of this graph type.
	/// </summary>
	protected bool Equals(IGraphType other)
		=> this.Name.EqualsIgnoreCase(other.Name);

	/// <summary>
	/// Determines if the graph type is equal to the specified object,
	/// or if the name of the specified graph type is equal to the name of this graph type.
	/// </summary>
	public override bool Equals(object? obj)
		=> obj switch
		{
			null => false,
			_ when ReferenceEquals(this, obj) => true,
			_ when obj.GetType() != this.GetType() => false,
			IGraphType graphType => this.Equals(graphType),
			_ => false
		};

	/// <inheritdoc />
	public override int GetHashCode()
		=> this.Name?.GetHashCode() ?? 0;
}
