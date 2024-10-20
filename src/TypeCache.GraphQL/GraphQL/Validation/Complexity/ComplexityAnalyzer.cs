using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using GraphQL.Validation.Errors.Custom;
using GraphQLParser;
using GraphQLParser.AST;

namespace GraphQL.Validation.Complexity
{
	/// <summary>
	/// The default complexity analyzer.
	/// </summary>
	[Obsolete("Please write a custom complexity analyzer as a validation rule. This class will be removed in v8.")]
    public class ComplexityAnalyzer : IComplexityAnalyzer
    {
        /// <inheritdoc/>
        public void Validate(GraphQLDocument document, ComplexityConfiguration complexityParameters, ISchema? schema = null)
        {
            if (complexityParameters is null)
                return;

            var complexityResult = Analyze(document, complexityParameters.FieldImpact ?? 2.0f, complexityParameters.MaxRecursionCount, schema).Result;

            Analyzed(document, complexityParameters, complexityResult);

			if (complexityResult.Complexity > complexityParameters.MaxComplexity)
			{
				var map = complexityResult.ComplexityMap.OrderByDescending(pair => pair.Value).First();
				var name = map.Key switch
				{
					GraphQLField f => f.Name.StringValue,
					GraphQLFragmentSpread fs => fs.FragmentName.Name.StringValue,
					_ => throw new NotSupportedException(map.Key.ToString()),
				};

				throw new ComplexityError(
					$"Query is too complex to execute. Complexity is {complexityResult.Complexity}, maximum allowed on this endpoint is {complexityParameters.MaxComplexity}. The field with the highest complexity is '{name}' with value {map.Value}.");
			}

            if (complexityResult.TotalQueryDepth > complexityParameters.MaxDepth)
                throw new ComplexityError(
                    $"Query is too nested to execute. Depth is {complexityResult.TotalQueryDepth} levels, maximum allowed on this endpoint is {complexityParameters.MaxDepth}.");
        }

        /// <summary>
        /// Executes after the complexity analysis has completed, before comparing results to the complexity configuration parameters.
        /// This method is made to be able to access the calculated <see cref="ComplexityResult"/> and handle it, for example, for logging.
        /// </summary>
        protected virtual void Analyzed(GraphQLDocument document, ComplexityConfiguration complexityParameters, ComplexityResult complexityResult)
        {
#if DEBUG
            Debug.WriteLine($"Complexity: {complexityResult.Complexity}");
            Debug.WriteLine($"Sum(Query depth across all subqueries) : {complexityResult.TotalQueryDepth}");
            foreach (var node in complexityResult.ComplexityMap)
                Debug.WriteLine($"{node.Key} : {node.Value}");
#endif
        }

		/// <summary>
		/// Analyzes the complexity of a document.
		/// </summary>
		internal async ValueTask<ComplexityResult> Analyze(GraphQLDocument doc, double avgImpact, int maxRecursionCount, ISchema? schema = null)
		{
			if (avgImpact <= 1)
				throw new ArgumentOutOfRangeException(nameof(avgImpact));

			var context = new AnalysisContext
			{
				MaxRecursionCount = maxRecursionCount,
				AvgImpact = avgImpact,
				CurrentEndNodeImpact = 1d
			};
			var visitor = schema is null ? new ComplexityVisitor() : new ComplexityVisitor(schema);

			// https://github.com/graphql-dotnet/graphql-dotnet/issues/3030
			// Sort fragment definitions so that independent fragments go in front.
			var dependencies = BuildDependencies(doc);
			var orderedFragments = new List<GraphQLFragmentDefinition>(dependencies.Count);

			while (dependencies.Count > 0)
			{
				var independentFragment = GetFirstFragmentWithoutPendingDependencies(dependencies);

				orderedFragments.Add(independentFragment);
				dependencies.Remove(independentFragment);

				foreach (var item in dependencies) // no deconstruct syntax for netstandard2.0
				{
					if (item.Value?.Remove(independentFragment) is true && item.Value.Count == 0)
						dependencies[item.Key].Clear();
				}
			}

			foreach (var frag in orderedFragments)
				await visitor.VisitAsync(frag, context);

			context.FragmentMapAlreadyBuilt = true;

			await visitor.VisitAsync(doc, context);

			return context.Result;
		}

        private static GraphQLFragmentDefinition GetFirstFragmentWithoutPendingDependencies(Dictionary<GraphQLFragmentDefinition, HashSet<GraphQLFragmentDefinition>> dependencies)
        {
            foreach (var item in dependencies)
            {
                if (item.Value.Count > 0)
                    return item.Key;
            }

            throw new InvalidOperationException("Fragments dependency cycle detected!");
        }

        private static Dictionary<GraphQLFragmentDefinition, HashSet<GraphQLFragmentDefinition>> BuildDependencies(GraphQLDocument document)
        {
            const int MAX_ITERATIONS = 2000;
            var selectionSetsToVisit = new Stack<GraphQLSelectionSet>();

            var dependencies = new Dictionary<GraphQLFragmentDefinition, HashSet<GraphQLFragmentDefinition>>();
            foreach (var fragmentDef in document.Definitions.OfType<GraphQLFragmentDefinition>())
            {
                selectionSetsToVisit.Push(fragmentDef.SelectionSet);
                dependencies[fragmentDef] = getDependencies(document, selectionSetsToVisit);
                selectionSetsToVisit.Clear();
            }

            return dependencies;

            static HashSet<GraphQLFragmentDefinition> getDependencies(GraphQLDocument document, Stack<GraphQLSelectionSet> selectionSetsToVisit)
            {
                var dependencies = new HashSet<GraphQLFragmentDefinition>(0);

                var counter = 0;
                while (selectionSetsToVisit.Count > 0)
                {
                    // https://github.com/graphql-dotnet/graphql-dotnet/issues/3527
                    if (++counter > MAX_ITERATIONS)
                        throw new ValidationError("It looks like document has fragment cycle. Please make sure you are using standard validation rules especially NoFragmentCycles one.");

                    foreach (var selection in selectionSetsToVisit.Pop().Selections)
                    {
                        if (selection is GraphQLFragmentSpread spread)
                        {
                            var fragment = document.FindFragmentDefinition(spread.FragmentName.Name.Value);
                            if (fragment is not null)
                            {
								dependencies.Add(fragment);
                                selectionSetsToVisit.Push(fragment.SelectionSet);
                            }
                        }
                        else if (selection is IHasSelectionSetNode hasSet && hasSet.SelectionSet is not null)
                        {
                            selectionSetsToVisit.Push(hasSet.SelectionSet);
                        }
                    }
                }

                return dependencies;
            }
        }
    }
}
