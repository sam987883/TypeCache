// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;

namespace TypeCache.GraphQL.Types;

public class GraphQLInterfaceType : GraphType, IInterfaceGraphType
{
	public GraphQLInterfaceType()
	{
	}

	protected GraphQLInterfaceType(RuntimeTypeHandle typeHandle)
	{
		this.TypeHandle = typeHandle;
	}

	public RuntimeTypeHandle TypeHandle { get; }

	public ISet<FieldType> Fields { get; } = new HashSet<FieldType>();

	/// <inheritdoc/>
	public ISet<IObjectGraphType> PossibleTypes { get; } = new HashSet<IObjectGraphType>();

	/// <inheritdoc/>
	public Func<object, IObjectGraphType?>? ResolveType { get; set; }

	/// <inheritdoc/>
	public void AddPossibleType(IObjectGraphType type)
	{
		type.ThrowIfNull();

		this.IsValidInterfaceFor(type, throwError: true);
		this.PossibleTypes.Add(type);
	}
}

public class GraphQLInterfaceType<T> : GraphQLInterfaceType
{
	public GraphQLInterfaceType()
		: base(typeof(T).TypeHandle)
	{
		typeof(T).IsInterface.ThrowIfFalse();

		this.Name = typeof(T).GraphQLName();
		this.Description = typeof(T).GraphQLDescription();
		this.DeprecationReason = typeof(T).GraphQLDeprecationReason();

		typeof(T).GetPublicProperties()
			.Where(propertyInfo => propertyInfo.CanRead && !propertyInfo.GraphQLIgnore())
			.ToArray()
			.ForEach(propertyInfo => this.Fields.Add(new(propertyInfo)));
	}
}
