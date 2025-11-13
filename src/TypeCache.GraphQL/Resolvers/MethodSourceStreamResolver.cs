// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodSourceStreamResolver(MethodEntity method) : SourceStreamResolver
{
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	protected override ValueTask<IObservable<object?>> ResolveAsync(global::GraphQL.IResolveFieldContext context)
	{
		context.RequestServices.ThrowIfNull();
		method.ThrowIfNull();

		var returnType = method.Return.ParameterType.ObjectType;
		var returnsObservable = returnType is ObjectType.Observable;
		if (!returnsObservable && (returnType is ObjectType.Task || returnType is ObjectType.ValueTask))
			returnsObservable = method.Return.ParameterType.GenericTypeArguments.Single().ObjectType is ObjectType.Observable;

		returnsObservable.ThrowIfFalse();

		var arguments = context.GetArguments(method.Parameters).ToArray();
		var controller = context.RequestServices.GetRequiredService(method.Type);
		var result = method.Invoke(controller, arguments);

		return result switch
		{
			ValueTask<IObservable<object?>> valueTask => valueTask,
			Task<IObservable<object?>> task => new(task),
			IObservable<object?> item => ValueTask.FromResult(item),
			_ => throw new UnreachableException(Invariant($"Method {method.Name} returned a null for {method.Return.ParameterType.Name}."))
		};
	}
}
