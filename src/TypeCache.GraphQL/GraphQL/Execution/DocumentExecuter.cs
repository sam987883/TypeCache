using GraphQL.DI;
using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQL.Validation;
using GraphQLParser;
using GraphQLParser.AST;
using TypeCache.Extensions;
using ExecutionContext = GraphQL.Execution.ExecutionContext;

namespace GraphQL;

/// <summary>
/// <inheritdoc cref="IDocumentExecuter"/>
/// <br/><br/>
/// Default implementation for <see cref="IDocumentExecuter"/>.
/// </summary>
public class DocumentExecuter : IDocumentExecuter
{
	private readonly IDocumentBuilder _documentBuilder;
	private readonly IDocumentValidator _documentValidator;
	private readonly ExecutionDelegate _execution;
	private readonly IExecutionStrategySelector _executionStrategySelector;

	/// <summary>
	/// Initializes a new instance with default <see cref="IDocumentBuilder"/> and
	/// <see cref="IDocumentValidator"/> instances, and without document caching.
	/// </summary>
	public DocumentExecuter()
		: this(new GraphQLDocumentBuilder(), new DocumentValidator())
	{
	}

	/// <summary>
	/// Initializes a new instance with specified <see cref="IDocumentBuilder"/> and
	/// <see cref="IDocumentValidator"/>.
	/// </summary>
	public DocumentExecuter(IDocumentBuilder documentBuilder, IDocumentValidator documentValidator)
		: this(documentBuilder, documentValidator, new DefaultExecutionStrategySelector(), Array.Empty<IConfigureExecution>())
	{
	}

	/// <summary>
	/// Initializes a new instance with the specified <see cref="IDocumentBuilder"/>,
	/// <see cref="IDocumentValidator"/>, <see cref="IExecutionStrategySelector"/> and
	/// a set of <see cref="IConfigureExecution"/> instances.
	/// </summary>
	public DocumentExecuter(IDocumentBuilder documentBuilder, IDocumentValidator documentValidator, IExecutionStrategySelector executionStrategySelector, IEnumerable<IConfigureExecution> configurations)
	{
		_documentBuilder = documentBuilder ?? throw new ArgumentNullException(nameof(documentBuilder));
		_documentValidator = documentValidator ?? throw new ArgumentNullException(nameof(documentValidator));
		_executionStrategySelector = executionStrategySelector ?? throw new ArgumentNullException(nameof(executionStrategySelector));
		_execution = BuildExecutionDelegate(configurations);
	}

	private ExecutionDelegate BuildExecutionDelegate(IEnumerable<IConfigureExecution> configurations)
	{
		ExecutionDelegate execution = CoreExecuteAsync;

		// OrderBy here performs a stable sort; that is, if the sort order of two elements are equal,
		// the order of the elements are preserved. The order is reversed since each execution wraps
		// the prior configured executions. OrderByDescending is not used because that would result
		// in a different sort order when there are two executions with equal sort orders.
		foreach (var action in configurations.OrderBy(x => x.SortOrder).Reverse())
		{
			var nextExecution = execution;
			execution = async options => await action.ExecuteAsync(options, nextExecution);
		}

		return execution;
	}

	/// <inheritdoc/>
	public virtual Task<ExecutionResult> ExecuteAsync(ExecutionOptions options)
	{
		options.ThrowIfNull();

		return _execution(options);
	}

