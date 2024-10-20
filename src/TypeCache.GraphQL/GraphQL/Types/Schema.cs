using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using GraphQL.Conversion;
using GraphQL.DI;
using GraphQL.Introspection;
using GraphQL.Utilities;
using GraphQLParser.AST;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace GraphQL.Types;

/// <inheritdoc cref="ISchema"/>
[DebuggerTypeProxy(typeof(SchemaDebugView))]
public class Schema : MetadataProvider, ISchema
{
	private sealed class SchemaDebugView(Schema schema)
	{
		public Dictionary<string, object?> Metadata => schema.Metadata;

		public ExperimentalFeatures Features => schema.Features;

		public INameConverter NameConverter => schema.NameConverter;

		public bool Initialized => schema.Initialized;

		public string? Description => schema.Description;

		public IObjectGraphType Query => schema.Query;

		public IObjectGraphType? Mutation => schema.Mutation;

		public IObjectGraphType? Subscription => schema.Subscription;

		public ISchemaFilter Filter => schema.Filter;

		/// <inheritdoc/>
		public SchemaDirectives Directives => schema.Directives;

		/// <inheritdoc/>
		public SchemaTypes? AllTypes => schema._allTypes;

		public string AllTypesMessage => schema._allTypes is null ? "AllTypes property too early initialization was suppressed to prevent unforeseen consequences. You may click Raw View in debugger window to evaluate all properties." : string.Empty;

		public IEnumerable<Type> AdditionalTypes => schema.AdditionalTypes;

		public IEnumerable<IGraphType> AdditionalTypeInstances => schema.AdditionalTypeInstances;

		/// <inheritdoc/>
		public FieldType? SchemaMetaFieldType => this.AllTypes?.SchemaMetaFieldType;

		/// <inheritdoc/>
		public FieldType? TypeMetaFieldType => this.AllTypes?.TypeMetaFieldType;

		/// <inheritdoc/>
		public FieldType? TypeNameMetaFieldType => this.AllTypes?.TypeNameMetaFieldType;

		public IEnumerable<(Type clrType, Type graphType)> TypeMappings => schema.TypeMappings;
	}

	private IServiceProvider _services;
	private SchemaTypes? _allTypes;
	private ExceptionDispatchInfo? _initializationException;
	private readonly object _allTypesInitializationLock = new();

	private List<Type>? _additionalTypes;
	private List<IGraphType>? _additionalInstances;

	private List<Type>? _visitorTypes;
	private List<ISchemaNodeVisitor>? _visitors;

	private List<(Type clrType, Type graphType)>? _clrToGraphTypeMappings;

	/// <summary>
	/// Create an instance of <see cref="Schema"/> with a specified <see cref="IServiceProvider"/>, used
	/// to create required objects.
	/// Pulls registered <see cref="IConfigureSchema"/> instances from <paramref name="services"/> and
	/// executes them.
	/// </summary>
	public Schema(IServiceProvider services)
		: this(services, true)
	{
	}

	/// <summary>
	/// Create an instance of <see cref="Schema"/> with a specified <see cref="IServiceProvider"/>, used
	/// to create required objects.
	/// If <paramref name="runConfigurations"/> is <see langword="true"/>, pulls registered
	/// <see cref="IConfigureSchema"/> instances from <paramref name="services"/> and executes them.
	/// </summary>
	public Schema(IServiceProvider services, bool runConfigurations = true)
		: this(services, (runConfigurations ? services.GetService(typeof(IEnumerable<IConfigureSchema>)) as IEnumerable<IConfigureSchema> : null)!)
	{
	}

	/// <summary>
	/// Create an instance of <see cref="Schema"/> with a specified <see cref="IServiceProvider"/>, used
	/// to create required objects.
	/// Executes the specified <see cref="IConfigureSchema"/> instances on the schema, if any.
	/// </summary>
	public Schema(IServiceProvider services, IEnumerable<IConfigureSchema> configurations)
	{
		this._services = services;

		this.Directives = new();
		this.Directives.UnionWith([this.Directives.Include, this.Directives.Skip, this.Directives.Deprecated]);

		if (configurations is not null)
		{
			foreach (var configuration in configurations)
			{
				configuration.Configure(this, services);
			}
		}
	}

