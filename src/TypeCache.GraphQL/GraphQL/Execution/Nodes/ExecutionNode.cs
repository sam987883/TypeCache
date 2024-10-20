using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.Execution;

/// <summary>
/// Represents a node to be executed.
/// </summary>
public abstract class ExecutionNode
{
	/// <summary>
	/// Returns the parent node, or <see langword="null"/> if this is the root node.
	/// </summary>
	public ExecutionNode Parent { get; }

	/// <summary>
	/// Returns the graph type of this node, unwrapped if it is a <see cref="NonNullGraphType"/>.
	/// Array nodes will be a <see cref="ListGraphType"/> instance.
	/// </summary>
	public IGraphType GraphType { get; }

	/// <summary>
	/// Returns the AST field of this node.
	/// </summary>
	public GraphQLField Field { get; }

	/// <summary>
	/// Returns the graph's field type of this node.
	/// </summary>
	public FieldType FieldDefinition { get; }

	/// <summary>
	/// For child array item nodes of a <see cref="ListGraphType"/>, returns the index of this array item within the field; otherwise, <see langword="null"/>.
	/// </summary>
	public int? IndexInParentNode { get; }

	/// <summary>
	/// Returns the underlying graph type of this node, retaining the <see cref="NonNullGraphType"/> wrapping if applicable.
	/// For child nodes of an array execution node, this property unwraps the <see cref="ListGraphType"/> instance and returns
	/// the underlying graph type, retaining the <see cref="NonNullGraphType"/> wrapping if applicable.
	/// </summary>
	internal IGraphType? ResolvedType
		=> IndexInParentNode.HasValue
			? ((ListGraphType?)Parent!.GraphType)?.ResolvedType
			: FieldDefinition?.ResolvedType;

	/// <summary>
	/// Returns the AST field alias, if specified, or AST field name otherwise.
	/// </summary>
	public string? Name => Field?.Alias?.Name.StringValue ?? FieldDefinition?.Name; // TODO: allocation in case of alias

	/// <summary>
	/// Sets or returns the result of the execution node. May return a <see cref="IDataLoaderResult"/> if a node returns a data loader
	/// result that has not yet finished executing.
	/// </summary>
	public object? Result { get; set; }

	/// <summary>
	/// Returns the parent node's result.
	/// </summary>
	public virtual object? Source => Parent?.Result;

	/// <summary>
	/// Initializes an instance of <see cref="ExecutionNode"/> with the specified values
	/// </summary>
	/// <param name="parent">The parent node, or <see langword="null"/> if this is the root node</param>
	/// <param name="graphType">The graph type of this node, unwrapped if it is a <see cref="NonNullGraphType"/>. Array nodes will be a <see cref="ListGraphType"/> instance.</param>
	/// <param name="field">The AST field of this node</param>
	/// <param name="fieldDefinition">The graph's field type of this node</param>
	/// <param name="indexInParentNode">For child array item nodes of a <see cref="ListGraphType"/>, the index of this array item within the field; otherwise, <see langword="null"/></param>
	protected ExecutionNode(ExecutionNode parent, IGraphType graphType, GraphQLField field, FieldType fieldDefinition, int? indexInParentNode)
	{
		Debug.Assert(field?.Name == fieldDefinition?.Name); // ? for RootExecutionNode

		Parent = parent;
		GraphType = graphType;
		Field = field!;
		FieldDefinition = fieldDefinition!;
		IndexInParentNode = indexInParentNode;
	}

	/// <summary>
	/// Returns an object that represents the result of this node.
	/// </summary>
	public abstract object? ToValue();

	/// <summary>
	/// Prepares this node and children nodes for serialization. Returns <see langword="true"/> if this node should return <see langword="null"/>.
	/// </summary>
	public virtual bool PropagateNull()
		=> ToValue() is null;

	/// <summary>
	/// Returns the parent graph type of this node.
	/// </summary>
	public IObjectGraphType? GetParentType(ISchema schema)
		=> Parent?.GraphType switch
		{
			IObjectGraphType objectType => objectType,
			IAbstractGraphType abstractType => abstractType.GetObjectType(Parent!.Result!, schema),
			_ => null
		};

	/// <summary>
	/// The path for the current node within the query.
	/// </summary>
	public IEnumerable<object> Path => GeneratePath(preferAlias: false);

	/// <summary>
	/// The path for the current node within the response.
	/// </summary>
	public IEnumerable<object> ResponsePath => GeneratePath(preferAlias: true);

	private static object GetObjectIndex(int index)
		=> index;

	private IEnumerable<object> GeneratePath(bool preferAlias)
	{
		var node = this;
		var count = 0;
		while (node is not RootExecutionNode)
		{
			node = node.Parent!;
			++count;
		}

		if (count == 0)
			return Array.Empty<object>();

		var pathList = new object[count];
		var index = count;
		node = this;
		while (node is not RootExecutionNode)
		{
			pathList[--index] = node.IndexInParentNode.HasValue
				? GetObjectIndex(node.IndexInParentNode.Value)
				: (preferAlias ? node.Name! : node.FieldDefinition.Name);
			node = node.Parent!;
		}

		return pathList;
	}
}
