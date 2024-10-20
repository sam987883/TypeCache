using GraphQL.Types;
using TypeCache.GraphQL.Resolvers;

namespace GraphQL.Builders;

/// <summary>
/// Builds a field for a graph with a specified source type and return type.
/// </summary>
/// <typeparam name="TSourceType">The type of <see cref="IResolveFieldContext.Source"/>.</typeparam>
/// <typeparam name="TReturnType">The type of the return value of the resolver.</typeparam>
public class FieldBuilder<[NotAGraphType] TSourceType, [NotAGraphType] TReturnType>
{
	/// <summary>
	/// Returns the generated field.
	/// </summary>
	public FieldType FieldType { get; }

	/// <summary>
	/// Initializes a new instance for the specified <see cref="Types.FieldType"/>.
	/// </summary>
	protected FieldBuilder(FieldType fieldType)
	{
		FieldType = fieldType;
	}

	public static FieldBuilder<TSourceType, TReturnType> Create(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type? type = null)
	{
		var fieldType = new FieldType
		{
			Name = name,
			Type = type,
		};
		return new FieldBuilder<TSourceType, TReturnType>(fieldType);
	}

	/// <summary>
	/// Sets the description of the field.
	/// </summary>
	public virtual FieldBuilder<TSourceType, TReturnType> Description(string? description)
	{
		FieldType.Description = description;
		return this;
	}

	/// <summary>
	/// Sets the deprecation reason of the field.
	/// </summary>
	public virtual FieldBuilder<TSourceType, TReturnType> DeprecationReason(string? deprecationReason)
	{
		FieldType.DeprecationReason = deprecationReason;
		return this;
	}

	/// <summary>
	/// Sets the resolver for the field.
	/// </summary>
	public virtual FieldBuilder<TSourceType, TReturnType> Resolve(IFieldResolver? resolver)
	{
		FieldType.Resolver = resolver;
		return this;
	}

	/// <inheritdoc cref="Resolve(IFieldResolver)"/>
	public virtual FieldBuilder<TSourceType, TReturnType> Resolve(Func<IResolveFieldContext<TSourceType>, TReturnType?> resolve)
		=> Resolve(new CustomFieldResolver(context => resolve((IResolveFieldContext<TSourceType>)context)));

	/// <summary>
	/// Adds an argument to the field.
	/// </summary>
	/// <typeparam name="TArgumentGraphType">The graph type of the argument.</typeparam>
	/// <param name="name">The name of the argument.</param>
	/// <param name="configure">A delegate to further configure the argument.</param>
	public virtual FieldBuilder<TSourceType, TReturnType> Argument<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TArgumentGraphType>(string name, Action<QueryArgument>? configure = null)
		where TArgumentGraphType : IGraphType
		=> Argument(typeof(TArgumentGraphType), name, configure);

	/// <summary>
	/// Adds an argument to the field.
	/// </summary>
	/// <param name="type">The graph type of the argument.</param>
	/// <param name="name">The name of the argument.</param>
	/// <param name="configure">A delegate to further configure the argument.</param>
	public virtual FieldBuilder<TSourceType, TReturnType> Argument([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type, string name, Action<QueryArgument>? configure = null)
	{
		var argument = new QueryArgument(name, type);
		configure?.Invoke(argument);
		FieldType.Arguments = [.. FieldType.Arguments, argument];
		return this;
	}
}
