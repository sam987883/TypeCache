// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Resolvers;

public class MethodFieldResolver : IFieldResolver
{
	private readonly object? _Controller;
	private readonly MethodMember _MethodMember;

	public MethodFieldResolver(object? controller, MethodMember methodMember)
	{
		this._Controller = controller;
		this._MethodMember = methodMember;
	}

	public async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		await Task.CompletedTask;
		return this._MethodMember.Invoke(this._Controller, context.GetArguments(this._Controller?.GetType(), this._MethodMember).ToArray());
	}
}
