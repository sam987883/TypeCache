// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLObjectType<T> : GraphQLComplexType, IObjectGraphType
{
	public GraphQLObjectType()
		: base(TypeOf<T>.Member.GraphQLName())
	{
		this.Interfaces = new();
		this.ResolvedInterfaces = new();

		this.Description = TypeOf<T>.Member.GraphQLDescription();
		this.DeprecationReason = TypeOf<T>.Member.GraphQLDeprecationReason();

		TypeOf<T>.Properties.Values
			.If(property => property.Getter is not null && !property.GraphQLIgnore())
			.Do(property => this.AddField(property.ToFieldType()));

		TypeOf<T>.InterfaceTypes
			.If(type => type.ElementType is null && !type.GenericHandle.HasValue)
			.Do(type => this.Interfaces.Add(type));
	}

	public Func<object, bool>? IsTypeOf { get; set; }

	public Interfaces Interfaces { get; }

	public ResolvedInterfaces ResolvedInterfaces { get; }

	public void AddResolvedInterface(IInterfaceGraphType graphType)
	{
		graphType.AssertNotNull();
		graphType.IsValidInterfaceFor(this);

		// this.ResolvedInterfaces.Add(graphType);
		TypeOf<ResolvedInterfaces>.InvokeMethod("Add", this.ResolvedInterfaces, graphType);
	}
}
