using System.Security.Claims;
using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQLParser.AST;

namespace GraphQL;

/// <summary>
/// A mutable implementation of <see cref="IResolveFieldContext"/>
/// </summary>
public class ResolveFieldContext : IResolveFieldContext<object?>
{
	/// <inheritdoc/>
	public GraphQLField FieldAst { get; set; }

	/// <inheritdoc/>
	public IFieldType FieldDefinition { get; set; }

	/// <inheritdoc/>
	public IObjectGraphType ParentType { get; set; }

	/// <inheritdoc/>
	public IResolveFieldContext? Parent { get; set; }

	/// <inheritdoc/>
	public IDictionary<string, ArgumentValue>? Arguments { get; set; }

	/// <inheritdoc/>
	public IDictionary<string, DirectiveInfo>? Directives { get; set; }

	/// <inheritdoc/>
	public object? RootValue { get; set; }

	/// <inheritdoc/>
	public IDictionary<string, object?> UserContext { get; set; }

	/// <inheritdoc/>
	public object? Source { get; set; }

	/// <inheritdoc/>
	public ISchema Schema { get; set; }

	/// <inheritdoc/>
	public GraphQLDocument Document { get; set; }

	/// <inheritdoc/>
	public GraphQLOperationDefinition Operation { get; set; }

	/// <inheritdoc/>
	public Variables Variables { get; set; }

	/// <inheritdoc/>
	public CancellationToken CancellationToken { get; set; }

	/// <inheritdoc/>
	public Metrics Metrics { get; set; }

	/// <inheritdoc/>
	public IList<ExecutionError> Errors { get; set; }

	/// <inheritdoc/>
	public IEnumerable<object> Path { get; set; }

	/// <inheritdoc/>
	public IEnumerable<object> ResponsePath { get; set; }

	/// <inheritdoc/>
	public Dictionary<string, (GraphQLField Field, FieldType FieldType)>? SubFields { get; set; }

	/// <inheritdoc/>
	public IServiceProvider? RequestServices { get; set; }

	/// <inheritdoc/>
	public IDictionary<string, object> InputExtensions { get; set; } = new Dictionary<string, object>();

	/// <inheritdoc/>
	public IDictionary<string, object> OutputExtensions { get; set; } = new Dictionary<string, object>();

	/// <inheritdoc/>
	public ClaimsPrincipal? User { get; set; }

	/// <summary>
	/// Initializes a new instance with all fields set to their default values.
	/// </summary>
	public ResolveFieldContext() { }

	/// <summary>
	/// Clone the specified <see cref="IResolveFieldContext"/>.
	/// </summary>
	public ResolveFieldContext(IResolveFieldContext context)
	{
		Source = context.Source;
		FieldAst = context.FieldAst;
		FieldDefinition = context.FieldDefinition;
		ParentType = context.ParentType;
		Parent = context.Parent;
		Arguments = context.Arguments;
		Directives = context.Directives;
		Schema = context.Schema;
		Document = context.Document;
		RootValue = context.RootValue;
		UserContext = context.UserContext;
		User = context.User;
		Operation = context.Operation;
		Variables = context.Variables;
		CancellationToken = context.CancellationToken;
		Metrics = context.Metrics;
		Errors = context.Errors;
		SubFields = context.SubFields;
		Path = context.Path;
		ResponsePath = context.ResponsePath;
		RequestServices = context.RequestServices;
		InputExtensions = context.InputExtensions;
		OutputExtensions = context.OutputExtensions;
	}
}

/// <inheritdoc cref="ResolveFieldContext"/>
public class ResolveFieldContext<TSource> : ResolveFieldContext, IResolveFieldContext<TSource>
{
	/// <inheritdoc cref="ResolveFieldContext()"/>
	public ResolveFieldContext()
	{
	}

	/// <summary>
	/// Clone the specified <see cref="IResolveFieldContext"/>
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if the <see cref="IResolveFieldContext.Source"/> property cannot be cast to <typeparamref name="TSource"/></exception>
	public ResolveFieldContext(IResolveFieldContext context) : base(context)
	{
		if (context.Source is not null && context.Source is not TSource)
			throw new ArgumentException($"IResolveFieldContext.Source must be an instance of type '{typeof(TSource).Name}'", nameof(context));
	}

	/// <inheritdoc cref="ResolveFieldContext.Source"/>
	public new TSource Source
	{
		get => (TSource)base.Source!;
		set => base.Source = value;
	}
}
