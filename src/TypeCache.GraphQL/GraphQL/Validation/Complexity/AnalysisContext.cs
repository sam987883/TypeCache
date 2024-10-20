using System.Collections.Generic;
using System.Threading;
using GraphQLParser.AST;
using GraphQLParser.Visitors;

namespace GraphQL.Validation.Complexity
{
    internal sealed class AnalysisContext : IASTVisitorContext
    {
        public double AvgImpact { get; set; }

        public double? CurrentSubSelectionImpact { get; set; }

        public double CurrentEndNodeImpact { get; set; }

        public FragmentComplexity CurrentFragmentComplexity { get; set; } = null!;

        public ComplexityResult Result { get; } = new ComplexityResult();

        public int LoopCounter { get; set; }

        public int MaxRecursionCount { get; set; }

        public bool FragmentMapAlreadyBuilt { get; set; }

        public Dictionary<string, FragmentComplexity> FragmentMap { get; } = new Dictionary<string, FragmentComplexity>();

        public CancellationToken CancellationToken => default;

        public void AssertRecursion()
        {
            if (LoopCounter++ > MaxRecursionCount)
            {
                throw new InvalidOperationException("Query is too complex to validate.");
            }
        }

		/// <summary>
		/// TODO: variables support
		/// </summary>
		public static double? GetImpactFromArgs(GraphQLField node)
			=> node.Arguments switch
			{
				null => null,
				_ when node.Arguments.ValueFor("id") is not null => 1,
				_ when node.Arguments.ValueFor("first") is GraphQLIntValue value => int.Parse(value.Value.Span),
				_ when node.Arguments.ValueFor("last") is GraphQLIntValue value => int.Parse(value.Value.Span),
				_ => null
			};

        /// <summary>
        /// Takes into account the complexity of the specified node.
        /// <br/>
        /// Available nodes:
        /// <list type="number">
        /// <item><see cref="GraphQLField"/></item>
        /// <item><see cref="GraphQLFragmentSpread"/></item>
        /// </list>
        /// </summary>
        /// <param name="node">The node for which the complexity is added.</param>
        /// <param name="impact">Added complexity.</param>
        public void RecordFieldComplexity(ASTNode node, double impact)
        {
            Result.Complexity += impact;

            if (Result.ComplexityMap.ContainsKey(node))
                Result.ComplexityMap[node] += impact;
            else
                Result.ComplexityMap.Add(node, impact);
        }
    }
}
