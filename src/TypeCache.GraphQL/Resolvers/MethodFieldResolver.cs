// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Resolvers;

public sealed class MethodFieldResolver : FieldResolver
{
	private readonly MethodInfo _MethodInfo;

	public MethodFieldResolver(MethodInfo methodInfo)
	{
		this._MethodInfo = methodInfo;
	}

	protected override ValueTask<object?> ResolveAsync(global::GraphQL.IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();

		var sourceType = !this._MethodInfo.IsStatic ? this._MethodInfo.DeclaringType : null;
		var controller = sourceType is not null ? context.RequestServices.GetRequiredService(sourceType) : null;
		var arguments = context.GetArguments(sourceType, this._MethodInfo).ToArray();
		var result = this._MethodInfo.InvokeMethod(controller, arguments);

		return result switch
		{
			ValueTask<object?> valueTask => valueTask,
			Task<object?> task => new ValueTask<object?>(task),
			_ => ValueTask.FromResult(result)
		};
	}
}
