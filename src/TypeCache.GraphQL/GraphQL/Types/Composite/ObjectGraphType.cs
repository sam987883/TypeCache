using GraphQL.Builders;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Types;

namespace GraphQL.Types;

/// <summary>
/// Represents an interface for all object (that is, having their own properties) output graph types.
/// </summary>
public interface IObjectGraphType : IGraphType
{
	FieldType? this[string field] => this.Fields.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(field));

	FieldType? this[GraphQLName field] => this.Fields.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(field.StringValue));

	ISet<FieldType> Fields { get; }

	ISet<RuntimeTypeHandle> Interfaces { get; }

	ISet<IInterfaceGraphType> ResolvedInterfaces { get; }

	RuntimeTypeHandle TypeHandle { get; }
}

/// <summary>
/// Represents a default base class for all object (that is, having their own properties) output graph types.
/// </summary>
/// <typeparam name="TSourceType">The type of the object that this graph represents. More specifically, the .NET type of the source property within field resolvers for this graph.</typeparam>
public class ObjectGraphType<[NotAGraphType] TSourceType> : GraphType, IObjectGraphType
{
	/// <inheritdoc/>
	public void AddResolvedInterface(IInterfaceGraphType graphType)
	{
		graphType.ThrowIfNull();

		_ = graphType.IsValidInterfaceFor(this, throwError: true);
		this.ResolvedInterfaces.Add(graphType);
	}

	public ISet<FieldType> Fields { get; } = new HashSet<FieldType>();

	public ISet<RuntimeTypeHandle> Interfaces { get; } = new HashSet<RuntimeTypeHandle>();

	public ISet<IInterfaceGraphType> ResolvedInterfaces { get; } = new HashSet<IInterfaceGraphType>();

	public RuntimeTypeHandle TypeHandle { get; } = typeof(TSourceType).TypeHandle;

	/// <inheritdoc/>
	public virtual FieldType AddField(FieldType fieldType)
	{
		ArgumentNullException.ThrowIfNull(fieldType);
		ArgumentNullException.ThrowIfNull(fieldType.Name);

		if (!fieldType.ResolvedType.IsGraphQLTypeReference())
		{
			if (this is IInputObjectGraphType)
			{
				if ((fieldType.ResolvedType?.IsInputType() ?? fieldType.Type?.IsInputType()) is not true)
					throw new ArgumentOutOfRangeException(nameof(fieldType),
						$"Input type '{this.Name ?? this.GetType().GetTypeName()}' can have fields only of input types: ScalarGraphType, EnumerationGraphType or IInputObjectGraphType. Field '{fieldType.Name}' has an output type.");
			}
			else
			{
				if ((fieldType.ResolvedType?.IsOutputType() ?? fieldType.Type?.IsOutputType()) is not true)
					throw new ArgumentOutOfRangeException(nameof(fieldType),
						$"Output type '{this.Name ?? this.GetType().GetTypeName()}' can have fields only of output types: ScalarGraphType, ObjectGraphType, InterfaceGraphType, UnionGraphType or EnumerationGraphType. Field '{fieldType.Name}' has an input type.");
			}
		}

		if (this.Fields.Any(_ => _.Name.EqualsIgnoreCase(fieldType.Name)))
			throw new ArgumentOutOfRangeException(nameof(fieldType),
				$"A field with the name '{fieldType.Name}' is already registered for GraphType '{Name ?? GetType().Name}'");

		if (fieldType.ResolvedType is null)
		{
			if (fieldType.Type is null)
				throw new ArgumentOutOfRangeException(nameof(fieldType),
					$"The declared field '{fieldType.Name ?? fieldType.GetType().GetTypeName()}' on '{Name ?? GetType().GetTypeName()}' requires a field '{nameof(fieldType.Type)}' when no '{nameof(fieldType.ResolvedType)}' is provided.");

			if (!fieldType.Type.IsGraphType())
				throw new ArgumentOutOfRangeException(nameof(fieldType),
					$"The declared Field type '{fieldType.Type.Name}' should derive from GraphType.");
		}

		this.Fields.Add(fieldType);

		return fieldType;
	}

	/// <summary>
	/// Adds a new field to the complex graph type and returns a builder for this newly added field.
	/// </summary>
	/// <typeparam name="TGraphType">The .NET type of the graph type of this field.</typeparam>
	/// <typeparam name="TReturnType">The return type of the field resolver.</typeparam>
	/// <param name="name">The name of the field.</param>
	public virtual FieldBuilder<TSourceType, TReturnType> Field<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TGraphType, [NotAGraphType] TReturnType>(string name)
		where TGraphType : IGraphType
	{
		var builder = FieldBuilder<TSourceType, TReturnType>.Create(name, typeof(TGraphType));
		this.AddField(builder.FieldType);
		return builder;
	}

	/// <summary>
	/// Adds a new field to the complex graph type and returns a builder for this newly added field.
	/// </summary>
	/// <typeparam name="TGraphType">The .NET type of the graph type of this field.</typeparam>
	/// <param name="name">The name of the field.</param>
	public virtual FieldBuilder<TSourceType, object> Field<TGraphType>(string name)
		where TGraphType : IGraphType
		=> this.Field<TGraphType, object>(name);
}

/// <summary>
/// Represents a default base class for all object (that is, having their own properties) output graph types.
/// </summary>
public class ObjectGraphType : ObjectGraphType<object?>
{
}
