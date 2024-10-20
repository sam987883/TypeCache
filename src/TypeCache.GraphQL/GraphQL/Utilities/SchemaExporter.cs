using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Types;

namespace GraphQL.Utilities;

/// <summary>
/// Exports a schema definition to a SDL.
/// </summary>
public class SchemaExporter
{
	/// <summary>
	/// Returns the <see cref="ISchema"/> instance this exporter is exporting.
	/// </summary>
	protected ISchema Schema { get; }

	private static readonly string[] _builtInScalars =
	[
		"String",
		"Boolean",
		"Int",
		"Float",
		"ID",
	];

	private static readonly string[] _builtInDirectives =
	[
		"skip",
		"include",
		"deprecated",
	];

	/// <summary>
	/// Initializes a new instance of the <see cref="SchemaExporter"/> class for the specified <see cref="ISchema"/>.
	/// </summary>
	public SchemaExporter(ISchema schema)
	{
		this.Schema = schema;
	}

	/// <summary>
	/// Exports the schema as a <see cref="GraphQLDocument"/>.
	/// </summary>
	public virtual GraphQLDocument Export()
	{
		// initialize the schema, so all the ResolvedType properties are set
		this.Schema.Initialize();

		// export the schema definition
		var definitions = new List<ASTNode>();

		var schemaDefinition = ApplyExtend(ExportSchemaDefinition(), this.Schema);
		if (schemaDefinition is GraphQLSchemaDefinition schemaDef && !IsDefaultSchemaConfiguration(schemaDef))
			definitions.Add(schemaDef);

		// export directives
		foreach (var directive in this.Schema.Directives)
		{
			if (!IsBuiltInDirective(directive.Name))
				definitions.Add(ExportDirectiveDefinition(directive));
		}

		// export types
		foreach (var type in this.Schema.AllTypes)
		{
			if (!IsIntrospectionType(type.Name) && !IsBuiltInScalar(type.Name))
				definitions.Add(ApplyExtend(ExportTypeDefinition(type), type));
		}

		return new GraphQLDocument(definitions);
	}

	/// <summary>
	/// Returns <see langword="true"/> if the schema definition uses the default type names of
	/// <c>Query</c>, <c>Mutation</c> and <c>Subscription</c>, and has no directives or description.
	/// </summary>
	protected virtual bool IsDefaultSchemaConfiguration(GraphQLSchemaDefinition schemaDefinition)
	{
		// if any directives are specified on the schema definition, or if the description
		// is set, return false
		if (schemaDefinition.Directives?.Count > 0 || schemaDefinition.Description is not null)
			return false;

		// if any of the operation types are the not the default type names, return false
		if (!schemaDefinition.OperationTypes.All(_ => _.Type?.Name.Value == _.Operation switch
		{
			OperationType.Query => "Query",
			OperationType.Mutation => "Mutation",
			OperationType.Subscription => "Subscription",
			_ => throw new UnreachableException("Could not identify the type of operation.")
		}))
			return false;

		var hasQueryOperation = schemaDefinition.OperationTypes.Any(_ => _.Operation is OperationType.Query);
		var hasMutationOperation = schemaDefinition.OperationTypes.Any(_ => _.Operation is OperationType.Mutation);
		var hasSubscriptionOperation = schemaDefinition.OperationTypes.Any(_ => _.Operation is OperationType.Subscription);

		var schemaHasQueryType = Schema.AllTypes["Query"] is not null;
		var schemaHasMutationType = Schema.AllTypes["Mutation"] is not null;
		var schemaHasSubscriptionType = Schema.AllTypes["Subscription"] is not null;

		// Ensure that the schema has the same operation types as the schema definition.
		return schemaHasQueryType == hasQueryOperation
			&& schemaHasMutationType == hasMutationOperation
			&& schemaHasSubscriptionType == hasSubscriptionOperation;
	}

	/// <summary>
	/// Returns <see langword="true"/> if the specified type name is a built-in introspection type.
	/// </summary>
	protected static bool IsIntrospectionType(string typeName)
		=> typeName.StartsWith("__", StringComparison.Ordinal);

