// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodFieldResolver(MethodInfo methodInfo) : FieldResolver
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();

		var arguments = context.GetArguments(methodInfo).ToArray();
		if (!methodInfo.IsStatic)
		{
			var controller = context.RequestServices.GetRequiredService(methodInfo.DeclaringType!);
			arguments = [controller, ..arguments];
		}

		var result = methodInfo.InvokeMethod(arguments);
		return result switch
		{
			ValueTask<object?> valueTask => await valueTask,
			Task<object?> task => await task,
			_ => result
		};
	}
}
