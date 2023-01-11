// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using static System.FormattableString;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodSourceStreamResolver : SourceStreamResolver
{
	private readonly MethodInfo _MethodInfo;

	public MethodSourceStreamResolver(MethodInfo methodInfo)
	{
		var returnsObservable = methodInfo.ReturnType.GetObjectType() == ObjectType.Observable;
		if (!returnsObservable && methodInfo.ReturnType.GetSystemType().IsAny(SystemType.ValueTask, SystemType.Task))
			returnsObservable = methodInfo.ReturnType.GenericTypeArguments.Single().GetObjectType() == ObjectType.Observable;

		returnsObservable.AssertTrue();

 		this._MethodInfo = methodInfo;
	}

	protected override ValueTask<IObservable<object?>> ResolveAsync(global::GraphQL.IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();

		var sourceType = !this._MethodInfo.IsStatic ? this._MethodInfo.DeclaringType : null;
		var controller = sourceType is not null ? context.RequestServices.GetRequiredService(sourceType) : null;
		var arguments = context.GetArguments<object>(this._MethodInfo).ToArray();
		var result = this._MethodInfo.Invoke(controller, arguments);

		return result switch
		{
			ValueTask<IObservable<object?>> valueTask => valueTask,
			Task<IObservable<object?>> task => new ValueTask<IObservable<object?>>(task),
			IObservable<object?> item => ValueTask.FromResult(item),
			_ => throw new UnreachableException(Invariant($"Method {this._MethodInfo.Name()} returned a null for {this._MethodInfo.ReturnType.Name()}."))
		};
	}
}
