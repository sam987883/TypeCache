// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using static System.FormattableString;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodSourceStreamResolver : ISourceStreamResolver
{
	private readonly MethodMember _Method;

	public MethodSourceStreamResolver(MethodMember method)
	{
		var returnsObservable = method.Return.Type.IsOrImplements(typeof(IObservable<>));
		if (!returnsObservable && method.Return.Type.SystemType.IsAny(SystemType.ValueTask, SystemType.Task))
			returnsObservable = method.Return.Type.GenericTypes.Single().ObjectType == ObjectType.Observable;

		returnsObservable.AssertTrue();

 		this._Method = method;
	}

	public ValueTask<IObservable<object?>> ResolveAsync(IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();

		var controller = !this._Method.Static ? context.RequestServices.GetRequiredService(this._Method.Type) : null;
		var arguments = context.GetArguments<object>(this._Method).ToArray();
		var result = this._Method.Invoke(controller, arguments);

		return result switch
		{
			ValueTask<IObservable<object?>> valueTask => valueTask,
			Task<IObservable<object?>> task => new ValueTask<IObservable<object?>>(task),
			IObservable<object?> item => ValueTask.FromResult(item),
			_ => throw new UnreachableException(Invariant($"Method {this._Method.Name} returned a null for {this._Method.Return.Type.Name}."))
		};
	}
}
