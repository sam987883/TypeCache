using TypeCache.Extensions;

namespace GraphQL.Types;

/// <summary>
/// Represents a GraphQL union graph type.
/// </summary>
public class UnionGraphType : GraphType, IAbstractGraphType
{
	private readonly ISet<Type> _types = new HashSet<Type>();

	/// <inheritdoc/>
	public ISet<IObjectGraphType> PossibleTypes { get; } = new HashSet<IObjectGraphType>();

	/// <inheritdoc/>
	public Func<object, IObjectGraphType?>? ResolveType { get; set; }

	/// <inheritdoc/>
	public void AddPossibleType(IObjectGraphType type)
		=> this.PossibleTypes.Add(type);

	/// <summary>
	/// Gets or sets a list of graph types that this union represents.
	/// </summary>
	public IEnumerable<Type> Types
	{
		get => this._types;
		set
		{
			this._types.Clear();
			this._types.Union(value);
		}
	}

	/// <summary>
	/// Adds a graph type to the list of graph types that this union represents.
	/// </summary>
	public void Type<TType>()
		where TType : IObjectGraphType
		=> this._types.Add(typeof(TType));

	/// <inheritdoc cref="Type{TType}"/>
	public void Type(Type type)
	{
		type.ThrowIfNull();

		if (!typeof(IObjectGraphType).IsAssignableFrom(type))
			throw new ArgumentException($"Added union type '{type.Name}' must implement {nameof(IObjectGraphType)}", nameof(type));

		this._types.Add(type);
	}
}
