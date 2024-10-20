using System.Collections;
using System.Collections.Frozen;
using System.Numerics;
using System.Text;
using GraphQL.Conversion;
using GraphQL.Introspection;
using GraphQL.Types.Collections;
using GraphQL.Utilities;
using GraphQLParser;
using GraphQLParser.AST;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;
using TypeCache.Utilities;

namespace GraphQL.Types;

/// <summary>
/// A class that represents a list of all the graph types utilized by a schema.
/// Also provides lookup for all schema types and has algorithms for discovering them.
/// <br/>
/// NOTE: After creating an instance of this class, its contents cannot be changed.
/// </summary>
public sealed class SchemaTypes : IEnumerable<IGraphType>
{
	private const string INITIALIZATIION_TRACE_KEY = "__INITIALIZATIION_TRACE_KEY__";

	[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GraphQLClrInputTypeReference<>))]
	[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GraphQLClrOutputTypeReference<>))]
	[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ListGraphType<>))]
	[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NonNullGraphType<>))]
	static SchemaTypes()
	{
		// The above attributes preserve those classes when T is a reference type, but not
		// when T is a value type, which is necessary for GraphQL CLR type references.
		// Also, specifying the closed generic type does not help, so the only way to force
		// the trimmer to preserve types such as GraphQLClrInputTypeReference<int> is to
		// directly reference the type within the compiled MSIL, even if the code does
		// not actually run. While this does not help with user defined structs, the combination
		// of the above attributes and below code will allow all built-in types to be handled
		// by the auto-registering graph types such as AutoRegisteringObjectGraphType and
		// similar code.
		var obj = new object();
		if (obj is not null) // always true
			return; // no need to actually execute the below code, but it must be present in the compiled IL

		// prevent trimming of these input and output type reference types
		Preserve<int>();
		Preserve<long>();
		Preserve<BigInteger>();
		Preserve<double>();
		Preserve<float>();
		Preserve<decimal>();
		Preserve<string>();
		Preserve<bool>();
		Preserve<DateTime>();
		Preserve<Half>();
		Preserve<DateOnly>();
		Preserve<TimeOnly>();
		Preserve<DateTimeOffset>();
		Preserve<TimeSpan>();
		Preserve<Guid>();
		Preserve<short>();
		Preserve<ushort>();
		Preserve<ulong>();
		Preserve<uint>();
		Preserve<byte>();
		Preserve<sbyte>();
		Preserve<Uri>();

		static void Preserve<T>()
		{
			// force the MSIL to contain strong references to the specified type
			GC.KeepAlive(typeof(GraphQLClrInputTypeReference<T>));
			GC.KeepAlive(typeof(GraphQLClrOutputTypeReference<T>));
		}
	}

	// Introspection types https://spec.graphql.org/October2021/#sec-Schema-Introspection
	private Dictionary<Type, IGraphType> _introspectionTypes;
	private TypeCollectionContext _context;
	private INameConverter _nameConverter;
	private bool _initialized;

	// Standard scalars https://spec.graphql.org/October2021/#sec-Scalars
	private readonly IReadOnlyDictionary<RuntimeTypeHandle, IGraphType> _builtInScalars = new IGraphType[]
	{
		Singleton<GraphQLStringType>.Instance,
		Singleton<GraphQLBooleanType>.Instance,
		Singleton<GraphQLNumberType>.Instance,
		Singleton<GraphQLNumberType<long>>.Instance,
	}.ToDictionary(_ => _.GetType().TypeHandle).ToFrozenDictionary();

	// .NET custom scalars
	private readonly IReadOnlyDictionary<RuntimeTypeHandle, IGraphType> _builtInCustomScalars = new IGraphType[]
	{
		Singleton<GraphQLBooleanType>.Instance,
		Singleton<GraphQLNumberType>.Instance,
		Singleton<GraphQLNumberType<sbyte>>.Instance,
		Singleton<GraphQLNumberType<short>>.Instance,
		Singleton<GraphQLNumberType<int>>.Instance,
		Singleton<GraphQLNumberType<long>>.Instance,
		Singleton<GraphQLNumberType<Int128>>.Instance,
		Singleton<GraphQLNumberType<BigInteger>>.Instance,
		Singleton<GraphQLNumberType<byte>>.Instance,
		Singleton<GraphQLNumberType<ushort>>.Instance,
		Singleton<GraphQLNumberType<uint>>.Instance,
		Singleton<GraphQLNumberType<ulong>>.Instance,
		Singleton<GraphQLNumberType<UInt128>>.Instance,
		Singleton<GraphQLStringType>.Instance,
		Singleton<GraphQLStringType<Guid>>.Instance,
		Singleton<GraphQLStringType<DateOnly>>.Instance,
		Singleton<GraphQLStringType<DateTime>>.Instance,
		Singleton<GraphQLStringType<DateTimeOffset>>.Instance,
		Singleton<GraphQLStringType<TimeOnly>>.Instance,
		Singleton<GraphQLStringType<TimeSpan>>.Instance,
		Singleton<GraphQLUriType>.Instance,
	}.ToDictionary(_ => _.GetType().TypeHandle).ToFrozenDictionary();

	/// <summary>
	/// Initializes a new instance for the specified schema, and with the specified type resolver.
	/// </summary>
	/// <param name="schema">A schema for which this instance is created.</param>
	/// <param name="serviceProvider">A service provider used to resolve graph types.</param>
	public SchemaTypes(ISchema schema, IServiceProvider serviceProvider)
	{
		this.TypeMetaFieldType = new()
		{
			Type = typeof(__Type),
			Description = "Request the type information of a single type.",
			Arguments = [new QueryArgument("name", typeof(NonNullGraphType<GraphQLStringType>))],
			Resolver = new CustomFieldResolver(context => (object?)context.Schema.AllTypes[context.GetArgument<string>("name")])
		};
		this.TypeMetaFieldType.SetName("__type", validate: false);

		this.SchemaMetaFieldType = new()
		{
			Type = typeof(__Schema),
			Description = "Access the current type schema of this server.",
			Resolver = new CustomFieldResolver(context => (object)context.Schema)
		};
		this.SchemaMetaFieldType.SetName("__schema", validate: false);

		this.TypeNameMetaFieldType = new()
		{
			Type = typeof(NonNullGraphType<GraphQLStringType>),
			Description = "The name of the current Object type at runtime.",
			Resolver = new CustomFieldResolver(context => (object)context.ParentType.Name)
		};
		this.TypeNameMetaFieldType.SetName("__typename", validate: false);

		var graphTypeMappings = serviceProvider.GetService<IEnumerable<IGraphTypeMappingProvider>>();
		Initialize(schema, serviceProvider, graphTypeMappings);
	}

	/// <summary>
	/// Initializes the instance for the specified schema, and with the specified type resolver.
	/// </summary>
	/// <param name="schema">A schema for which this instance is created.</param>
	/// <param name="serviceProvider">A service provider used to resolve graph types.</param>
	/// <param name="graphTypeMappings">A service used to map CLR types to graph types.</param>
	private void Initialize(ISchema schema, IServiceProvider serviceProvider, IEnumerable<IGraphTypeMappingProvider>? graphTypeMappings)
	{
		schema.ThrowIfNull();
		serviceProvider.ThrowIfNull();

		if (_initialized)
			throw new InvalidOperationException("SchemaTypes has already been initialized.");

		this._initialized = true;

		if (schema.TypeMappings is not null)
		{
			// this code could be moved into Schema
			var additionalMappings = schema.TypeMappings.Select(_ => new ManualGraphTypeMappingProvider(_.clrType, _.graphType));
			graphTypeMappings = graphTypeMappings is not null ? graphTypeMappings.Concat(additionalMappings).ToList() : additionalMappings.ToList();
		}

		schema.Directives.ThrowIfNull();

		this._typeDictionary = new Dictionary<Type, IGraphType>();
		if (schema.Features.DeprecationOfInputValues)
		{
			// TODO: remove this code block when the next version of the spec will be released
			schema.Directives.Deprecated.Locations.Add(GraphQLParser.AST.DirectiveLocation.ArgumentDefinition);
			schema.Directives.Deprecated.Locations.Add(GraphQLParser.AST.DirectiveLocation.InputFieldDefinition);
		}

		this._introspectionTypes = CreateIntrospectionTypes(schema.Features.AppliedDirectives, schema.Features.RepeatableDirectives, schema.Features.DeprecationOfInputValues);

		this._context = new TypeCollectionContext(
			type => BuildGraphQLType(type, _ => this._builtInScalars.TryGetValue(_.TypeHandle, out var graphType)
				? graphType : (this._introspectionTypes.TryGetValue(_, out graphType) ? graphType : (IGraphType)_.Create()!)),
			(name, type, context) =>
			{
				this.SetGraphType(name, type);
				context.AddType(name, type, null!);
			},
			graphTypeMappings,
			schema);

		var typeInstances = GetSchemaTypeInstances(schema, serviceProvider);

		// Add manually-added scalar types. To allow overriding of built-in scalars, these must be added
		// prior to adding any other types (including introspection types).
		using (this._context.Trace("Loop over manually-added scalar types from AdditionalTypeInstances"))
		{
			foreach (var type in typeInstances)
			{
				if (type is ScalarGraphType)
					AddType(type, _context);
			}
		}

		// Add introspection types. Note that introspection types rely on the
		// CamelCaseNameConverter, as some fields are defined in pascal case - e.g. Field(x => x.Name)
		_nameConverter = CamelCaseNameConverter.Instance;

		using (_context.Trace("__Schema root type"))
			AddType(_introspectionTypes[typeof(__Schema)], _context);

		// set the name converter properly
		_nameConverter = schema.NameConverter ?? CamelCaseNameConverter.Instance;

		var ctx = new TypeCollectionContext(
			type => _builtInScalars.TryGetValue(type.TypeHandle, out var graphType) ? graphType : (IGraphType)serviceProvider.GetRequiredService(type),
			(name, graphType, context) =>
			{
				if (this[name] is null)
				{
					using var _ = context.Trace("TypeCollectionContext.AddType delegate");
					AddType(graphType, context);
				}
			},
			graphTypeMappings,
			schema);

		using (ctx.Trace("Loop over manually-added non-scalar types from AdditionalTypeInstances"))
		{
			foreach (var type in typeInstances)
			{
				if (type is not ScalarGraphType)
					AddTypeIfNotRegistered(type, ctx);
			}
		}

		var types = GetSchemaTypeInstances(schema, serviceProvider);

		using (ctx.Trace("Loop over manually-added types from AdditionalTypes"))
		{
			foreach (var type in types)
			{
				AddTypeIfNotRegistered(type, ctx);
			}
		}

		// these fields must not have their field names translated by INameConverter; see HandleField
		using (ctx.Trace("__schema root field"))
			HandleField(null, SchemaMetaFieldType, ctx, false);
		using (ctx.Trace("__type root field"))
			HandleField(null, TypeMetaFieldType, ctx, false);
		using (ctx.Trace("__typename root field"))
			HandleField(null, TypeNameMetaFieldType, ctx, false);

		using (ctx.Trace("Loop for directives"))
		{
			foreach (var directive in schema.Directives)
			{
				using var _ = ctx.Trace("Directive '{0}'", directive.Name);
				HandleDirective(directive, ctx);
			}
		}

		this.ApplyTypeReferences();

		// https://github.com/graphql-dotnet/graphql-dotnet/issues/1004
		foreach (var fieldOwner in this.Dictionary.Values.OfType<IObjectGraphType>())
		{
			if (fieldOwner is ObjectGraphType implementation && implementation.ResolvedInterfaces.Count > 0)
			{
				foreach (var field in fieldOwner.Fields.Where(field => field.Description is null))
				{
					foreach (var resolvedInterfaces in implementation.ResolvedInterfaces)
					{
						var fieldFromInterface = resolvedInterfaces[field.Name];
						if (fieldFromInterface?.Description is not null)
						{
							field.Description = fieldFromInterface.Description;
							break;
						}
					}
				}
			}
		}

		Debug.Assert(ctx.InFlightRegisteredTypes.Count == 0);
		Debug.Assert((ctx.InitializationTrace?.Count ?? 0) == 0);
		Debug.Assert((_context.InitializationTrace?.Count ?? 0) == 0);

		_typeDictionary = null!; // not needed once initialization is complete
	}

	private static IEnumerable<IGraphType> GetSchemaTypeInstances(ISchema schema, IServiceProvider serviceProvider)
	{
		// Manually registered AdditionalTypeInstances and AdditionalTypes should be handled first.
		// This is necessary for the correct processing of overridden built-in scalars.

		foreach (var instance in schema.AdditionalTypeInstances)
			yield return instance;

		foreach (var type in schema.AdditionalTypes)
		{
			var type2 = type.GetNamedType();
			if (typeof(ScalarGraphType).IsAssignableFrom(type2))
				yield return (IGraphType)serviceProvider.GetRequiredService(type2);
		}

		//TODO: According to the specification, Query is a required type. But if you uncomment these lines, then the mass of tests begin to fail, because they do not set Query.
		// if (Query is null)
		//    throw new InvalidOperationException("Query root type must be provided. See https://spec.graphql.org/October2021/#sec-Schema-Introspection");

		if (schema.Query is not null)
			yield return schema.Query;

		if (schema.Mutation is not null)
			yield return schema.Mutation;

		if (schema.Subscription is not null)
			yield return schema.Subscription;
	}

	private static Dictionary<Type, IGraphType> CreateIntrospectionTypes(bool allowAppliedDirectives, bool allowRepeatable, bool deprecationOfInputValues)
		=> (allowAppliedDirectives
			? new IGraphType[]
			{
				Singleton<GraphQLEnumType<DirectiveLocation>>.Instance,
				new __DirectiveArgument(),
				new __AppliedDirective(),
				Singleton<GraphQLEnumType<TypeKind>>.Instance,
				new __EnumValue(true),
				new __Directive(true, allowRepeatable),
				new __Field(true, deprecationOfInputValues),
				new __InputValue(true, deprecationOfInputValues),
				new __Type(true, deprecationOfInputValues),
				new __Schema(true)
			}
			: new IGraphType[]
			{
				Singleton<GraphQLEnumType<DirectiveLocation>>.Instance,
                //new __DirectiveArgument(), forbidden
                //new __AppliedDirective(),  forbidden
				Singleton<GraphQLEnumType<TypeKind>>.Instance,
				new __EnumValue(false),
				new __Directive(false, allowRepeatable),
				new __Field(false, deprecationOfInputValues),
				new __InputValue(false, deprecationOfInputValues),
				new __Type(false, deprecationOfInputValues),
				new __Schema(false)
			})
		.ToDictionary(_ => _.GetType());

	/// <summary>
	/// Returns a dictionary that relates type names to graph types.
	/// </summary>
	internal Dictionary<ROM, IGraphType> Dictionary { get; } = new Dictionary<ROM, IGraphType>();
	private Dictionary<Type, IGraphType> _typeDictionary;

	/// <inheritdoc cref="IEnumerable.GetEnumerator"/>
	public IEnumerator<IGraphType> GetEnumerator() => Dictionary.Values.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <summary>
	/// Gets the count of all the graph types utilized by the schema.
	/// </summary>
	public int Count => Dictionary.Count;

	private IGraphType BuildGraphQLType(Type type, IGraphType resolvedType)
		=> BuildGraphQLType(type, _ => resolvedType);

	/// <summary>
	/// Returns a new instance of the specified graph type, using the specified resolver to
	/// instantiate a new instance if the required type cannot be found from the lookup table.
	/// Defaults to <see cref="Activator.CreateInstance(Type)"/> if no <paramref name="resolve"/>
	/// parameter is specified. List and non-null graph types are instantiated and their
	/// <see cref="IProvideResolvedType.ResolvedType"/> property is set to a new instance of
	/// the base (wrapped) type.
	/// </summary>
	private IGraphType BuildGraphQLType(Type type, Func<Type, IGraphType> resolve)
	{
		var local = resolve ?? (_ => (IGraphType)_.Create()!);
		resolve = t => FindGraphType(t) ?? local(t);

		if (type.IsGenericType)
		{
			if (type.GetGenericTypeDefinition() == typeof(NonNullGraphType<>))
			{
				var nonNull = (NonNullGraphType)type.Create()!;
				nonNull.ResolvedType = BuildGraphQLType(type.GenericTypeArguments[0], resolve);
				return nonNull;
			}

			if (type.GetGenericTypeDefinition() == typeof(ListGraphType<>))
			{
				var list = (ListGraphType)type.Create()!;
				list.ResolvedType = BuildGraphQLType(type.GenericTypeArguments[0], resolve);
				return list;
			}
		}

		return resolve(type) ?? throw new InvalidOperationException(
			$"Expected non-null value, but {nameof(resolve)} delegate return null for '{type.Name}'");
	}

	/// <summary>
	/// Returns a graph type instance from the lookup table by its GraphQL type name.
	/// </summary>
	public IGraphType? this[ROM typeName]
		=> !typeName.IsEmpty
			? (Dictionary.TryGetValue(typeName, out var type) ? type : null)
			: throw new ArgumentOutOfRangeException(nameof(typeName), "A type name is required to lookup.");

	/// <summary>
	/// Returns a graph type instance from the lookup table by its .NET type.
	/// </summary>
	/// <param name="type">The .NET type of the graph type.</param>
	private IGraphType? FindGraphType(Type type)
		=> this._typeDictionary?.TryGetValue(type, out var value) is true ? value : null;

	private void AddType(IGraphType type, TypeCollectionContext context)
	{
		if (type is null || type is GraphQLTypeReference)
			return;

		if (type is NonNullGraphType || type is ListGraphType)
			throw new ArgumentOutOfRangeException(nameof(type), "Only add root types.");

		type.Initialize(context.Schema);
		if (context.InitializationTrace is not null)
			type.WithMetadata(INITIALIZATIION_TRACE_KEY, string.Join(Environment.NewLine, context.InitializationTrace));

		this.SetGraphType(type.Name, type);

		if (type is IObjectGraphType complexType)
		{
			using var _ = context.Trace("Loop for fields of complex type '{0}'", complexType.Name);
			foreach (var field in complexType.Fields)
			{
				using var __ = context.Trace("Field '{0}.{1}'", complexType.Name, field.Name);
				this.HandleField(complexType, field, context, true);
			}
		}

		if (type is ObjectGraphType obj)
		{
			using var _ = context.Trace("Loop for interfaces of object type '{0}'", obj.Name);
			foreach (var interfaceTypeHandle in obj.Interfaces)
			{
				var interfaceType = interfaceTypeHandle.ToType();

				using var __ = context.Trace("Interface '{0}'", interfaceType.Name);

				object typeOrError = this.RebuildType(interfaceType, false, context.ClrToGraphTypeMappings);
				if (typeOrError is string error)
					throw new InvalidOperationException($"The GraphQL implemented type '{interfaceType.GetTypeName()}' for object graph type '{type.Name}' could not be derived implicitly. {error}");

				var objectInterface2 = (Type)typeOrError;
				if (this.AddTypeIfNotRegistered(objectInterface2, context) is IInterfaceGraphType interfaceInstance)
				{
					obj.ResolvedInterfaces.Add(interfaceInstance);

					interfaceInstance.AddPossibleType(obj);
					if (interfaceInstance.ResolveType is null)
						throw new InvalidOperationException($"Interface type '{interfaceInstance.Name}' does not provide a 'resolveType' function and possible Type '{obj.Name}' does not provide a 'isTypeOf' function. There is no way to resolve this possible type during execution.");
				}
			}
		}

		if (type is UnionGraphType union)
		{
			using var _ = context.Trace("Loop for possible types of union type '{0}'", union.Name);
			if (!union.Types.Any() && !union.PossibleTypes.Any())
				throw new InvalidOperationException($"Must provide types for Union '{union}'.");

			foreach (var unionedType in union.PossibleTypes)
			{
				using var __ = context.Trace("Possible graph type '{0}'", unionedType.Name);

				// skip references
				if (unionedType is GraphQLTypeReference)
					continue;

				this.AddTypeIfNotRegistered(unionedType, context);

				if (union.ResolveType is null)
					throw new InvalidOperationException($"Union type '{union.Name}' does not provide a 'resolveType' function and possible Type '{unionedType.Name}' does not provide a 'isTypeOf' function. There is no way to resolve this possible type during execution.");
			}

			foreach (var unionedType in union.Types)
			{
				using var __ = context.Trace("Possible clr type '{0}'", unionedType.Name);

				var typeOrError = RebuildType(unionedType, false, context.ClrToGraphTypeMappings);
				if (typeOrError is string error)
					throw new InvalidOperationException($"The GraphQL type '{unionedType.GetTypeName()}' for union graph type '{type.Name}' could not be derived implicitly. {error}");

				var unionedType2 = (Type)typeOrError;
				if (AddTypeIfNotRegistered(unionedType2, context) is not IObjectGraphType objType)
					throw new InvalidOperationException($"The GraphQL type '{unionedType.GetTypeName()}' for union graph type '{type.Name}' could not be derived implicitly. The resolved type is not an {nameof(IObjectGraphType)}.");

				if (union.ResolveType is null && objType is not null)
					throw new InvalidOperationException($"Union type '{union.Name}' does not provide a 'resolveType' function and possible Type '{objType.Name}' does not provide a 'isTypeOf' function. There is no way to resolve this possible type during execution.");

				union.AddPossibleType((ObjectGraphType)objType!);
			}
		}
	}

	private void HandleField(IObjectGraphType? parentType, FieldType field, TypeCollectionContext context, bool applyNameConverter)
	{
		// applyNameConverter will be false while processing the three root introspection query fields: __schema, __type, and __typename
		//
		// During processing of those three root fields, the NameConverter will be set to the schema's selected NameConverter,
		//   and the field names must not be processed by the NameConverter
		//
		// For other introspection types and fields, the NameConverter will be set to CamelCaseNameConverter at the time this
		//   code executes, and applyNameConverter will be true
		//
		// For any other fields, the NameConverter will be set to the schema's selected NameConverter at the time this code
		//   executes, and applyNameConverter will be true

		if (applyNameConverter)
		{
			field.Name = this._nameConverter.NameForField(field.Name, parentType!);
			NameValidator.ValidateNameOnSchemaInitialize(field.Name, NamedElement.Field);
		}

		if (field.ResolvedType is not null)
			this.AddTypeIfNotRegistered(field.ResolvedType, context);
		else if (field.Type is null)
			throw new InvalidOperationException($"Both ResolvedType and Type properties on field '{parentType?.Name}.{field.Name}' are null.");
		else
		{
			var type = field.Type.IsGraphType() ? field.Type : field.Type.ToGraphQLType(parentType is IInputObjectGraphType);
			var namedType = this.AddTypeIfNotRegistered(type, context);
			field.ResolvedType = this.BuildGraphQLType(type, namedType);
		}

		if (field.Arguments.Length > 0)
		{
			using var _ = context.Trace("Loop for arguments of field '{0}'", field.Name);
			field.Arguments.ForEach(arg =>
			{
				using var __ = context.Trace("Argument '{0}'", arg.Name);

				if (applyNameConverter)
				{
					arg.Name = _nameConverter.NameForArgument(arg.Name, parentType!, field);
					NameValidator.ValidateNameOnSchemaInitialize(arg.Name, NamedElement.Argument);
				}

				if (arg.ResolvedType is null)
				{
					if (arg.Type is null)
						throw new InvalidOperationException($"Both ResolvedType and Type properties on argument '{parentType?.Name}.{field.Name}.{arg.Name}' are null.");

					object typeOrError = RebuildType(arg.Type, true, context.ClrToGraphTypeMappings);
					if (typeOrError is string error)
						throw new InvalidOperationException($"The GraphQL type for argument '{parentType?.Name}.{field.Name}.{arg.Name}' could not be derived implicitly. " + error);

					arg.Type = (Type)typeOrError;

					var namedType = this.AddTypeIfNotRegistered(arg.Type, context);
					arg.ResolvedType = this.BuildGraphQLType(arg.Type, namedType);
				}
				else
					this.AddTypeIfNotRegistered(arg.ResolvedType, context);
			});
		}
	}

	private void HandleDirective(Directive directive, TypeCollectionContext context)
	{
		if (directive.Arguments.Length > 0)
		{
			using var _ = context.Trace("Loop for arguments of directive '{0}'", directive.Name);
			directive.Arguments.ForEach(arg =>
			{
				using var __ = context.Trace("Argument '{0}'", arg.Name);
				if (arg.ResolvedType is null)
				{
					if (arg.Type is null)
						throw new InvalidOperationException($"Both ResolvedType and Type properties on argument '{directive.Name}.{arg.Name}' are null.");

					object typeOrError = RebuildType(arg.Type, true, context.ClrToGraphTypeMappings);
					if (typeOrError is string error)
						throw new InvalidOperationException($"The GraphQL type for argument '{directive.Name}.{arg.Name}' could not be derived implicitly. {error}");

					arg.Type = (Type)typeOrError;

					var namedType = this.AddTypeIfNotRegistered(arg.Type, context);
					arg.ResolvedType = this.BuildGraphQLType(arg.Type, namedType);
				}
				else
				{
					this.AddTypeIfNotRegistered(arg.ResolvedType, context);
					arg.ResolvedType = this.ConvertTypeReference(directive, arg.ResolvedType);
				}
			});
		}
	}

	// https://github.com/graphql-dotnet/graphql-dotnet/pull/1010
	private void AddTypeWithLoopCheck(IGraphType resolvedType, TypeCollectionContext context, Type namedType)
	{
		if (context.InFlightRegisteredTypes.Any(_ => _ == namedType))
			throw new InvalidOperationException($@"A loop has been detected while registering schema types. There was an attempt to re-register '{namedType.FullName}' with instance of '{resolvedType.GetType().FullName}'. Make sure that your ServiceProvider is configured correctly.");

		context.InFlightRegisteredTypes.Push(namedType);

		try
		{
			using var _ = context.Trace("AddTypeWithLoopCheck for type '{0}'", namedType.Name);
			this.AddType(resolvedType, context);
		}
		finally
		{
			_ = context.InFlightRegisteredTypes.Pop();
		}
	}

	private IGraphType AddTypeIfNotRegistered(Type type, TypeCollectionContext context)
	{
		using var _ = context.Trace("AddTypeIfNotRegistered(Type, TypeCollectionContext) for type '{0}'", type.Name);

		var namedType = type.GetNamedType();
		var foundType = this.FindGraphType(namedType);
		if (foundType is not null)
			return foundType;

		if (namedType == typeof(GraphQLObjectType<PageInfo>))
		{
			foundType = Singleton<GraphQLObjectType<PageInfo>>.Instance;
			this.AddType(foundType, context);
		}
		else if (namedType.IsGenericType
			&& (namedType.Implements(typeof(GraphQLObjectType<>).MakeGenericType(typeof(Edge<>)))
				|| namedType.Implements(typeof(GraphQLObjectType<>).MakeGenericType(typeof(Connection<>)))))
		{
			foundType = (IGraphType)namedType.Create()!;
			this.AddType(foundType, context);
		}
		else if (this._builtInCustomScalars.TryGetValue(namedType.TypeHandle, out var builtInCustomScalar))
		{
			foundType = builtInCustomScalar;
			this.AddType(foundType, _context); // TODO: why _context instead of context here? See https://github.com/graphql-dotnet/graphql-dotnet/pull/3488
		}
		else
		{
			foundType = context.ResolveType(namedType);
			AddTypeWithLoopCheck(foundType, context, namedType);
		}

		return foundType;
	}

	private void AddTypeIfNotRegistered(IGraphType type, TypeCollectionContext context)
	{
		var namedGraphType = type.GetNamedGraphType();
		namedGraphType ??= context.ResolveType(type.GetNamedType()!);

		using var _ = context.Trace("AddTypeIfNotRegistered(IGraphType, TypeCollectionContext) for type '{0}'", namedGraphType.Name);

		var existingType = this[namedGraphType.Name];
		if (existingType is null)
			this.AddType(namedGraphType, context);
		else
			this.EnsureTypeEquality(existingType, namedGraphType, context);
	}

	private void EnsureTypeEquality(IGraphType existingType, IGraphType newType, TypeCollectionContext context)
	{
		if (object.ReferenceEquals(existingType, newType))
			return;

		// Ignore scalars
		if (existingType is ScalarGraphType && newType is ScalarGraphType)
			return;

		if (existingType.GetType() != newType.GetType())
			throw new InvalidOperationException($"Unable to register GraphType '{newType.GetType().GetTypeName()}' with the name '{newType.Name}'. The name '{newType.Name}' is already registered to '{existingType.GetType().GetTypeName()}'. Check your schema configuration.");

		// All other types are considered "potentially wrong" when being re-registered, throw detailed exception
		throw new InvalidOperationException(ErrorMessage());

		string ErrorMessage()
		{
			var errorBuilder = new StringBuilder()
				.Append("A different instance of the GraphType '")
				.Append(newType.GetType().GetTypeName())
				.Append("' with the name '")
				.Append(newType.Name)
				.Append("' has already been registered within the schema. Please use the same instance for all references within the schema, or use ")
				.Append(nameof(GraphQLTypeReference))
				.AppendLine(" to reference a type instantiated elsewhere.");

			if (GlobalSwitches.TrackGraphTypeInitialization)
				errorBuilder.Append("Existing type trace:").AppendLine().AppendLine()
					.Append(existingType.GetMetadata<string>(INITIALIZATIION_TRACE_KEY)).AppendLine().AppendLine()
					.Append("New type trace:").AppendLine().AppendLine()
					.AppendIf(context.InitializationTrace is not null, string.Join(Environment.NewLine, context.InitializationTrace!));
			else
				errorBuilder.Append("To view additional trace enable ")
					.Append(nameof(GlobalSwitches)).Append('.').Append(nameof(GlobalSwitches.TrackGraphTypeInitialization))
					.Append(" switch.");

			return errorBuilder.ToString();
		}
	}

	private object RebuildType(Type type, bool input, IEnumerable<IGraphTypeMappingProvider>? typeMappings)
	{
		if (!type.IsGenericType)
			return type;

		var genericDef = type.GetGenericTypeDefinition();
		if (genericDef == typeof(GraphQLClrOutputTypeReference<>) || genericDef == typeof(GraphQLClrInputTypeReference<>))
			return GetGraphType(type.GetGenericArguments()[0], input, typeMappings);

		var typeList = type.GetGenericArguments();
		var changed = false;
		for (int i = 0; i < typeList.Length; i++)
		{
			object typeOrError = RebuildType(typeList[i], input, typeMappings);
			if (typeOrError is string)
				return typeOrError;

			var changedType = (Type)typeOrError;
			changed |= changedType != typeList[i];
			typeList[i] = changedType;
		}

		return changed ? genericDef.MakeGenericType(typeList) : type;
	}

	private object GetGraphType(Type clrType, bool input, IEnumerable<IGraphTypeMappingProvider>? typeMappings)
	{
		var graphType = GetGraphTypeFromClrType(clrType, input, typeMappings);
		if (graphType is not null)
			return graphType;

		string additionalMessage = typeof(IGraphType).IsAssignableFrom(clrType)
			? $" Note that '{clrType.FullName}' is already a GraphType (i.e. not CLR type like System.DateTime or System.String). Most likely you need to specify corresponding CLR type instead of GraphType."
			: string.Empty;

		return $"Could not find type mapping from CLR type '{clrType.FullName}' to GraphType. Did you forget to register the type mapping with the '{nameof(ISchema)}.{nameof(ISchema.RegisterTypeMapping)}'?{additionalMessage}";
	}

	/// <summary>
	/// Returns a graph type for a specified input or output CLR type.
	/// This method is called when a graph type is specified as a <see cref="GraphQLClrInputTypeReference{T}"/> or <see cref="GraphQLClrOutputTypeReference{T}"/>.
	/// </summary>
	/// <param name="clrType">The CLR type to be mapped.</param>
	/// <param name="isInputType">Indicates if the CLR type should be mapped to an input or output graph type.</param>
	/// <param name="typeMappings">The list of registered type mappings on the schema.</param>
	/// <returns>The graph type to be used, or <see langword="null"/> if no match can be found.</returns>
	/// <remarks>
	/// This method should not return wrapped types such as <see cref="ListGraphType"/> or <see cref="NonNullGraphType"/>.
	/// </remarks>
	private Type? GetGraphTypeFromClrType(Type clrType, bool isInputType, IEnumerable<IGraphTypeMappingProvider>? typeMappings)
	{
		// check custom mappings first
		if (typeMappings is not null)
		{
			Type? mappedType = null;
			foreach (var mapping in typeMappings)
			{
				mappedType ??= mapping.GetGraphTypeFromClrType(clrType, isInputType);
				if (mappedType is not null)
					return mappedType;
			}
		}

		// create an enumeration graph type if applicable
		if (clrType.IsEnum)
			return typeof(GraphQLEnumType<>).MakeGenericType(clrType);

		// then built-in mappings
		return clrType.GetScalarType().ToGraphType();
	}

	private void ApplyTypeReferences()
	{
		// ToList() is a necessary measure here since otherwise we get System.InvalidOperationException: 'Collection was modified; enumeration operation may not execute.'
		foreach (var type in Dictionary.Values.ToList())
		{
			if (type is IObjectGraphType complexType)
			{
				foreach (var field in complexType.Fields)
				{
					field.ResolvedType = ConvertTypeReference(type, field.ResolvedType!);
					field.Arguments.ForEach(arg => arg.ResolvedType = ConvertTypeReference(type, arg.ResolvedType!));
				}
			}

			if (type is ObjectGraphType objectType)
			{
				var resolvedInterfaces = objectType.ResolvedInterfaces.Select(resolvedInterface =>
				{
					var interfaceType = (IInterfaceGraphType)ConvertTypeReference(objectType, resolvedInterface);
					if (interfaceType.ResolveType is null)
						throw new InvalidOperationException($"Interface type '{interfaceType.Name}' does not provide a 'resolveType' function.  There is no way to resolve this possible type during execution.");

					interfaceType.AddPossibleType(objectType);
					return interfaceType;
				});

				objectType.ResolvedInterfaces.Clear();
				objectType.ResolvedInterfaces.UnionWith(resolvedInterfaces);
			}

			if (type is UnionGraphType union)
			{
				foreach (var possibleType in union.PossibleTypes)
				{
					var unionType = ConvertTypeReference(union, possibleType) as IObjectGraphType;

					if (union.ResolveType is null && unionType is not null)
						throw new InvalidOperationException($"Union type '{union.Name}' does not provide a 'resolveType' function and possible Type '{union.Name}' does not provide a 'isTypeOf' function.  There is no way to resolve this possible type during execution.");
				}
			}
		}
	}

	private IGraphType ConvertTypeReference(INamedType parentType, IGraphType type)
	{
		if (type is NonNullGraphType nonNullGraphType)
		{
			nonNullGraphType.ResolvedType = ConvertTypeReference(parentType, nonNullGraphType.ResolvedType!);
			return nonNullGraphType;
		}

		if (type is ListGraphType listGraphType)
		{
			listGraphType.ResolvedType = ConvertTypeReference(parentType, listGraphType.ResolvedType!);
			return listGraphType;
		}

		if (type is GraphQLTypeReference reference)
		{
			var type2 = this[reference.TypeName];
			if (type2 is null)
			{
				type2 = _builtInScalars.Values.FirstOrDefault(t => t.Name == reference.TypeName) ?? _builtInCustomScalars.Values.FirstOrDefault(t => t.Name == reference.TypeName);
				if (type2 is not null)
					SetGraphType(type2.Name, type2);

				throw new InvalidOperationException($"Unable to resolve reference to type '{reference.TypeName}' on '{parentType.Name}'");
			}

			type = type2;
		}

		return type;
	}

	private void SetGraphType(string graphTypeName, IGraphType graphType)
	{
		graphTypeName.ThrowIfBlank();

		var type = graphType.GetType();
		if (Dictionary.TryGetValue(graphTypeName, out var existingGraphType))
		{
			if (ReferenceEquals(existingGraphType, graphType) || existingGraphType.GetType() == type)
			{
				// Soft schema configuration error.
				// Intentionally or inadvertently, a situation may arise when the same GraphType is registered more that one time.
				// This may be due to the simultaneous registration of GraphType instances and the GraphType types. In this case
				// the duplicate MUST be ignored, otherwise errors will occur.
			}
			else if (type.IsAssignableFrom(existingGraphType.GetType()) && typeof(ScalarGraphType).IsAssignableFrom(type))
			{
				// This can occur when a built-in scalar graph type is overridden by preregistering a replacement graph type that
				// has the same name and inherits from it.
				if (!_typeDictionary.ContainsKey(type))
					_typeDictionary.Add(type, existingGraphType);
			}
			else // Fatal schema configuration error.
				throw new InvalidOperationException($"Unable to register GraphType '{type.GetTypeName()}' with the name '{graphTypeName}'. The name '{graphTypeName}' is already registered to '{existingGraphType.GetType().GetTypeName()}'. Check your schema configuration.");
		}
		else
		{
			this.Dictionary.Add(graphTypeName, graphType);

			// if building a schema from code, the .NET types will not be unique, which should be ignored
			this._typeDictionary.TryAdd(type, graphType);
		}
	}

	/// <summary>
	/// Returns the <see cref="FieldType"/> instance for the <c>__schema</c> meta-field.
	/// </summary>
	internal FieldType SchemaMetaFieldType { get; }

	/// <summary>
	/// Returns the <see cref="FieldType"/> instance for the <c>__type</c> meta-field.
	/// </summary>
	internal FieldType TypeMetaFieldType { get; }

	/// <summary>
	/// Returns the <see cref="FieldType"/> instance for the <c>__typename</c> meta-field.
	/// </summary>
	internal FieldType TypeNameMetaFieldType { get; }
}
