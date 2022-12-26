// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using static System.FormattableString;

namespace TypeCache.GraphQL.Resolvers;

public sealed class ItemLoaderFieldResolver<T> : FieldResolver
{
	private readonly MethodMember _Method;

	/// <exception cref="ArgumentException"/>
	public ItemLoaderFieldResolver(MethodMember method)
	{
		if (!method.Return.Type.Is<T>() && !method.Return.Type.Is<Task<T>>() && !method.Return.Type.Is<ValueTask<T>>())
			throw new ArgumentException($"{nameof(ItemLoaderFieldResolver<T>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<T>.Member.GraphQLName()}] instead of [{method.Return.Type.Name}].");

		this._Method = method;
	}

	/// <exception cref="ArgumentNullException"/>
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();
		context.Source.AssertNotNull();

		var dataLoaderAccessor = context.RequestServices.GetRequiredService<IDataLoaderContextAccessor>();
		var dataLoaderContext = dataLoaderAccessor.Context;
		dataLoaderContext.AssertNotNull();

		var loaderKey = Invariant($"{context.Source.GetTypeMember()!.GraphQLName()}.{this._Method.GraphQLName()}");
		var dataLoader = dataLoaderContext.GetOrAddLoader<T>(loaderKey, () =>
		{
			var arguments = context.GetArguments<T>(this._Method).ToArray();
			var controller = !this._Method.Static ? context.RequestServices.GetRequiredService(this._Method.Type) : null;
			var result = this._Method.Invoke(controller, arguments);
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
