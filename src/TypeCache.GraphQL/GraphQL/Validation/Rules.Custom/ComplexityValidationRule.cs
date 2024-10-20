using System.Threading.Tasks;
using GraphQL.Validation.Complexity;
using GraphQL.Validation.Errors.Custom;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules.Custom;

/// <summary>
/// Analyzes a document to determine if its complexity exceeds a threshold.
/// </summary>
public class ComplexityValidationRule : IValidationRule
{
    private readonly ComplexityConfiguration _complexityConfiguration;
    private readonly IComplexityAnalyzer _complexityAnalyzer;

    /// <summary>
    /// Initializes an instance with the specified complexity configuration.
    /// </summary>
    public ComplexityValidationRule(ComplexityConfiguration complexityConfiguration)
    {
		_complexityConfiguration = complexityConfiguration;
		_complexityAnalyzer = new ComplexityAnalyzer();
	}

    /// <inheritdoc/>
    public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context)
        => new(new PostponeComplexityValidationVisitor(_complexityConfiguration, _complexityAnalyzer));

    // https://github.com/graphql-dotnet/graphql-dotnet/issues/3527
    private sealed class PostponeComplexityValidationVisitor : INodeVisitor
    {
        private readonly ComplexityConfiguration _complexityConfiguration;
        private readonly IComplexityAnalyzer _complexityAnalyzer;

        public PostponeComplexityValidationVisitor(ComplexityConfiguration complexityConfiguration, IComplexityAnalyzer complexityAnalyzer)
        {
            _complexityConfiguration = complexityConfiguration;
            _complexityAnalyzer = complexityAnalyzer;
        }

        public ValueTask EnterAsync(ASTNode node, ValidationContext context) => default;

        public ValueTask LeaveAsync(ASTNode node, ValidationContext context)
        {
            // Complexity analysis should run at the very end.
            if (node is GraphQLDocument)
            {
                // Fast return here to avoid all possible problems with complexity analysis.
                // For example, document may contain fragment cycles, see https://github.com/graphql-dotnet/graphql-dotnet/issues/3527
                // Note, that ComplexityValidationRule/ComplexityAnalyzer still checks for fragment cycles since there may be no standard rules configured.
                if (context.HasErrors)
                    return default;

                try
                {
                    using (context.Metrics.Subject("document", "Analyzing complexity"))
                        _complexityAnalyzer.Validate(context.Document, _complexityConfiguration, context.Schema);
                }
                catch (ComplexityError ex)
                {
                    context.ReportError(ex);
                }
            }

            return default;
        }
    }
}
