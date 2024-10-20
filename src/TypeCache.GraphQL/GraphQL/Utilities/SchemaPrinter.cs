using System.Text;
using System.Text.RegularExpressions;
using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Types;
using TypeCache.Utilities;

// TODO:should be completely rewritten

namespace GraphQL.Utilities;

/// <summary>
/// Enables printing schema as SDL (Schema Definition Language) document.
/// <br/>
/// See <see href="https://spec.graphql.org/October2021/#sec-Type-System"/> for more information.
/// </summary>
// TODO: [Obsolete("Please use the schema.Print() extension method instead. This class will be removed in v9.")]
public class SchemaPrinter //TODO: rewrite string concatenations to use buffer ?
{
	private static readonly List<string> _builtInScalars =
	[
		"String",
		"Boolean",
		"Int",
		"Float",
		"ID"
	];

	private static readonly List<string> _builtInDirectives =
	[
		"skip",
		"include",
		"deprecated"
	];

	/// <summary>
	/// Creates printer with the specified options.
	/// </summary>
	/// <param name="schema">Schema to print.</param>
	/// <param name="options">Printer options.</param>
	public SchemaPrinter(ISchema schema, SchemaPrinterOptions? options = null)
	{
		Schema = schema;
		Options = options ?? new SchemaPrinterOptions();
	}

	protected static bool IsIntrospectionType(string typeName) => typeName.StartsWith("__", StringComparison.InvariantCulture);

	protected static bool IsBuiltInScalar(string typeName) => _builtInScalars.Contains(typeName, StringComparer.Ordinal);

	protected static bool IsBuiltInDirective(string directiveName) => _builtInDirectives.Contains(directiveName, StringComparer.Ordinal);

	protected ISchema Schema { get; set; }

	protected SchemaPrinterOptions Options { get; }

	/// <summary>
	/// Prints only 'defined' types and directives.
	/// <br/>
	/// See <see cref="IsDefinedType(string)"/> and <see cref="IsDefinedDirective(string)"/> for more information about what 'defined' means.
	/// </summary>
	/// <returns>SDL document.</returns>
	public string Print()
		=> this.PrintFilteredSchema(this.IsDefinedDirective, this.IsDefinedType);

	/// <summary>
	/// Prints only introspection types.
	/// </summary>
	/// <returns>SDL document.</returns>
	public string PrintIntrospectionSchema()
		=> this.PrintFilteredSchema(IsBuiltInDirective, IsIntrospectionType);

	/// <summary>
	/// Prints schema according to the specified filters.
	/// </summary>
	/// <param name="directiveFilter">Filter for directives.</param>
	/// <param name="typeFilter">Filter for types.</param>
	/// <returns>SDL document.</returns>
	public string PrintFilteredSchema(Func<string, bool> directiveFilter, Func<string, bool> typeFilter)
	{
		if (this.Schema is null)
			return string.Empty;

		this.Schema.Initialize();

		var directives = Schema.Directives
			.Where(_ => directiveFilter(_.Name))
			.OrderBy(_ => _.Name, StringComparer.Ordinal);
		var types = Schema.AllTypes.Dictionary.Values
			.Where(_ => typeFilter(_.Name))
			.OrderBy(_ => _.Name, StringComparer.Ordinal);

		var result = new[] { PrintSchemaDefinition(Schema) }
			.Concat(directives.Select(PrintDirective))
			.Concat(types.Select(PrintType))
			.WhereNotNull()
			.ToArray();

		return string.Join(Environment.NewLine + Environment.NewLine, result) + Environment.NewLine;
	}

	/// <summary>
	/// Determines that the specified directive is defined in the schema and should be printed.
	/// By default, all directives are defined (printed) except for built-in directives.
	/// </summary>
	protected virtual bool IsDefinedDirective(string directiveName)
		=> !IsBuiltInDirective(directiveName);

