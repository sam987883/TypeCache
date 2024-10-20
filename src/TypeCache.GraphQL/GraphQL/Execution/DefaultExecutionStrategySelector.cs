using System.Collections.Generic;
using System.Linq;
using GraphQLParser.AST;
using TypeCache.Utilities;

namespace GraphQL.Execution;

/// <inheritdoc cref="IExecutionStrategySelector"/>
public class DefaultExecutionStrategySelector : IExecutionStrategySelector
{
	private readonly ExecutionStrategyRegistration[] _registrations;

	/// <summary>
	/// Initializes an instance that only returns the default registrations;
	/// <see cref="ParallelExecutionStrategy"/> for <see cref="OperationType.Query"/> and
	/// <see cref="SerialExecutionStrategy"/> for <see cref="OperationType.Mutation"/>.
	/// </summary>
	public DefaultExecutionStrategySelector()
		: this(Array.Empty<ExecutionStrategyRegistration>())
	{
	}

	/// <summary>
	/// Initializes a new instance with the specified registrations.
	/// If no registration is specified for <see cref="OperationType.Query"/>, returns <see cref="ParallelExecutionStrategy"/>.
	/// If no registration is specified for <see cref="OperationType.Mutation"/>, returns <see cref="SerialExecutionStrategy"/>.
	/// </summary>
	public DefaultExecutionStrategySelector(IEnumerable<ExecutionStrategyRegistration> registrations)
	{
		ArgumentNullException.ThrowIfNull(registrations);

		this._registrations = registrations.ToArray();
	}

	/// <inheritdoc/>
	public virtual IExecutionStrategy Select(ExecutionContext context)
	{
		var operationType = context.Operation.Operation;

		var registration = this._registrations.FirstOrDefault(_ => _.Operation == operationType);
		if (registration is not null)
			return registration.Strategy;

		// No matching registration, so return default implementation.
		return operationType switch
		{
			OperationType.Query => Singleton<ParallelExecutionStrategy>.Instance,
			OperationType.Mutation => Singleton<SerialExecutionStrategy>.Instance,
			OperationType.Subscription => Singleton<SubscriptionExecutionStrategy>.Instance,
			_ => throw new UnreachableException($"Unexpected OperationType '{operationType}'.")
		};
	}
}
