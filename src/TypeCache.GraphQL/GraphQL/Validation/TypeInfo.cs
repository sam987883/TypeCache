using GraphQL.Types;
using GraphQLParser;
using GraphQLParser.AST;
using TypeCache.Extensions;

namespace GraphQL.Validation;

/// <summary>
/// Provides information pertaining to the current state of the AST tree while being walked.
/// Thus, validation rules checking is designed for sequential execution.
/// </summary>
public class TypeInfo : INodeVisitor
{
	private readonly ISchema _schema;
	private readonly Stack<IGraphType?> _typeStack = new();
	private readonly Stack<IGraphType?> _inputTypeStack = new();
	private readonly Stack<IGraphType> _parentTypeStack = new();
	private readonly Stack<IFieldType?> _fieldDefStack = new();
	private readonly Stack<ASTNode> _ancestorStack = new();
	private Directive? _directive;
	private QueryArgument? _argument;

	/// <summary>
	/// Initializes a new instance for the specified schema.
	/// </summary>
	/// <param name="schema"></param>
	public TypeInfo(ISchema schema)
	{
		_schema = schema;
	}

	private static T? PeekElement<T>(Stack<T> from, int index)
	{
		if (index == 0)
			return from.Count > 0 ? from.Peek() : default;
		else
		{
			if (index >= from.Count)
				throw new InvalidOperationException($"Stack contains only {from.Count} items");

			var e = from.GetEnumerator();
			e.Move(index);
			return e.Current;
		}
	}

	/// <summary>
	/// Returns an ancestor of the current node.
	/// </summary>
	/// <param name="index">Index of the ancestor; 0 for the node itself, 1 for the direct ancestor and so on.</param>
	public ASTNode? GetAncestor(int index)
		=> PeekElement(_ancestorStack, index);

	/// <summary>
	/// Returns the last graph type matched, or <see langword="null"/> if none.
	/// </summary>
	/// <param name="index">Index of the type; 0 for the top-most type, 1 for the direct ancestor and so on.</param>
	public IGraphType? GetLastType(int index = 0)
		=> PeekElement(_typeStack, index);

	/// <summary>
	/// Returns the last input graph type matched, or <see langword="null"/> if none.
	/// </summary>
	/// <param name="index">Index of the type; 0 for the top-most type, 1 for the direct ancestor and so on.</param>
	public IGraphType? GetInputType(int index = 0)
		=> PeekElement(_inputTypeStack, index);

	/// <summary>
	/// Returns the parent graph type of the current node, or <see langword="null"/> if none.
	/// </summary>
	/// <param name="index">Index of the type; 0 for the top-most type, 1 for the direct ancestor and so on.</param>
	public IGraphType? GetParentType(int index = 0)
		=> PeekElement(_parentTypeStack, index);

	/// <summary>
	/// Returns the last field type matched, or <see langword="null"/> if none.
	/// </summary>
	/// <param name="index">Index of the field; 0 for the top-most field, 1 for the direct ancestor and so on.</param>
	public IFieldType? GetFieldDef(int index = 0)
		=> PeekElement(_fieldDefStack, index);

	/// <summary>
	/// Returns the last directive specified, or <see langword="null"/> if none.
	/// </summary>
	public Directive? GetDirective()
		=> this._directive;

	/// <summary>
	/// Returns the last query argument matched, or <see langword="null"/> if none.
	/// </summary>
	public QueryArgument? GetArgument()
		=> this._argument;

	/// <inheritdoc/>
	public ValueTask EnterAsync(ASTNode node, ValidationContext context)
	{
		this._ancestorStack.Push(node);

		if (node is GraphQLSelectionSet)
		{
			this._parentTypeStack.Push(GetLastType()!);
			return default;
		}

		if (node is GraphQLField field)
		{
			var parentType = _parentTypeStack.Peek().GetNamedGraphType();
			var fieldType = GetFieldDef(_schema, parentType, field);
			this._fieldDefStack.Push(fieldType);

			var targetType = fieldType?.ResolvedType;
			this._typeStack.Push(targetType);

			return default;
		}

		if (node is GraphQLDirective directive)
			_directive = _schema.Directives[directive.Name.StringValue];

		if (node is GraphQLOperationDefinition op)
		{
			IGraphType? type = op.Operation switch
			{
				OperationType.Query => _schema.Query,
				OperationType.Mutation => _schema.Mutation,
				OperationType.Subscription => _schema.Subscription,
				_ => null
			};

			this._typeStack.Push(type);
			return default;
		}

		if (node is GraphQLFragmentDefinition def1)
		{
			var type = _schema.AllTypes[def1.TypeCondition.Type.Name];
			this._typeStack.Push(type);
			return default;
		}

		if (node is GraphQLInlineFragment def)
		{
			var type = def.TypeCondition is not null ? _schema.AllTypes[def.TypeCondition.Type.Name] : GetLastType();
			this._typeStack.Push(type);
			return default;
		}

		if (node is GraphQLVariableDefinition varDef)
		{
			var inputType = varDef.Type.GraphTypeFromType(_schema);
			this._inputTypeStack.Push(inputType);
			return default;
		}

		if (node is GraphQLArgument argAst)
		{
			this._argument = this.GetDirective()?[argAst.Name] ?? this.GetFieldDef()?[argAst.Name];
			this._inputTypeStack.Push(this._argument?.ResolvedType);
		}

		if (node is GraphQLListValue)
		{
			var type = GetInputType()?.GetNamedGraphType();
			this._inputTypeStack.Push(type);
		}

		if (node is GraphQLObjectField objectField)
		{
			var objectType = GetInputType()?.GetNamedGraphType();
			IGraphType? fieldType = null;
			FieldType? inputField = null;

			if (objectType is IInputObjectGraphType complexType)
			{
				inputField = complexType[objectField.Name];
				fieldType = inputField?.ResolvedType;
			}

			this._inputTypeStack.Push(fieldType);
			this._fieldDefStack.Push(inputField);
		}

		return default;
	}