	/// <summary>
	/// Determines that the specified type is defined in the schema and should be printed.
	/// By default, all types are defined (printed) except for introspection types and built-in scalars.
	/// </summary>
	protected virtual bool IsDefinedType(string typeName)
		=> !IsIntrospectionType(typeName) && !IsBuiltInScalar(typeName);

	public string? PrintSchemaDefinition(ISchema schema)
	{
		schema?.Initialize();

		if (schema is null || IsSchemaOfCommonNames(schema))
			return null;

		var operationTypes = new List<string>();

		if (schema.Query is not null)
			operationTypes.Add($"  query: {schema.Query}");

		if (schema.Mutation is not null)
			operationTypes.Add($"  mutation: {schema.Mutation}");

		if (schema.Subscription is not null)
			operationTypes.Add($"  subscription: {schema.Subscription}");

		return $"schema {{{Environment.NewLine}{string.Join(Environment.NewLine, operationTypes)}{Environment.NewLine}}}";
	}

	/**
	 * GraphQL schema define root types for each type of operation. These types are
	 * the same as any other type and can be named in any manner, however there is
	 * a common naming convention:
	 *
	 *   schema {
	 *     query: Query
	 *     mutation: Mutation
	 *     subscription: Subscription
	 *   }
	 *
	 * When using this naming convention, the schema description can be omitted.
	 */
	public bool IsSchemaOfCommonNames(ISchema schema)
	{
		Schema?.Initialize();

		if (schema.Query is not null && schema.Query.Name != "Query")
			return false;

		if (schema.Mutation is not null && schema.Mutation.Name != "Mutation")
			return false;

		if (schema.Subscription is not null && schema.Subscription.Name != "Subscription")
			return false;

		return true;
	}

	public string PrintType(IGraphType type)
	{
		Schema?.Initialize();

		return type switch
		{
			GraphQLEnumType graphType => PrintEnum(graphType),
			ScalarGraphType scalarGraphType => PrintScalar(scalarGraphType),
			IInterfaceGraphType interfaceGraphType => PrintInterface(interfaceGraphType),
			IObjectGraphType objectGraphType => PrintObject(objectGraphType),
			UnionGraphType unionGraphType => PrintUnion(unionGraphType),
			Directive directiveGraphType => PrintDirective(directiveGraphType),  //TODO: DirectiveGraphType does not inherit IGraphType
			IInputObjectGraphType input => PrintInputObject(input),
			_ => throw new InvalidOperationException($"Unknown GraphType '{type.GetType().Name}' with name '{type.Name}'")
		};
	}

	public virtual string PrintScalar(ScalarGraphType type)
	{
		Schema?.Initialize();

		var description = FormatDescription(type.Description);
		return $"{description}scalar {type.Name}";
	}

	public virtual string PrintObject(IObjectGraphType type)
	{
		Schema?.Initialize();

		var interfaces = type is ObjectGraphType graphType ? graphType.ResolvedInterfaces.OrderBy(_ => _.Name).Select(_ => _.Name).ToArray() : Array<string>.Empty;
		var delimiter = Options.OldImplementsSyntax ? ", " : " & ";
		var implementedInterfaces = interfaces.Length > 0 ? $" implements {string.Join(delimiter, interfaces)}" : string.Empty;

		var description = FormatDescription(type.Description);
		if (type.Fields.Count > 0)
			return $"{description}type {type.Name}{implementedInterfaces} {{{{{Environment.NewLine}{PrintFields(type)}{Environment.NewLine}}}}}";
		else
			return $"{description}type {type.Name}{implementedInterfaces}";
	}

	public virtual string PrintInterface(IInterfaceGraphType type)
	{
		Schema?.Initialize();

		var description = FormatDescription(type.Description);
		return $"{description}interface {type.Name} {{{{{Environment.NewLine}{PrintFields(type)}{Environment.NewLine}}}}}";
	}

