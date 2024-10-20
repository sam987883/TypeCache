using GraphQL.Validation.Rules;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace GraphQL.Validation;

/// <summary>
/// Validates a document against a set of validation rules and returns a list of the errors found.
/// </summary>
public interface IDocumentValidator
{
	/// <inheritdoc cref="IDocumentValidator"/>
	Task<(IValidationResult validationResult, Variables variables)> ValidateAsync(in ValidationOptions options);
}

/// <inheritdoc/>
public class DocumentValidator : IDocumentValidator
{
	// frequently reused objects
	private ValidationContext? _reusableValidationContext;

	/// <summary>
	/// Returns the default set of validation rules.
	/// </summary>
	public static readonly IEnumerable<IValidationRule> CoreRules =
	[
		Singleton<UniqueOperationNames>.Instance,
		Singleton<LoneAnonymousOperation>.Instance,
		Singleton<SingleRootFieldSubscriptions>.Instance,
		Singleton<KnownTypeNames>.Instance,
		Singleton<FragmentsOnCompositeTypes>.Instance,
		Singleton<VariablesAreInputTypes>.Instance,
		Singleton<ScalarLeafs>.Instance,
		Singleton<FieldsOnCorrectType>.Instance,
		Singleton<UniqueFragmentNames>.Instance,
		Singleton<KnownFragmentNames>.Instance,
		Singleton<NoUnusedFragments>.Instance,
		Singleton<PossibleFragmentSpreads>.Instance,
		Singleton<NoFragmentCycles>.Instance,
		Singleton<NoUndefinedVariables>.Instance,
		Singleton<NoUnusedVariables>.Instance,
		Singleton<UniqueVariableNames>.Instance,
		Singleton<KnownDirectivesInAllowedLocations>.Instance,
		Singleton<UniqueDirectivesPerLocation>.Instance,
		Singleton<KnownArgumentNames>.Instance,
		Singleton<UniqueArgumentNames>.Instance,
		Singleton<ArgumentsOfCorrectType>.Instance,
		Singleton<ProvidedNonNullArguments>.Instance,
		Singleton<DefaultValuesOfCorrectType>.Instance,
		Singleton<VariablesInAllowedPosition>.Instance,
		Singleton<UniqueInputFieldNames>.Instance,
		Singleton<OverlappingFieldsCanBeMerged>.Instance,
	];

	/// <inheritdoc/>
	public Task<(IValidationResult validationResult, Variables variables)> ValidateAsync(in ValidationOptions options)
	{
		options.Schema.Initialize();

		var context = Interlocked.Exchange(ref _reusableValidationContext, null) ?? new ValidationContext();
		context.TypeInfo = new TypeInfo(options.Schema);
		context.Schema = options.Schema;
		context.Document = options.Document;
		context.UserContext = options.UserContext;
		context.Variables = options.Variables;
		context.Extensions = options.Extensions;
		context.Operation = options.Operation;
		context.Metrics = options.Metrics;
		context.RequestServices = options.RequestServices;
		context.User = options.User;
		context.CancellationToken = options.CancellationToken;

		return ValidateAsyncCoreAsync(context, options.Rules ?? CoreRules);
	}

	private async Task<(IValidationResult validationResult, Variables variables)> ValidateAsyncCoreAsync(ValidationContext context, IEnumerable<IValidationRule> rules)
	{
		try
		{
			Variables? variables = null;
			var variableVisitors = new List<IVariableVisitor>();

			if (rules.Any())
			{
				var visitors = new List<INodeVisitor> { context.TypeInfo };

				if (rules is List<IValidationRule> list) //TODO:allocation - optimization for List+Enumerator<IvalidationRule>
				{
					foreach (var rule in list)
					{
						variableVisitors.AddIfNotNull((rule as IVariableVisitorProvider)?.GetVisitor(context));
						visitors.AddIfNotNull(await rule.ValidateAsync(context));
					}
				}
				else
				{
					foreach (var rule in rules)
					{
						variableVisitors.AddIfNotNull((rule as IVariableVisitorProvider)?.GetVisitor(context));
						visitors.AddIfNotNull(await rule.ValidateAsync(context));
					}
				}

				await new BasicVisitor(visitors).VisitAsync(context.Document, new BasicVisitor.State(context));
			}

			// can report errors even without rules enabled
			(variables, var errors) = await context.GetVariablesValuesAsync(variableVisitors.Count switch
			{
				0 => null,
				1 => variableVisitors[0],
				_ => new CompositeVariableVisitor(variableVisitors)
			});

			if (errors is not null)
			{
				foreach (var error in errors)
					context.ReportError(error);
			}

			return context.HasErrors
				? (new ValidationResult(context.Errors), variables)
				: (new SuccessfullyValidatedResult(), variables);
		}
		finally
		{
			if (!context.HasErrors)
			{
				context.Reset();
				_ = Interlocked.CompareExchange(ref _reusableValidationContext, context, null);
			}
		}
	}
}
