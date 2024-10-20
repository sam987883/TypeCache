// Copyright (c) 2021 Samuel Abraham

using GraphQL.Execution;
using GraphQL.Validation;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Document;

internal sealed class DocumentExecutionListener(ILogger<DocumentExecutionListener>? logger) : IDocumentExecutionListener
{
	/// <inheritdoc/>
	public Task AfterValidationAsync(IExecutionContext context, IValidationResult validationResult)
		=> Task.Run(() =>
		{
			if (!validationResult.IsValid && logger is not null)
			{
				validationResult.Errors.AsArray().ForEach(_ => logger.LogError(_, _.InnerException?.Message ?? _.Message));
				validationResult.Errors.AsArray().ForEach(_ => Console.WriteLine(_.InnerException?.Message ?? _.Message));
			}
		}, context.CancellationToken);

	/// <inheritdoc/>
	public Task BeforeExecutionAsync(IExecutionContext context)
		=> Task.CompletedTask;

	/// <inheritdoc/>
	public Task AfterExecutionAsync(IExecutionContext context)
		=> Task.CompletedTask;
}