	public virtual string PrintUnion(UnionGraphType type)
	{
		Schema?.Initialize();

		var possibleTypes = string.Join(" | ", type.PossibleTypes.OrderBy(_ => _.Name).Select(_ => _.Name));
		var description = FormatDescription(type.Description);
		return $"{description}union {type.Name} = {possibleTypes}";
	}

	public virtual string PrintEnum(GraphQLEnumType type)
	{
		Schema?.Initialize();

		var values = string.Join(Environment.NewLine,
			type.Values
				.OrderBy(_ => _.Key)
				.Select(_ => $"{FormatDescription(_.Value.Description, "  ")}  {_.Value.Name}{(Options.IncludeDeprecationReasons ? PrintDeprecation(_.Value.DeprecationReason) : string.Empty)}"));
		var description = FormatDescription(type.Description);
		return $"{description}enum {type.Name} {{{{{Environment.NewLine}{values}{Environment.NewLine}}}}}";
	}

	public virtual string PrintInputObject(IInputObjectGraphType type)
	{
		Schema?.Initialize();

		var fields = type.Fields
			.OrderBy(_ => _.Name)
			.Select(PrintInputValue);
		var description = FormatDescription(type.Description);
		return $"{description}input {type.Name} {{{{{Environment.NewLine}{string.Join(Environment.NewLine, fields)}{Environment.NewLine}}}}}";
	}

	public virtual string PrintFields(IObjectGraphType type)
	{
		Schema?.Initialize();

		var fields = type?.Fields
			.OrderBy(_ => _.Name)
			.Select(_ => new
			{
				_.Name,
				Type = _.ResolvedType,
				Args = PrintArgs(_),
				Description = FormatDescription(_.Description, "  "),
				Deprecation = Options.IncludeDeprecationReasons ? PrintDeprecation(_.DeprecationReason) : string.Empty,
			}).ToList();

		return fields is null
			? string.Empty
			: string.Join(Environment.NewLine, fields.Select(_ => $"{_.Description}  {_.Name}{_.Args}: {_.Type}{_.Deprecation}"));
	}

	public virtual string PrintFields(IInterfaceGraphType type)
	{
		Schema?.Initialize();

		var fields = type?.Fields
			.OrderBy(_ => _.Name)
			.Select(_ => new
			{
				_.Name,
				Type = _.ResolvedType,
				Args = PrintArgs(_),
				Description = FormatDescription(_.Description, "  "),
				Deprecation = Options.IncludeDeprecationReasons ? PrintDeprecation(_.DeprecationReason) : string.Empty,
			}).ToList();

		return fields is null
			? string.Empty
			: string.Join(Environment.NewLine, fields.Select(_ => $"{_.Description}  {_.Name}{_.Args}: {_.Type}{_.Deprecation}"));
	}

	public virtual string PrintArgs(FieldType field)
	{
		this.Schema?.Initialize();

		if (field.Arguments.Length == 0)
			return string.Empty;

		return $"({string.Join(", ", field.Arguments.OrderBy(_ => _.Name).Select(PrintInputValue))})"; // TODO: iterator allocation
	}

	public string PrintInputValue(FieldType field)
	{
		this.Schema?.Initialize();

		return field.DefaultValue is not null
			? $"{FormatDescription(field.Description, "  ")}  {field.Name}: {field.ResolvedType} = {field.ResolvedType!.ToString(field.DefaultValue)}"
			: $"{FormatDescription(field.Description, "  ")}  {field.Name}: {field.ResolvedType}";
	}

	public string PrintInputValue(QueryArgument argument)
	{
		this.Schema?.Initialize();

		return argument.DefaultValue is not null
			? $"{argument.Name}: {argument.ResolvedType}"
			: $"{argument.Name}: {argument.ResolvedType} = {argument.ResolvedType!.ToString(argument.DefaultValue)}";
	}