	/// <summary>
	/// Returns <see langword="true"/> if the specified type name is a built-in scalar type.
	/// </summary>
	protected static bool IsBuiltInScalar(string typeName)
		=> _builtInScalars.Contains(typeName);

	/// <summary>
	/// Returns <see langword="true"/> if the specified directive name is a built-in directive.
	/// </summary>
	protected static bool IsBuiltInDirective(string directiveName)
		=> _builtInDirectives.Contains(directiveName);

	/// <summary>
	/// Exports the specified <see cref="IGraphType"/> as a <see cref="GraphQLTypeDefinition"/>.
	/// </summary>
	protected virtual GraphQLTypeDefinition ExportTypeDefinition(IGraphType graphType)
		=> graphType switch
		{
			GraphQLEnumType enumType => this.ExportEnumTypeDefinition(enumType),
			ScalarGraphType scalarType => this.ExportScalarTypeDefinition(scalarType),
			IInterfaceGraphType interfaceType => this.ExportInterfaceTypeDefinition(interfaceType),
			IInputObjectGraphType inputType => this.ExportInputObjectTypeDefinition(inputType),
			UnionGraphType unionType => this.ExportUnionTypeDefinition(unionType),
			ObjectGraphType objectType => this.ExportObjectTypeDefinition(objectType),
			_ => throw new ArgumentOutOfRangeException(nameof(graphType), "Could not identify the type of graph type supplied.")
		};

	/// <summary>
	/// Exports the specified <see cref="IInputObjectGraphType"/> as a <see cref="GraphQLInputObjectTypeDefinition"/>.
	/// </summary>
	protected virtual GraphQLInputObjectTypeDefinition ExportInputObjectTypeDefinition(IInputObjectGraphType graphType)
	{
		var typedef = new GraphQLInputObjectTypeDefinition(new(graphType.Name));
		if (graphType.Fields.Count > 0)
		{
			var list = new List<GraphQLInputValueDefinition>(graphType.Fields.Count);
			foreach (var field in graphType.Fields)
			{
				list.Add(this.ExportInputValueDefinition(field));
			}
			typedef.Fields = new(list);
		}

		return this.ApplyDescription(this.ApplyDirectives(typedef, graphType), graphType);
	}

	/// <summary>
	/// Exports the specified <see cref="FieldType"/> as a <see cref="GraphQLInputValueDefinition"/>.
	/// </summary>
	protected virtual GraphQLInputValueDefinition ExportInputValueDefinition(FieldType fieldType)
	{
		var ret = new GraphQLInputValueDefinition(new(fieldType.Name), this.ExportTypeReference(fieldType.ResolvedType!))
		{
			DefaultValue = fieldType.DefaultValue is not null ? fieldType.ResolvedType!.ToAST(fieldType.DefaultValue) : null
		};
		return this.ApplyDescription(this.ApplyDirectives(ret, fieldType), fieldType);
	}

	/// <summary>
	/// Exports the specified <see cref="IInterfaceGraphType"/> as a <see cref="GraphQLInterfaceTypeDefinition"/>.
	/// </summary>
	protected virtual GraphQLInterfaceTypeDefinition ExportInterfaceTypeDefinition(IInterfaceGraphType graphType)
	{
		// Note: Interface inheritance is not yet supported by GraphQL.NET.
		var typedef = new GraphQLInterfaceTypeDefinition(new(graphType.Name));
		if (graphType.Fields.Count > 0)
		{
			var list = new List<GraphQLFieldDefinition>(graphType.Fields.Count);
			foreach (var field in graphType.Fields)
			{
				list.Add(ExportFieldDefinition(field));
			}
			typedef.Fields = new(list);
		}

		return this.ApplyDescription(this.ApplyDirectives(typedef, graphType), graphType);
	}

