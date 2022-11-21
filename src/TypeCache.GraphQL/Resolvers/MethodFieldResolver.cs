// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodFieldResolver : IFieldResolver
{
	private readonly object? _Controller;
	private readonly MethodMember _Method;

	public MethodFieldResolver(MethodMember method, object? controller)
	{
		if (!method.Static)
		{
			controller.AssertNotNull();
			this._Controller = controller;
		}
		else
			this._Controller = null;

		this._Method = method;
	}

	public async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
		=> await Task.FromResult(this._Method.Invoke(this._Controller, context.GetArguments(this._Controller?.GetType(), this._Method).ToArray()));
}
