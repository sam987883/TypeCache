using GraphQL.Types;

namespace GraphQL.Introspection;

/// <summary>
/// Schema filter that enables some experimental features that are not in the
/// official specification, i.e. ability to expose user-defined meta-information
/// via introspection. See https://github.com/graphql/graphql-spec/issues/300
/// for more information.
/// </summary>
public class ExperimentalIntrospectionFeaturesSchemaFilter : DefaultSchemaFilter
{
	/// <inheritdoc/>
	public override bool AllowType(IGraphType type) => true;

	/// <inheritdoc/>
	public override bool AllowField(IGraphType parent, IFieldType field) => true;
}