	/// <inheritdoc/>
	public async ValueTask LeaveAsync(ASTNode node, ValidationContext context)
	{
		this._ancestorStack.Pop();

		if (node is GraphQLSelectionSet)
		{
			this._parentTypeStack.Pop();
		}
		else if (node is GraphQLField)
		{
			this._fieldDefStack.Pop();
			this._typeStack.Pop();
		}
		else if (node is GraphQLDirective)
		{
			this._directive = null;
		}
		else if (node is GraphQLOperationDefinition || node is GraphQLFragmentDefinition || node is GraphQLInlineFragment)
		{
			this._typeStack.Pop();
		}
		else if (node is GraphQLVariableDefinition)
		{
			this._inputTypeStack.Pop();
		}
		else if (node is GraphQLArgument)
		{
			this._argument = null;
			this._inputTypeStack.Pop();
		}
		else if (node is GraphQLListValue)
		{
			this._inputTypeStack.Pop();
		}
		else if (node is GraphQLObjectField)
		{
			this._inputTypeStack.Pop();
			this._fieldDefStack.Pop();
		}

		await ValueTask.CompletedTask;
	}

	private static FieldType? GetFieldDef(ISchema schema, IGraphType parentType, GraphQLField field)
		=> field.Name switch
		{
			GraphQLName name when name == schema.SchemaMetaFieldType.Name && object.Equals(schema.Query, parentType) => schema.SchemaMetaFieldType,
			GraphQLName name when name == schema.TypeMetaFieldType.Name && object.Equals(schema.Query, parentType) => schema.TypeMetaFieldType,
			GraphQLName name when name == schema.TypeNameMetaFieldType.Name && parentType.IsCompositeType() => schema.TypeNameMetaFieldType,
			GraphQLName name when parentType is IObjectGraphType => ((IObjectGraphType)parentType)[name],
			_ => null
		};

	/// <summary>
	/// Tracks already visited fragments to maintain O(N) and to ensure that cycles
	/// are not redundantly reported.
	/// </summary>
	internal HashSet<ROM>? NoFragmentCycles_VisitedFrags;
	/// <summary>
	/// Array of AST nodes used to produce meaningful errors
	/// </summary>
	internal Stack<GraphQLFragmentSpread>? NoFragmentCycles_SpreadPath;
	/// <summary>
	/// Position in the spread path
	/// </summary>
	internal Dictionary<ROM, int>? NoFragmentCycles_SpreadPathIndexByName;

	internal HashSet<ROM>? NoUndefinedVariables_VariableNameDefined;

	internal List<GraphQLOperationDefinition>? NoUnusedFragments_OperationDefs;
	internal List<GraphQLFragmentDefinition>? NoUnusedFragments_FragmentDefs;

	internal List<GraphQLVariableDefinition>? NoUnusedVariables_VariableDefs;

	internal Dictionary<ROM, GraphQLArgument>? UniqueArgumentNames_KnownArgs;

	internal Dictionary<ROM, GraphQLFragmentDefinition>? UniqueFragmentNames_KnownFragments;

	internal Stack<Dictionary<ROM, GraphQLValue>>? UniqueInputFieldNames_KnownNameStack;
	internal Dictionary<ROM, GraphQLValue>? UniqueInputFieldNames_KnownNames;

	internal HashSet<ROM>? UniqueOperationNames_Frequency;

	internal Dictionary<ROM, GraphQLVariableDefinition>? UniqueVariableNames_KnownVariables;

	internal Dictionary<ROM, GraphQLVariableDefinition>? VariablesInAllowedPosition_VarDefMap;
}
