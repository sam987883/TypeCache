namespace GraphQL.Execution;

/// <inheritdoc cref="ExecuteNodeTreeAsync(ExecutionContext, ExecutionNode)"/>
public class ParallelExecutionStrategy : ExecutionStrategy
{
	// frequently reused objects
	private Queue<ExecutionNode>? _reusablePendingNodes;
	private List<Task>? _reusableCurrentTasks;
	private List<ExecutionNode>? _reusableCurrentNodes;

	/// <summary>
	/// Executes document nodes in parallel. Field resolvers must be designed for multi-threaded use.
	/// Nodes that return a <see cref="IDataLoaderResult"/> will execute once all other pending nodes
	/// have been completed.
	/// </summary>
	public override async Task ExecuteNodeTreeAsync(ExecutionContext context, ExecutionNode rootNode)
	{
		var pendingNodes = Interlocked.Exchange(ref _reusablePendingNodes, null) ?? new Queue<ExecutionNode>();
		pendingNodes.Enqueue(rootNode);

		var currentTasks = Interlocked.Exchange(ref _reusableCurrentTasks, null) ?? new List<Task>();
		var currentNodes = Interlocked.Exchange(ref _reusableCurrentNodes, null) ?? new List<ExecutionNode>();

		try
		{
			while (pendingNodes.Count > 0 || currentTasks.Count > 0)
			{
				while (pendingNodes.Count > 0 || currentTasks.Count > 0)
				{
					// Start executing pending nodes, while limiting the maximum number of parallel executed nodes to the set limit
					while ((context.MaxParallelExecutionCount is null || currentTasks.Count < context.MaxParallelExecutionCount)
						&& pendingNodes.Count > 0)
					{
						context.CancellationToken.ThrowIfCancellationRequested();
						var pendingNode = pendingNodes.Dequeue();
						var pendingNodeTask = ExecuteNodeAsync(context, pendingNode);
						if (pendingNodeTask.IsCompleted)
						{
							// Throw any caught exceptions
							await pendingNodeTask;

							// Node completed synchronously, so no need to add it to the list of currently executing nodes
							// instead add any child nodes to the pendingNodes queue directly here
							if (pendingNode is IParentExecutionNode parentExecutionNode)
							{
								parentExecutionNode.ApplyToChildren((node, state) => state.Enqueue(node), pendingNodes);
							}
						}
						else
						{
							// Node is actually asynchronous, so add it to the list of current tasks being executed in parallel
							currentTasks.Add(pendingNodeTask);
							currentNodes.Add(pendingNode);
						}
					}

					// Await tasks for this execution step
					await Task.WhenAll(currentTasks)
						;

					// Add child nodes to pending nodes to execute the next level in parallel
					foreach (var node in currentNodes)
					{
						if (node is IParentExecutionNode p)
						{
							p.ApplyToChildren((node, state) => state.Enqueue(node), pendingNodes);
						}
					}

					currentTasks.Clear();
					currentNodes.Clear();
				}
			}
		}
		catch (Exception original)
		{
			if (currentTasks.Count > 0)
			{
				try
				{
					await Task.WhenAll(currentTasks);
				}
				catch (Exception awaited)
				{
					if (original.Data?.IsReadOnly is false)
						original.Data["GRAPHQL_ALL_TASKS_AWAITED_EXCEPTION"] = awaited;
				}
			}
			throw;
		}
		finally
		{
			pendingNodes.Clear();
			currentTasks.Clear();
			currentNodes.Clear();

			_ = Interlocked.CompareExchange(ref _reusablePendingNodes, pendingNodes, null);
			_ = Interlocked.CompareExchange(ref _reusableCurrentTasks, currentTasks, null);
			_ = Interlocked.CompareExchange(ref _reusableCurrentNodes, currentNodes, null);
		}
	}
}
