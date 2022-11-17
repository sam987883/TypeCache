// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodSourceStreamResolver : ISourceStreamResolver
{
	private readonly object? _Controller;
	private readonly MethodMember _Method;

	public MethodSourceStreamResolver(MethodMember method, object? controller)
	{
		var isObservable = method.Return.Type.Is(typeof(IObservable<>)) || method.Return.Type.Implements(typeof(IObservable<>));
		isObservable.AssertEquals(true);

		if (!method.Static)
		{
			controller.AssertNotNull();
			this._Controller = controller;
		}
		else
			this._Controller = null;

		this._Method = method;
	}

	public async ValueTask<IObservable<object?>> ResolveAsync(IResolveFieldContext context)
		=> await Task.FromResult((IObservable<object?>)this._Method.Invoke(this._Controller, context.GetArguments<object>(this._Method).ToArray())!);
}