	/// <inheritdoc/>
	public ExperimentalFeatures Features { get; set; } = new();

	/// <inheritdoc/>
	public INameConverter NameConverter { get; set; } = CamelCaseNameConverter.Instance;

	/// <inheritdoc/>
	public bool Initialized { get; private set; }

	// TODO: It would be worthwhile to think at all about how to redo the design so that such a situation does not arise.
	private void CheckInitialized([CallerMemberName] string name = "")
	{
		if (this.Initialized)
			throw new InvalidOperationException($"Schema is already initialized and sealed for modifications. You should call '{name}' only when Schema.Initialized = false.");
	}

	/// <inheritdoc/>
	public void Initialize()
	{
		if (this.Initialized)
			return;

		lock (this._allTypesInitializationLock)
		{
			if (this.Initialized)
				return;

			this._initializationException?.Throw();

			try
			{
				this.CreateAndInitializeSchemaTypes();

				this.Initialized = true;
			}
			catch (Exception ex)
			{
				this._initializationException = ExceptionDispatchInfo.Capture(ex);
				throw;
			}
		}
	}

	/// <inheritdoc/>
	public string? Description { get; set; }

	/// <inheritdoc/>
	public IObjectGraphType Query { get; set; } = null!;

	/// <inheritdoc/>
	public IObjectGraphType? Mutation { get; set; }

	/// <inheritdoc/>
	public IObjectGraphType? Subscription { get; set; }

	/// <inheritdoc/>
	public ISchemaFilter Filter { get; set; } = new DefaultSchemaFilter();

	/// <inheritdoc/>
	public SchemaDirectives Directives { get; }

	/// <inheritdoc/>
	public SchemaTypes AllTypes
	{
		get
		{
			if (this._allTypes is null)
				this.Initialize();

			return this._allTypes!;
		}
	}

	/// <inheritdoc/>
	public IEnumerable<Type> AdditionalTypes => this._additionalTypes ?? Enumerable.Empty<Type>();

	/// <inheritdoc/>
	public IEnumerable<IGraphType> AdditionalTypeInstances => this._additionalInstances ?? Enumerable.Empty<IGraphType>();

	/// <inheritdoc/>
	public FieldType SchemaMetaFieldType => this.AllTypes.SchemaMetaFieldType;

	/// <inheritdoc/>
	public FieldType TypeMetaFieldType => this.AllTypes.TypeMetaFieldType;

	/// <inheritdoc/>
	public FieldType TypeNameMetaFieldType => this.AllTypes.TypeNameMetaFieldType;

	/// <inheritdoc/>
	public void RegisterVisitor(ISchemaNodeVisitor visitor)
	{
		this.CheckInitialized();

		visitor.ThrowIfNull();

		this._visitors ??= [visitor];
		if (!this._visitors.Contains(visitor))
			this._visitors.Add(visitor);
	}

	/// <inheritdoc/>
	public void RegisterVisitor(Type type)
	{
		this.CheckInitialized();

		type.ThrowIfNull();

		if (!typeof(ISchemaNodeVisitor).IsAssignableFrom(type))
			throw new ArgumentOutOfRangeException(nameof(type), $"Type must be of {nameof(ISchemaNodeVisitor)}.");

		this._visitorTypes ??= [type];
		if (!this._visitorTypes.Contains(type))
			this._visitorTypes.Add(type);
	}

	/// <inheritdoc/>
	public void RegisterType(IGraphType type)
	{
		this.CheckInitialized();

		type.ThrowIfNull();

		this._additionalInstances ??= [type];
		if (!this._additionalInstances.Contains(type))
			this._additionalInstances.Add(type);
	}

