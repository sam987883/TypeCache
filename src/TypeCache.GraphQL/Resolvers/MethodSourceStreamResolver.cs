// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodSourceStreamResolver : SourceStreamResolver
{
	private readonly MethodInfo _MethodInfo;

	public MethodSourceStreamResolver(MethodInfo methodInfo)
	{
		var returnsObservable = methodInfo.ReturnType.Is(typeof(IObservable<>)) || methodInfo.ReturnType.Implements(typeof(IObservable<>));
		if (!returnsObservable)
		{
			var returnType = methodInfo.ReturnType.GetObjectType();
			if (returnType is ObjectType.Task || returnType is ObjectType.ValueTask)
				returnsObservable = methodInfo.ReturnType.GenericTypeArguments.Single().GetObjectType() is ObjectType.Observable;

			returnsObservable.ThrowIfFalse();
		}

		this._MethodInfo = methodInfo;
	}

	protected override ValueTask<IObservable<object?>> ResolveAsync(global::GraphQL.IResolveFieldContext context)
	{
		context.RequestServices.ThrowIfNull();

		var arguments = context.GetArguments(this._MethodInfo).ToArray();
		object? result;
		if (!this._MethodInfo.IsStatic)
		{
			var controller = context.RequestServices.GetRequiredService(this._MethodInfo.DeclaringType!);
			result = this._MethodInfo.InvokeFunc(controller, arguments);
		}
		else
			result = this._MethodInfo.InvokeStaticFunc(arguments);

		return result switch
		{
			ValueTask<IObservable<object?>> valueTask => valueTask,
			Task<IObservable<object?>> task => new(task),
			IObservable<object?> item => ValueTask.FromResult(item),
			_ => throw new UnreachableException(Invariant($"Method {this._MethodInfo.Name()} returned a null for {this._MethodInfo.ReturnType.Name()}."))
		};
	}
}