	/// <summary>
	/// Exports the specified <see cref="IObjectGraphType"/> as a <see cref="GraphQLObjectTypeDefinition"/>.
	/// </summary>
	protected virtual GraphQLObjectTypeDefinition ExportObjectTypeDefinition(ObjectGraphType graphType)
	{
		var typedef = new GraphQLObjectTypeDefinition(new(graphType.Name));
		if (graphType.ResolvedInterfaces.Count > 0)
		{
			var list = new List<GraphQLNamedType>(graphType.ResolvedInterfaces.Count);
			foreach (var interfaceType in graphType.ResolvedInterfaces)
			{
				list.Add(new(new(interfaceType.Name)));
			}
			typedef.Interfaces = new(list);
		}

		if (graphType.Fields.Count > 0)
		{
			var list = new List<GraphQLFieldDefinition>(graphType.Fields.Count);
			foreach (var field in graphType.Fields)
			{
				list.Add(ExportFieldDefinition(field));
			}
			typedef.Fields = new(list);
		}

		return this.ApplyDescription(this.ApplyDirectives(typedef, graphType), graphType);
	}

	/// <summary>
	/// Converts the specified type definition to an extension definition
	/// if it was defined as such by the <see cref="SchemaBuilder"/>.
	/// </summary>
	protected virtual ASTNode ApplyExtend(ASTNode node, IProvideMetadata graphType)
	{
		if (graphType.HasExtensionAstTypes() && graphType.GetAstType<ASTNode>() is null)
		{
			return node switch
			{
				GraphQLObjectTypeDefinition otd => new GraphQLObjectTypeExtension(otd.Name)
				{
					Interfaces = otd.Interfaces,
					Fields = otd.Fields,
					Directives = otd.Directives,
					Comments = otd.Comments,
				},
				GraphQLInterfaceTypeDefinition itd => new GraphQLInterfaceTypeExtension(itd.Name)
				{
					Interfaces = itd.Interfaces,
					Fields = itd.Fields,
					Directives = itd.Directives,
					Comments = itd.Comments,
				},
				GraphQLUnionTypeDefinition utd => new GraphQLUnionTypeExtension(utd.Name)
				{
					Types = utd.Types,
					Directives = utd.Directives,
					Comments = utd.Comments,
				},
				GraphQLScalarTypeDefinition std => new GraphQLScalarTypeExtension(std.Name)
				{
					Directives = std.Directives,
					Comments = std.Comments,
				},
				GraphQLEnumTypeDefinition etd => new GraphQLEnumTypeExtension(etd.Name)
				{
					Values = etd.Values,
					Directives = etd.Directives,
					Comments = etd.Comments,
				},
				GraphQLInputObjectTypeDefinition iotd => new GraphQLInputObjectTypeExtension(iotd.Name)
				{
					Fields = iotd.Fields,
					Directives = iotd.Directives,
					Comments = iotd.Comments,
				},
				GraphQLSchemaDefinition sd => new GraphQLSchemaExtension
				{
					OperationTypes = sd.OperationTypes,
					Directives = sd.Directives,
					Comments = sd.Comments,
				},
				_ => throw new InvalidOperationException($"Invalid node type of '{node.GetType().GetTypeName()}'.")
			};
		}

		return node;
	}

	/// <summary>
	/// Exports the specified <see cref="FieldType"/> as a <see cref="GraphQLFieldDefinition"/>.
	/// </summary>
	protected virtual GraphQLFieldDefinition ExportFieldDefinition(FieldType fieldType)
	{
		var fieldDef = new GraphQLFieldDefinition(new(fieldType.Name), this.ExportTypeReference(fieldType.ResolvedType!))
		{
			Arguments = this.ExportArgumentsDefinition(fieldType.Arguments),
		};
		return this.ApplyDescription(this.ApplyDirectives(fieldDef, fieldType), fieldType);
	}

	/// <summary>
	/// Exports a <see cref="UnionGraphType"/> as a <see cref="GraphQLUnionTypeDefinition"/>.
	/// </summary>
	protected virtual GraphQLUnionTypeDefinition ExportUnionTypeDefinition(UnionGraphType graphType)
	{
		var unionDef = new GraphQLUnionTypeDefinition(new(graphType.Name));
		if (graphType.PossibleTypes.Count > 0)
		{
			var types = new List<GraphQLNamedType>(graphType.PossibleTypes.Count);
			types.AddRange(graphType.PossibleTypes.Select(_ => new GraphQLNamedType(new(_.Name))));
			unionDef.Types = new(types);
		}

		return this.ApplyDescription(this.ApplyDirectives(unionDef, graphType), graphType);
	}