	/// <inheritdoc/>
	public void RegisterType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type)
	{
		this.CheckInitialized();

		type.ThrowIfNull();

		if (!typeof(IGraphType).IsAssignableFrom(type))
			throw new ArgumentOutOfRangeException(nameof(type), $"Type must be of {nameof(IGraphType)}.");

		this._additionalTypes ??= [type];
		if (!this._additionalTypes.Contains(type))
			this._additionalTypes.Add(type);
	}

	/// <inheritdoc/>
	public void RegisterTypes(params Type[] types)
	{
		this.CheckInitialized();

		types.ThrowIfNull();

		foreach (var type in types)
		{
			this.RegisterType(type);
		}
	}

	/// <inheritdoc/>
	public void RegisterTypeMapping(
		Type clrType,
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type graphType)
	{
		clrType.ThrowIfNull();
		graphType.ThrowIfNull();

		if (typeof(IGraphType).IsAssignableFrom(clrType))
			throw new ArgumentOutOfRangeException(nameof(clrType), $"{clrType.FullName}' is already a GraphType (i.e. not CLR type like System.DateTime or System.String). You must specify CLR type instead of GraphType.");

		if (!typeof(IGraphType).IsAssignableFrom(graphType))
			throw new ArgumentOutOfRangeException(nameof(graphType), $"{graphType.FullName}' must be a GraphType (i.e. not CLR type like System.DateTime or System.String). You must specify GraphType type instead of CLR type.");

		this._clrToGraphTypeMappings ??= new();
		this._clrToGraphTypeMappings.Add((clrType, graphType));
	}

	/// <inheritdoc/>
	public IEnumerable<(Type clrType, Type graphType)> TypeMappings => this._clrToGraphTypeMappings ?? Enumerable.Empty<(Type, Type)>();

	private void CreateAndInitializeSchemaTypes()
	{
		this._allTypes = new SchemaTypes(this, _services);

		try
		{
			if (this._visitors is not null)
			{
				foreach (var visitor in this._visitors)
					visitor.Run(this);
			}

			if (this._visitorTypes is not null)
			{
				foreach (var type in this._visitorTypes)
					((ISchemaNodeVisitor)this._services.GetRequiredService(type)).Run(this);
			}

			this.Validate();
		}
		catch
		{
			this._allTypes = null;
			throw;
		}
	}

	/// <summary>
	/// Validates correctness of the created schema. This method is called only once - during schema initialization.
	/// </summary>
	protected virtual void Validate()
	{
		//TODO: add different validations, also see SchemaBuilder.Validate
		//TODO: checks for parsed SDL may be expanded in the future, see https://github.com/graphql/graphql-spec/issues/653
		// Do not change the order of these validations.
		CoerceInputTypeDefaultValues();
		Singleton<SchemaValidationVisitor>.Instance.Run(this);
		Singleton<AppliedDirectivesValidationVisitor>.Instance.Run(this);
	}

	/// <summary>
	/// Coerces input types' default values when those values are <see cref="GraphQLValue"/> nodes.
	/// This is applicable when the <see cref="SchemaBuilder"/> is used to build the schema.
	/// </summary>
	protected virtual void CoerceInputTypeDefaultValues()
	{
		var completed = new HashSet<IInputObjectGraphType>();
		var inProcess = new Stack<IInputObjectGraphType>();
		foreach (var type in AllTypes.Dictionary.Values)
		{
			if (type is IInputObjectGraphType inputType)
				ExamineType(inputType, completed, inProcess);
		}

		static void ExamineType(IInputObjectGraphType inputType, HashSet<IInputObjectGraphType> completed, Stack<IInputObjectGraphType> inProcess)
		{
			if (completed.Contains(inputType))
				return;

			if (inProcess.Contains(inputType))
				throw new InvalidOperationException($"Default values in input types cannot contain a circular dependency loop. Please resolve dependency loop between the following types: {string.Join(", ", inProcess.Select(x => $"'{x.Name}'"))}.");

			inProcess.Push(inputType);
			foreach (var field in inputType.Fields)
			{
				if (field.DefaultValue is GraphQLValue value)
				{
					var baseType = field.ResolvedType!.GetNamedGraphType();
					if (baseType is IInputObjectGraphType inputFieldType)
						ExamineType(inputFieldType, completed, inProcess);

					field.DefaultValue = Execution.ExecutionHelper.CoerceValue(field.ResolvedType!, value).Value;
				}
			}
			inProcess.Pop();
			completed.Add(inputType);
		}
	}
}
