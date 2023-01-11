// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.Resolvers;

public sealed class ItemLoaderFieldResolver<T> : FieldResolver
{
	private readonly MethodInfo _MethodInfo;

	/// <exception cref="ArgumentException"/>
	public ItemLoaderFieldResolver(MethodInfo methodInfo)
	{
		methodInfo.ReturnType.IsAny<T, Task<T>, ValueTask<T>>().AssertTrue();
		this._MethodInfo = methodInfo;
	}

	/// <exception cref="ArgumentNullException"/>
	protected override async ValueTask<object?> ResolveAsync(global::GraphQL.IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();
		context.Source.AssertNotNull();

		var dataLoaderAccessor = context.RequestServices.GetRequiredService<IDataLoaderContextAccessor>();
		var dataLoaderContext = dataLoaderAccessor.Context;
		dataLoaderContext.AssertNotNull();

		var loaderKey = Invariant($"{context.Source.GetType().GraphQLName()}.{this._MethodInfo.GraphQLName()}");
		var dataLoader = dataLoaderContext.GetOrAddLoader<T>(loaderKey, () =>
		{
			var arguments = context.GetArguments<T>(this._MethodInfo).ToArray();
			var sourceType = !this._MethodInfo.IsStatic ? this._MethodInfo.DeclaringType : null;
			var controller = sourceType is not null ? context.RequestServices.GetRequiredService(sourceType) : null;
			var result = this._MethodInfo.Invoke(controller, arguments);
			return result switch
			{
				ValueTask<T> valueTask => valueTask.AsTask(),
				Task<T> task => task,
				T item => Task.FromResult(item),
				_ => Task.FromResult(default(T))!
			};
		});

		return await dataLoader.LoadAsync().GetResultAsync(context.CancellationToken);
	}
}