	public string PrintDirective(Directive directive)
	{
		this.Schema?.Initialize();

		var directiveBuilder = new StringBuilder();
		directiveBuilder.Append(FormatDescription(directive.Description));
		directiveBuilder.Append("directive @").Append(directive.Name);

		if (directive.Arguments.Length > 0)
		{
			directiveBuilder
				.Append('(')
				.AppendLine()
				.AppendJoin($"{Environment.NewLine}  ", directive.Arguments.OrderBy(_ => _.Name).Select(PrintInputValue))
				.AppendLine()
				.Append(')');
		}

		if (directive.Repeatable)
			directiveBuilder.Append(" repeatable");

		directiveBuilder.Append(" on ").Append(FormatDirectiveLocationList(directive.Locations));
		return directiveBuilder.ToString().TrimStart();
	}

	private string FormatDirectiveLocationList(IEnumerable<DirectiveLocation> locations)
		=> string.Join(" | ", locations.OrderBy(_ => _.Name()).Select(_ => Singleton<GraphQLEnumType<DirectiveLocation>>.Instance.Serialize(_))); //TODO: remove allocations

	protected string FormatDescription(string? description, string indentation = "")
		=> this.Options.IncludeDescriptions
			? (this.Options.PrintDescriptionsAsComments ? PrintComment(description, indentation) : PrintDescription(description, indentation))
			: string.Empty;

	public virtual string PrintComment(string? comment, string indentation = "", bool firstInBlock = true)
	{
		if (comment.IsBlank())
			return string.Empty;

		indentation ??= string.Empty;

		// normalize newlines
		comment = comment!.Replace("\r", string.Empty);

		var descriptionBuilder = new StringBuilder();
		if (indentation.IsNotBlank() && !firstInBlock)
			descriptionBuilder.AppendLine();

		foreach (var line in comment.Split('\n'))
		{
			if (line == string.Empty)
				descriptionBuilder.Append(indentation).Append('#').AppendLine();
			else
			{
				// For > 120 character long lines, cut at space boundaries into sublines
				// of ~80 chars.
				var subLines = BreakLine(line, 120 - indentation.Length);
				foreach (string subLine in subLines)
					descriptionBuilder.Append(indentation).Append("# ").AppendLine(subLine);
			}
		}

		return descriptionBuilder.ToString();
	}

	public string PrintDescription(string? description, string indentation = "", bool firstInBlock = true)
	{
		if (description.IsBlank())
			return string.Empty;

		indentation ??= string.Empty;

		// escape """ with \"""
		description = description!.Replace("\"\"\"", "\\\"\"\"");

		// normalize newlines
		description = description.Replace("\r", "");

		// remove control characters besides newline and tab
		if (description.Any(c => c < ' ' && c != '\t' & c != '\n'))
			description = new string(description.Where(c => c >= ' ' || c == '\t' || c == '\n').ToArray());

		var descriptionBuilder = new StringBuilder();
		if (indentation.IsNotBlank() && !firstInBlock)
			descriptionBuilder.AppendLine();

		descriptionBuilder.Append(indentation).AppendLine("\"\"\"");

		foreach (var line in description.Split('\n'))
		{
			if (line.IsBlank())
				descriptionBuilder.AppendLine();
			else
				descriptionBuilder.Append(indentation).AppendLine(line);
		}

		descriptionBuilder.Append(indentation).AppendLine("\"\"\"");
		return descriptionBuilder.ToString();
	}

	public string PrintDeprecation(string? reason)
		=> reason.IsNotBlank() ? $" @deprecated(reason: {new GraphQLStringValue(reason!).Print()})" : string.Empty;

	public string[] BreakLine(string line, int len)
	{
		if (line.Length < len + 5)
			return [line];

		var parts = Regex.Split(line, $"((?: |^).{{15,{len - 40}}}(?= |$))");
		if (parts.Length < 4)
			return [line];

		var sublines = new List<string>()
		{
			$"{parts[0]}{parts[1]}{parts[2]}"
		};

		for (var i = 3; i < parts.Length; i += 2)
			sublines.Add(parts[i].Substring(1) + parts[i + 1]);

		return sublines.ToArray();
	}
}