	/// <summary>
	/// Exports the specified <see cref="ScalarGraphType"/> as a <see cref="GraphQLScalarTypeDefinition"/>.
	/// </summary>
	protected virtual GraphQLScalarTypeDefinition ExportScalarTypeDefinition(ScalarGraphType scalarType)
		=> this.ApplyDescription(this.ApplyDirectives(new GraphQLScalarTypeDefinition(new(scalarType.Name)), scalarType), scalarType);

	/// <summary>
	/// Exports the specified <see cref="GraphQLEnumType"/> as a <see cref="GraphQLEnumTypeDefinition"/>.
	/// </summary>
	protected virtual GraphQLEnumTypeDefinition ExportEnumTypeDefinition(GraphQLEnumType enumType)
	{
		var enumDef = new GraphQLEnumTypeDefinition(new(enumType.Name));
		if (enumType.Values.Count > 0)
		{
			var values = new List<GraphQLEnumValueDefinition>(enumType.Values.Count);
			foreach (var pair in enumType.Values)
			{
				var name = new GraphQLName(pair.Value.Name);
				var def = new GraphQLEnumValueDefinition(name, new GraphQLEnumValue(name));
				values.Add(this.ApplyDescription(this.ApplyDirectives(def, pair.Value), pair.Value));
			}
			enumDef.Values = new GraphQLEnumValuesDefinition(values);
		}

		return this.ApplyDescription(this.ApplyDirectives(enumDef, enumType), enumType);
	}

	/// <summary>
	/// Exports the schema definition as a <see cref="GraphQLSchemaDefinition"/>.
	/// </summary>
	protected virtual GraphQLSchemaDefinition ExportSchemaDefinition()
	{
		var definitions = new List<GraphQLRootOperationTypeDefinition>(3)
		{
			new()
			{
				Operation = OperationType.Query,
				Type = new GraphQLNamedType(new(Schema.Query.Name))
			}
		};

		if (this.Schema.Mutation is not null)
		{
			definitions.Add(new()
			{
				Operation = OperationType.Mutation,
				Type = new GraphQLNamedType(new(Schema.Mutation.Name))
			});
		}

		if (this.Schema.Subscription is not null)
		{
			definitions.Add(new()
			{
				Operation = OperationType.Subscription,
				Type = new GraphQLNamedType(new(Schema.Subscription.Name))
			});
		}

		return this.ApplyDescription(this.ApplyDirectives(new GraphQLSchemaDefinition(definitions), Schema), Schema);
	}

	/// <summary>
	/// Exports the specified <see cref="Directive"/> as a <see cref="GraphQLDirectiveDefinition"/>.
	/// </summary>
	protected virtual GraphQLDirectiveDefinition ExportDirectiveDefinition(Directive directive)
	{
		var def = new GraphQLDirectiveDefinition(new(directive.Name), new GraphQLDirectiveLocations(directive.Locations))
		{
			Repeatable = directive.Repeatable,
			Arguments = this.ExportArgumentsDefinition(directive.Arguments!.ToArray()),
		};
		return this.ApplyDescription(def, directive);
	}

	private GraphQLArgumentsDefinition? ExportArgumentsDefinition(QueryArgument[] arguments)
	{
		if (arguments.Length == 0)
			return null;

		var args = new List<GraphQLInputValueDefinition>(arguments.Length);
		foreach (var arg in arguments)
		{
			args.Add(this.ExportArgumentDefinition(arg));
		}

		return new(args);
	}

	/// <summary>
	/// Exports the specified <see cref="QueryArgument"/> as a <see cref="GraphQLInputValueDefinition"/>.
	/// </summary>
	protected virtual GraphQLInputValueDefinition ExportArgumentDefinition(QueryArgument argument)
	{
		var defaultValue = argument.DefaultValue is not null
			? argument.ResolvedType!.ToAST(argument.DefaultValue)
			: null;

		var inputdef = new GraphQLInputValueDefinition(new(argument.Name), this.ExportTypeReference(argument.ResolvedType!))
		{
			DefaultValue = defaultValue,
		};
		return this.ApplyDescription(this.ApplyDirectives(inputdef, argument), argument);
	}

