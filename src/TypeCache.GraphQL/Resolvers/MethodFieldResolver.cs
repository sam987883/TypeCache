// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodFieldResolver : FieldResolver
{
	private readonly MethodInfo _MethodInfo;

	public MethodFieldResolver(MethodInfo methodInfo)
	{
		this._MethodInfo = methodInfo;
	}

	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();

		var arguments = context.GetArguments(this._MethodInfo).ToArray();
		if (!this._MethodInfo.IsStatic)
		{
			var controller = context.RequestServices.GetRequiredService(this._MethodInfo.DeclaringType!);
			arguments = arguments.Prepend(controller).ToArray();
		}

		var result = this._MethodInfo.InvokeMethod(arguments);
		return result switch
		{
			ValueTask<object?> valueTask => await valueTask,
			Task<object?> task => await task,
			_ => result
		};
	}
}
