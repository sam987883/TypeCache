// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodFieldResolver : IFieldResolver
{
	private readonly MethodMember _Method;

	public MethodFieldResolver(MethodMember method)
	{
		this._Method = method;
	}

	public ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();

		var controller = !this._Method.Static ? context.RequestServices.GetRequiredService(this._Method.Type) : null;
		var sourceType = !this._Method.Static ? (Type)this._Method.Type : null;
		var arguments = context.GetArguments(sourceType, this._Method).ToArray();
		var result = this._Method.Invoke(controller, arguments);

		return result switch
		{
			ValueTask<object?> valueTask => valueTask,
			Task<object?> task => new ValueTask<object?>(task),
			_ => ValueTask.FromResult(result)
		};
	}
}
