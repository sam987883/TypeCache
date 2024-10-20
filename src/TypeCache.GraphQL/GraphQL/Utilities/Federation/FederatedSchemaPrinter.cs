using System;
using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Types;
using TypeCache.Utilities;

namespace GraphQL.Utilities.Federation;

// TODO: [Obsolete("Please use the schema.Print() extension method instead. This class will be removed in v9.")]
public class FederatedSchemaPrinter : SchemaPrinter // TODO: should be completely rewritten
{
	const string AST_METAFIELD = "__AST_MetaField__";
	const string EXTENSION_AST_METAFIELD = "__EXTENSION_AST_MetaField__";

	private static readonly GraphQLName[] _federatedDirectives =
	[
		new GraphQLName("external"),
		new GraphQLName("provides"),
		new GraphQLName("requires"),
		new GraphQLName("key"),
		new GraphQLName("extends")
	];

	private static readonly GraphQLName[] _federatedTypes =
	[
		new GraphQLName("_Service"),
		new GraphQLName("_Entity"),
		new GraphQLName("_Any")
	];

	public FederatedSchemaPrinter(ISchema schema, SchemaPrinterOptions? options = null)
		: base(schema, options)
	{
	}

	public string PrintFederatedDirectives(IGraphType type)
	{
		this.Schema?.Initialize();

		return type.IsInputObjectType() ? string.Empty : PrintFederatedDirectivesFromAst(type);
	}

	public string PrintFederatedDirectivesFromAst(IProvideMetadata type)
	{
		this.Schema?.Initialize();

		var astDirectives = type.GetAstType<IHasDirectivesNode>()?.Directives ?? type.GetExtensionDirectives<GraphQLDirective>();
		if (astDirectives is null)
			return string.Empty;

		var dirs = string.Join(' ', astDirectives
			.Where(_ => IsFederatedDirective(_.Name))
			.Select(PrintAstDirective)
		);

		return string.IsNullOrWhiteSpace(dirs) ? string.Empty : $" {dirs}";
	}

	public string PrintAstDirective(GraphQLDirective directive)
	{
		this.Schema?.Initialize();

		return directive.Print();
	}

	public override string PrintObject(IObjectGraphType type)
	{
		this.Schema?.Initialize();

		var interfaces = type is ObjectGraphType graphType ? graphType!.ResolvedInterfaces.Select(_ => _.Name).ToArray() : [];
		var implementedInterfaces = interfaces.Any() ? $" implements {string.Join(" & ", interfaces)}" : string.Empty;

		var federatedDirectives = PrintFederatedDirectives(type);
		var description = FormatDescription(type.Description);

		var isExtension = type!.IsExtensionType();
		var text = $"{description}{(isExtension ? "extend " : string.Empty)}type {type.Name}{implementedInterfaces}{federatedDirectives}";

		if (type.Fields.Any(x => !IsFederatedType(new GraphQLName(x.ResolvedType!.GetNamedGraphType().Name))))
			return $"{text} {{{Environment.NewLine}{PrintFields(type)}{Environment.NewLine}}}";

		return text;
	}

	public override string PrintInterface(IInterfaceGraphType type)
	{
		this.Schema?.Initialize();

		var isExtension = type!.IsExtensionType();

		var description = FormatDescription(type.Description);
		return $"{description}{(isExtension ? "extend " : string.Empty)}interface {type.Name} {{{Environment.NewLine}{PrintFields(type)}{Environment.NewLine}}}";
	}

	public override string PrintFields(IObjectGraphType type)
	{
		this.Schema?.Initialize();

		if (type is null)
			return string.Empty;

		var fields = type.Fields
			.Where(_ => !IsFederatedType(new GraphQLName(_.ResolvedType!.GetNamedGraphType().Name)))
			.Select(_ => new
			{
				_.Name,
				Type = _.ResolvedType,
				Args = PrintArgs(_),
				Description = FormatDescription(_.Description, "  "),
				Deprecation = Options.IncludeDeprecationReasons ? PrintDeprecation(_.DeprecationReason) : string.Empty,
				FederatedDirectives = PrintFederatedDirectivesFromAst(_)
			}).ToArray();

		return string.Join(Environment.NewLine, fields.Select(_ =>
			$"{_.Description}  {_.Name}{_.Args}: {_.Type}{_.Deprecation}{_.FederatedDirectives}"));
	}

	public string PrintFederatedSchema()
	{
		this.Schema?.Initialize();

		return PrintFilteredSchema(
			directiveName => !IsBuiltInDirective(directiveName) && !IsFederatedDirective(new GraphQLName(directiveName)),
			typeName => !IsFederatedType(new GraphQLName(typeName)) && IsDefinedType(typeName));
	}

	public bool IsFederatedDirective(GraphQLName name)
		=> _federatedDirectives.Contains(name);

	public bool IsFederatedType(GraphQLName name)
		=> _federatedTypes.Contains(name);
}
