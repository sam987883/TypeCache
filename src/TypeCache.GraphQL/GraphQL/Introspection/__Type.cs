using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;

namespace GraphQL.Introspection;

/// <summary>
/// <see cref="__Type"/> is at the core of the type introspection system.
/// It represents scalars, interfaces, object types, unions, enums in the system.
/// </summary>
public class __Type : ObjectGraphType<IGraphType>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="__Type"/> introspection type.
	/// </summary>
	/// <param name="allowAppliedDirectives">Allows 'appliedDirectives' field for this type. It is an experimental feature.</param>
	public __Type(bool allowAppliedDirectives = false)
		: this(allowAppliedDirectives, false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="__Type"/> introspection type.
	/// </summary>
	/// <param name="allowAppliedDirectives">Allows 'appliedDirectives' field for this type. It is an experimental feature.</param>
	/// <param name="deprecationOfInputValues">
	/// Allows deprecation of input values - arguments on a field or input fields on an input type.
	/// This feature is from a working draft of the specification.
	/// </param>
	public __Type(bool allowAppliedDirectives = false, bool deprecationOfInputValues = false)
	{
		this.SetName(nameof(__Type), validate: false);

		this.Description = @"The fundamental unit of any GraphQL Schema is the type. There are
			many kinds of types in GraphQL as represented by the `__TypeKind` enum.

			Depending on the kind of a type, certain fields describe
			information about that type. Scalar types provide no information
			beyond a name and description, while Enum types provide their values.
			Object and Interface types provide the fields they describe. Abstract
			types, Union and Interface, provide the Object types possible
			at runtime. List and NonNull types compose other types.";

		this.Field<NonNullGraphType<GraphQLEnumType<TypeKind>>>("kind").Resolve(context =>
			context.Source is IGraphType type ? KindForInstance(type) : throw new InvalidOperationException($"Unknown kind of type: {context.Source}"));

		this.Field<GraphQLStringType>("name").Resolve(context => context.Source!.Name);

		this.Field<GraphQLStringType>("description").Resolve(context => context.Source!.Description);

		this.Field<ListGraphType<NonNullGraphType<__Field>>>("fields")
			.Argument<GraphQLBooleanType>("includeDeprecated", arg => arg.DefaultValue = false.Box())
			.Resolve(context =>
			{
				if (context.Source is not IObjectGraphType && context.Source is not IInterfaceGraphType)
					return null;

				var type = (IObjectGraphType)context.Source;
				var includeDeprecated = context.GetArgument<bool>("includeDeprecated");
				return type.Fields
					.Where(_ => (includeDeprecated || _.DeprecationReason.IsBlank()) && context.Schema.Filter.AllowField(type, _))
					.OrderBy(_ => _.Name)
					.ToArray();
			});

		this.Field<ListGraphType<NonNullGraphType<__Type>>>("interfaces")
			.Resolve(context =>
			{
				if (context.Source is not ObjectGraphType type)
					return null;

				return type.ResolvedInterfaces
					.Where(context.Schema.Filter.AllowType)
					.OrderBy(_ => _.Name)
					.ToArray();
			});

		this.Field<ListGraphType<NonNullGraphType<__Type>>>("possibleTypes")
			.Resolve(context =>
			{
				if (context.Source is not IAbstractGraphType type)
					return null;

				return type.PossibleTypes
					.Where(context.Schema.Filter.AllowType)
					.OrderBy(_ => _.Name)
					.ToArray();
			});

		this.Field<ListGraphType<NonNullGraphType<__EnumValue>>>("enumValues")
			.Argument<GraphQLBooleanType>("includeDeprecated", _ => _.DefaultValue = false.Box())
			.Resolve(context =>
			{
				if (context.Source is not GraphQLEnumType type)
					return null;

				var includeDeprecated = context.GetArgument<bool>("includeDeprecated");
				return type.Values
					.Select(_ => _.Value)
					.Where(_ => (includeDeprecated || _.DeprecationReason.IsBlank()) && context.Schema.Filter.AllowEnumValue(type, _))
					.OrderBy(_ => _.Name)
					.ToArray();
			});

		var inputFieldsField = Field<ListGraphType<NonNullGraphType<__InputValue>>>("inputFields")
			.Resolve(context =>
			{
				if (context.Source is not IInputObjectGraphType type)
					return null;

				var includeDeprecated = context.GetArgument<bool>("includeDeprecated", !deprecationOfInputValues);
				return type.Fields
					.Where(_ => (includeDeprecated || _.DeprecationReason.IsBlank()) && context.Schema.Filter.AllowField(type, _))
					.OrderBy(_ => _.Name)
					.ToArray();
			});

		if (deprecationOfInputValues)
			inputFieldsField.Argument<GraphQLBooleanType>("includeDeprecated", _ => _.DefaultValue = false.Box());

		this.Field<__Type>("ofType")
			.Resolve(context => context.Source switch
			{
				NonNullGraphType nonNull => nonNull.ResolvedType,
				ListGraphType list => list.ResolvedType,
				_ => null
			});

		if (allowAppliedDirectives)
			this.AddAppliedDirectivesField("type");
	}

	private static object KindForInstance(IGraphType type)
		=> type switch
		{
			GraphQLEnumType => TypeKind.ENUM.Box(),
			ScalarGraphType => TypeKind.SCALAR.Box(),
			IInterfaceGraphType => TypeKind.INTERFACE.Box(),
			IObjectGraphType => TypeKind.OBJECT.Box(),
			UnionGraphType => TypeKind.UNION.Box(),
			IInputObjectGraphType => TypeKind.INPUT_OBJECT.Box(),
			ListGraphType => TypeKind.LIST.Box(),
			NonNullGraphType => TypeKind.NON_NULL.Box(),
			_ => throw new ArgumentOutOfRangeException(nameof(type), $"Unknown kind of type: {type}")
		};
}
