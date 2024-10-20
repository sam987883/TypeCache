using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;
using TypeCache.Utilities;

namespace GraphQL.Introspection;

/// <summary>
/// The <see cref="__Field"/> introspection type represents each field in an Object or Interface type.
/// </summary>
public class __Field : ObjectGraphType<IFieldType>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="__Field"/> introspection type.
	/// </summary>
	/// <param name="allowAppliedDirectives">Allows 'appliedDirectives' field for this type. It is an experimental feature.</param>
	public __Field(bool allowAppliedDirectives = false)
		: this(allowAppliedDirectives, false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="__Field"/> introspection type.
	/// </summary>
	/// <param name="allowAppliedDirectives">Allows 'appliedDirectives' field for this type. It is an experimental feature.</param>
	/// <param name="deprecationOfInputValues">
	/// Allows deprecation of input values - arguments on a field or input fields on an input type.
	/// This feature is from a working draft of the specification.
	/// </param>
	public __Field(bool allowAppliedDirectives = false, bool deprecationOfInputValues = false)
	{
		this.SetName(nameof(__Field), validate: false);

		this.Description = "Object and Interface types are described by a list of Fields, each of which has a name, potentially a list of arguments, and a return type.";

		this.Field<NonNullGraphType<GraphQLStringType>>("name").Resolve(context => context.Source.Name);

		this.Field<GraphQLStringType>("description").Resolve(context => context.Source.Description);

		var argsField = this.Field<NonNullGraphType<ListGraphType<NonNullGraphType<__InputValue>>>>("args")
			.Resolve(context =>
			{
				if (!(context.Source.Arguments.Length > 0))
					return Array<QueryArgument>.Empty;

				var includeDeprecated = context.GetArgument<bool>("includeDeprecated");
				return context.Source.Arguments
					.Where(_ => (includeDeprecated || _.DeprecationReason.IsBlank()) && context.Schema.Filter.AllowArgument(context.Source, _))
					.OrderBy(_ => _.Name)
					.ToArray();
			});
		if (deprecationOfInputValues)
			argsField.Argument<GraphQLBooleanType>("includeDeprecated", _ => _.DefaultValue = false.Box());

		this.Field<NonNullGraphType<__Type>>("type").Resolve(context => context.Source.ResolvedType);

		this.Field<NonNullGraphType<GraphQLBooleanType>>("isDeprecated").Resolve(context => context.Source.DeprecationReason.IsNotBlank().Box());

		this.Field<GraphQLStringType>("deprecationReason").Resolve(context => context.Source.DeprecationReason);

		if (allowAppliedDirectives)
			this.AddAppliedDirectivesField("field");
	}
}
