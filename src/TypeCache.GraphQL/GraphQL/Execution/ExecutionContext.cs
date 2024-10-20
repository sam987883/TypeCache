using System.Collections.Generic;
using System.Security.Claims;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQLParser.AST;

namespace GraphQL.Execution;

/// <summary>
/// Provides a mutable instance of <see cref="IExecutionContext"/>.
/// </summary>
public class ExecutionContext : IExecutionContext
{
	/// <summary>
	/// Initializes a new instance.
	/// </summary>
	public ExecutionContext()
	{
	}

	/// <summary>
	/// Clones reusable state information from an existing instance; not any properties that
	/// hold result information. Specifically, <see cref="Errors"/>, <see cref="Metrics"/>,
	/// <see cref="OutputExtensions"/>, array pool reservations and internal reusable references
	/// are not cloned.
	/// </summary>
	public ExecutionContext(ExecutionContext context)
	{
		ExecutionStrategy = context.ExecutionStrategy;
		Document = context.Document;
		Schema = context.Schema;
		RootValue = context.RootValue;
		UserContext = context.UserContext;
		Operation = context.Operation;
		Variables = context.Variables;
		CancellationToken = context.CancellationToken;
		Listeners = context.Listeners;
		ThrowOnUnhandledException = context.ThrowOnUnhandledException;
		UnhandledExceptionDelegate = context.UnhandledExceptionDelegate;
		MaxParallelExecutionCount = context.MaxParallelExecutionCount;
		InputExtensions = context.InputExtensions;
		RequestServices = context.RequestServices;
		User = context.User;
	}

	/// <inheritdoc/>
	public IExecutionStrategy ExecutionStrategy { get; set; }

	/// <inheritdoc/>
	public GraphQLDocument Document { get; set; }

	/// <inheritdoc/>
	public ISchema Schema { get; set; }

	/// <inheritdoc/>
	public object? RootValue { get; set; }

	/// <inheritdoc/>
	public IDictionary<string, object?> UserContext { get; set; }

	/// <inheritdoc/>
	public GraphQLOperationDefinition Operation { get; set; }

	/// <inheritdoc/>
	public Variables Variables { get; set; }

	/// <inheritdoc/>
	public IList<ExecutionError> Errors { get; set; } = new List<ExecutionError>();

	/// <inheritdoc/>
	public CancellationToken CancellationToken { get; set; }

	/// <inheritdoc/>
	public Metrics Metrics { get; set; }

	/// <inheritdoc/>
	public IList<IDocumentExecutionListener> Listeners { get; set; }

	/// <inheritdoc/>
	public bool ThrowOnUnhandledException { get; set; }

	/// <inheritdoc/>
	public Func<UnhandledExceptionContext, Task> UnhandledExceptionDelegate { get; set; } = _ => Task.CompletedTask;

	/// <inheritdoc/>
	public int? MaxParallelExecutionCount { get; set; }

	/// <inheritdoc/>
	public IDictionary<string, object> InputExtensions { get; set; }

	/// <inheritdoc/>
	public IDictionary<string, object> OutputExtensions { get; set; }

	/// <inheritdoc/>
	public IServiceProvider? RequestServices { get; set; }

	/// <inheritdoc/>
	public ClaimsPrincipal? User { get; set; }

	/// <summary>
	/// Allows for an execution strategy to reuse an instance of <see cref="ReadonlyResolveFieldContext"/>.
	/// This field may be accessed by multiple threads at the same time, so
	/// access is restricted to <see cref="Interlocked.Exchange{T}(ref T, T)"/>
	/// and <see cref="Interlocked.CompareExchange{T}(ref T, T, T)"/>.
	/// </summary>
	internal ReadonlyResolveFieldContext? ReusableReadonlyResolveFieldContext;

	/// <summary>
	/// Allows for an execution strategy to reuse an instance of <see cref="Dictionary{TKey, TValue}"/>.
	/// This field may be accessed by multiple threads at the same time, so
	/// access is restricted to <see cref="Interlocked.Exchange{T}(ref T, T)"/>
	/// and <see cref="Interlocked.CompareExchange{T}(ref T, T, T)"/>.
	/// </summary>
	internal Dictionary<string, (GraphQLField field, FieldType fieldType)>? ReusableFields;
}