	private async Task<ExecutionResult> CoreExecuteAsync(ExecutionOptions options)
	{
		if (options.Schema is null)
			throw new InvalidOperationException("Cannot execute request if no schema is specified");

		if (options.Query is null && options.Document is null)
			return new ExecutionResult { Errors = [new QueryMissingError()] };

		options.CancellationToken.ThrowIfCancellationRequested();

		var metrics = (options.EnableMetrics ? new Metrics() : Metrics.None).Start(options.OperationName);

		ExecutionResult? result = null;
		ExecutionContext? context = null;
		bool executionOccurred = false;

		try
		{
			if (!options.Schema.Initialized)
			{
				using var schema = metrics.Subject("schema", "Initializing schema");
				options.Schema.Initialize();
			}

			var document = options.Document;
			if (document is null)
			{
				using var subject = metrics.Subject("document", "Building document");
				document = _documentBuilder.Build(options.Query!);
			}

			if (document.OperationsCount() == 0)
				throw new NoOperationError();

			var operation = GetOperation(options.OperationName, document)
				?? throw new InvalidOperationError($"Query does not contain operation '{options.OperationName}'.");
			metrics.SetOperationName(operation.Name);

			IValidationResult validationResult;
			Variables variables;
			using (metrics.Subject("document", "Validating document"))
			{
				(validationResult, variables) = await _documentValidator.ValidateAsync(
					new ValidationOptions
					{
						Document = document,
						Rules = options.ValidationRules,
						Operation = operation,
						UserContext = options.UserContext,
						RequestServices = options.RequestServices,
						User = options.User,
						CancellationToken = options.CancellationToken,
						Schema = options.Schema,
						Metrics = metrics,
						Variables = options.Variables,
						Extensions = options.Extensions,
					});
			}

			context = BuildExecutionContext(options, document, operation, variables, metrics);

			foreach (var listener in options.Listeners)
			{
				await listener.AfterValidationAsync(context, validationResult); // TODO: remove ExecutionContext or make different type ?
			}

			if (!validationResult.IsValid)
			{
				return new ExecutionResult
				{
					Errors = validationResult.Errors,
					Perf = metrics.Finish()
				};
			}

			if (context.Errors.Count > 0)
			{
				return new ExecutionResult
				{
					Errors = context.Errors,
					Perf = metrics.Finish()
				};
			}

			executionOccurred = true;

			using var marker = metrics.Subject("execution", "Executing operation");

			if (context.Listeners is not null)
			{
				foreach (var listener in context.Listeners)
				{
					await listener.BeforeExecutionAsync(context);
				}
			}

			context.ExecutionStrategy.ThrowIfNull();
			result = await context.ExecutionStrategy.ExecuteAsync(context);

			if (context.Listeners is not null)
			{
				foreach (var listener in context.Listeners)
				{
					await listener.AfterExecutionAsync(context);
				}
			}

			foreach (var error in context.Errors)
				result.Errors.Add(error);
		}
		catch (OperationCanceledException) when (options.CancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (ExecutionError ex)
		{
			result ??= new();
			result.Errors.Add(ex);
		}
		catch (Exception ex)
		{
			if (options.ThrowOnUnhandledException)
				throw;

			UnhandledExceptionContext? exceptionContext = null;
			if (options.UnhandledExceptionDelegate is not null)
			{
				exceptionContext = new UnhandledExceptionContext(context, null, ex);
				await options.UnhandledExceptionDelegate(exceptionContext);
				ex = exceptionContext.Exception;
			}

			result ??= new();
			result.Errors.Add(ex is ExecutionError executionError ? executionError : new UnhandledError(exceptionContext?.ErrorMessage ?? "Error executing document.", ex));
		}
		finally
		{
			result ??= new();
			result.Perf = metrics.Finish();
			if (executionOccurred)
				result.Executed = true;
		}

		return result;
	}

	/// <summary>
	/// Builds a <see cref="ExecutionContext"/> instance from the provided values.
	/// </summary>
	protected virtual ExecutionContext BuildExecutionContext(ExecutionOptions options, GraphQLDocument document, GraphQLOperationDefinition operation, Variables variables, Metrics metrics)
	{
		var context = new ExecutionContext
		{
			Document = document,
			Schema = options.Schema!,
			RootValue = options.Root,
			UserContext = options.UserContext,

			Operation = operation,
			Variables = variables,
			Errors = new List<ExecutionError>(),
			InputExtensions = options.Extensions,
			OutputExtensions = new Dictionary<string, object?>(),
			CancellationToken = options.CancellationToken,

			Metrics = metrics,
			Listeners = options.Listeners,
			ThrowOnUnhandledException = options.ThrowOnUnhandledException,
			UnhandledExceptionDelegate = options.UnhandledExceptionDelegate,
			MaxParallelExecutionCount = options.MaxParallelExecutionCount,
			RequestServices = options.RequestServices,
			User = options.User,
		};

		context.ExecutionStrategy = SelectExecutionStrategy(context);

		return context;
	}

	/// <summary>
	/// Returns the selected <see cref="GraphQLOperationDefinition"/> given a specified <see cref="GraphQLDocument"/> and operation name.
	/// <br/><br/>
	/// Returns <see langword="null"/> if an operation cannot be found that matches the given criteria.
	/// Returns the first operation from the document if no operation name was specified.
	/// </summary>
	protected virtual GraphQLOperationDefinition? GetOperation(string? operationName, GraphQLDocument document)
		=> document.OperationWithName(operationName);

	/// <summary>
	/// Returns an instance of an <see cref="IExecutionStrategy"/> given specified execution parameters.
	/// <br/><br/>
	/// Typically the strategy is selected based on the type of operation.
	/// <br/><br/>
	/// By default, the selection is handled by the <see cref="IExecutionStrategySelector"/> implementation passed to the
	/// constructor, which will select an execution strategy based on a set of <see cref="ExecutionStrategyRegistration"/>
	/// instances passed to it.
	/// <br/><br/>
	/// For the <see cref="DefaultExecutionStrategySelector"/> without any registrations,
	/// query operations will return a <see cref="ParallelExecutionStrategy"/> while mutation operations return a
	/// <see cref="SerialExecutionStrategy"/>. Subscription operations return a <see cref="SubscriptionExecutionStrategy"/>.
	/// </summary>
	protected virtual IExecutionStrategy SelectExecutionStrategy(ExecutionContext context)
		=> this._executionStrategySelector.Select(context);
}
