// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Types;

public sealed class InterfaceGraphType<T> : global::GraphQL.Types.InterfaceGraphType<T>
	where T : class
{
	public InterfaceGraphType()
	{
		TypeCache.Reflection.Type<T>.ClrType.ThrowIfNotEqual(ClrType.Interface);

		this.Name = TypeCache.Reflection.Type<T>.Attributes.GraphQLName() ?? TypeCache.Reflection.Type<T>.Name;

		TypeCache.Reflection.Type<T>.Properties.Values
			.Where(_ => _.CanRead && !_.Attributes.GraphQLIgnore())
			.ForEach(_ => this.AddField(_.ToFieldType()));
	}
}
