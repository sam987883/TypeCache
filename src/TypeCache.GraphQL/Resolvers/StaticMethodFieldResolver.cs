// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class StaticMethodFieldResolver(StaticMethodEntity method) : FieldResolver
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.RequestServices.ThrowIfNull();

		var arguments = context.GetArguments(method.Parameters).ToArray();
		object? result;
		result = method.Invoke(arguments);

		return result switch
		{
			ValueTask<object?> valueTask => await valueTask,
			Task<object?> task => await task,
			_ => result
		};
	}
}
