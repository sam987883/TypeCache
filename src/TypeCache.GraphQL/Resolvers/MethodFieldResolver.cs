// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodFieldResolver(MethodInfo methodInfo) : FieldResolver
{
	protected override Task<object?> ResolveAsync(IResolveFieldContext context)
	{
		var arguments = context.GetArguments(methodInfo).ToArray();
		object? result;
		if (!methodInfo.IsStatic)
		{
			context.RequestServices.ThrowIfNull();

			var source = context.RequestServices.GetRequiredService(methodInfo.DeclaringType!);
			result = methodInfo.InvokeFunc(source, arguments);
		}
		else
			result = methodInfo.InvokeStaticFunc(arguments);

		return result switch
		{
			ValueTask<object?> valueTask => valueTask.AsTask(),
			Task<object?> task => task,
			_ => Task.FromResult(result)
		};
	}
}
