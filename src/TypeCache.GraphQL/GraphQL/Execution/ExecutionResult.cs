using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQLParser;
using GraphQLParser.AST;
using ExecutionContext = GraphQL.Execution.ExecutionContext;

namespace GraphQL;

/// <summary>
/// Represents the result of an execution.
/// </summary>
public class ExecutionResult
{
	/// <summary>
	/// Indicates if the operation included execution. If an error was encountered BEFORE execution begins,
	/// the data entry SHOULD NOT be present in the result. If an error was encountered DURING the execution
	/// that prevented a valid response, the data entry in the response SHOULD BE <see langword="null"/>.
	/// </summary>
	public bool Executed { get; set; }

	/// <summary>
	/// Returns the data from the graph resolvers. This property is serialized as part of the GraphQL json response.
	/// Should be set to <see langword="null"/> for subscription results.
	/// </summary>
	public object? Data { get; set; }

	/// <summary>
	/// Gets or sets a dictionary of returned subscription fields along with their
	/// response streams as <see cref="IObservable{T}"/> implementations.
	/// Should be set to <see langword="null"/> for query or mutation results.
	/// According to the GraphQL specification this dictionary should have exactly one item.
	/// </summary>
	public IDictionary<string, IObservable<ExecutionResult>>? Streams { get; set; }

	/// <summary>
	/// Returns a set of errors that occurred during any stage of processing (parsing, validating, executing, etc.). This property is serialized as part of the GraphQL json response.
	/// </summary>
	public IList<ExecutionError> Errors { get; set; } = new List<ExecutionError>();

	/// <summary>
	/// Returns the original GraphQL query.
	/// </summary>
	public ROM Query { get; set; }

	/// <summary>
	/// Returns the parsed GraphQL request.
	/// </summary>
	public GraphQLDocument? Document { get; set; }

	/// <summary>
	/// Returns the GraphQL operation that is being executed.
	/// </summary>
	public GraphQLOperationDefinition? Operation { get; set; }

	/// <summary>
	/// Returns the performance metrics (Apollo Tracing) when enabled by <see cref="ExecutionOptions.EnableMetrics"/>.
	/// </summary>
	public PerfRecord[]? Perf { get; set; }

	/// <summary>
	/// Returns additional user-defined data; see <see cref="IExecutionContext.OutputExtensions"/> and <see cref="IResolveFieldContext.OutputExtensions"/>. This property is serialized as part of the GraphQL json response.
	/// </summary>
	public IDictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>();

	/// <summary>
	/// Initializes a new instance with all properties set to their defaults.
	/// </summary>
	public ExecutionResult()
	{
	}

	/// <summary>
	/// Initializes a new instance with the <see cref="Query"/>, <see cref="Document"/>,
	/// <see cref="Operation"/> and <see cref="Extensions"/> properties set from the
	/// specified <see cref="ExecutionContext"/>.
	/// </summary>
	internal ExecutionResult(ExecutionContext context)
	{
		Query = context.Document.Source;
		Document = context.Document;
		Operation = context.Operation;
		Extensions = context.OutputExtensions;
	}
}