	/// <summary>
	/// Exports the specified <see cref="IGraphType"/> as a <see cref="GraphQLType"/>.
	/// This will return a <see cref="GraphQLNamedType"/>, wrapped if necessary within
	/// <see cref="GraphQLNonNullType"/> or <see cref="GraphQLListType"/> instances.
	/// </summary>
	protected virtual GraphQLType ExportTypeReference(IGraphType graphType)
		=> graphType switch
		{
			NonNullGraphType nonNullGraphType => new GraphQLNonNullType(this.ExportTypeReference(nonNullGraphType.ResolvedType!)),
			ListGraphType listGraphType => new GraphQLListType(this.ExportTypeReference(listGraphType.ResolvedType!)),
			_ => new GraphQLNamedType(new(graphType.Name))
		};

	/// <summary>
	/// Adds a description to an AST node if the schema object has a description.
	/// </summary>
	protected virtual T ApplyDescription<T>(T node, IProvideDescription obj)
		where T : IHasDescriptionNode
	{
		if (obj.Description.IsNotBlank())
			node.Description = new GraphQLDescription(obj.Description);

		return node;
	}

	/// <summary>
	/// Adds any applied directives from a schema object to an AST node.
	/// If the schema object implements <see cref="IProvideDeprecationReason"/>
	/// and has a deprecation reason set, and if the @deprecated directive is not
	/// already set on the schema object, then the deprecation reason is added
	/// as a directive also.
	/// </summary>
	protected virtual T ApplyDirectives<T>(T node, IProvideMetadata obj) // v8: IMetadataReader
		where T : IHasDirectivesNode
	{
		var deprecationReason = (obj as IProvideDeprecationReason)?.DeprecationReason;
		var appliedDirectives = obj.GetAppliedDirectives();
		if (appliedDirectives?.Count > 0)
		{
			var directives = new List<GraphQLDirective>(appliedDirectives.Count + (deprecationReason is not null ? 1 : 0));
			foreach (var appliedDirective in appliedDirectives)
			{
				directives.Add(ExportAppliedDirective(appliedDirective));

				// Do not add the @deprecated directive twice; give preference to the directive
				// set within the metadata rather than the DeprecationReason property.
				if (appliedDirective.Name.EqualsIgnoreCase("deprecated"))
					deprecationReason = null;
			}
			node.Directives = new(directives);
		}

		if (deprecationReason is not null)
		{
			var directive = new GraphQLDirective(new("deprecated"))
			{
				Arguments = deprecationReason.IsNotBlank() ? new(new(1) { new(new("reason"), new GraphQLStringValue(deprecationReason)) }) : null
			};
			if (node.Directives is not null)
				node.Directives.Items.Add(directive);
			else
				node.Directives = new([directive]);
		}

		return node;
	}

	/// <summary>
	/// Exports the specified <see cref="AppliedDirective"/> as a <see cref="GraphQLDirective"/>.
	/// </summary>
	protected virtual GraphQLDirective ExportAppliedDirective(AppliedDirective appliedDirective)
	{
		var schemaDirective = this.Schema.Directives[appliedDirective.Name]
			?? throw new InvalidOperationException($"Could not find a directive named '{appliedDirective.Name}' defined within the schema.");
		var directive = new GraphQLDirective(new(appliedDirective.Name));
		if (appliedDirective.ArgumentsCount > 0)
		{
			var arguments = new List<GraphQLArgument>(appliedDirective.ArgumentsCount);
			foreach (var argument in appliedDirective)
			{
				arguments.Add(this.ExportAppliedDirectiveArgument(schemaDirective, argument));
			}

			directive.Arguments = new GraphQLArguments(arguments);
		}

		return directive;
	}

	/// <summary>
	/// Exports the specified <see cref="DirectiveArgument"/> as a <see cref="GraphQLArgument"/>.
	/// This requires a reference to the <see cref="Directive"/> that the argument is defined on
	/// in order to convert the value to an AST.
	/// </summary>
	protected virtual GraphQLArgument ExportAppliedDirectiveArgument(Directive directive, DirectiveArgument argument)
	{
		var directiveArgument = directive[argument.Name]
			?? throw new InvalidOperationException($"Unable to find argument '{argument.Name}' on directive '{directive.Name}'.");

		return new(new(argument.Name), directiveArgument.ResolvedType!.ToAST(argument.Value));
	}
}
