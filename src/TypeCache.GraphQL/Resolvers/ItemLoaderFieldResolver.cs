// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using static System.FormattableString;

namespace TypeCache.GraphQL.Resolvers;

public sealed class ItemLoaderFieldResolver<T> : IFieldResolver
{
	private readonly MethodMember _Method;
	private readonly object? _Controller;
	private readonly IDataLoaderContextAccessor _DataLoader;

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public ItemLoaderFieldResolver(
		MethodMember method,
		object? controller,
		IDataLoaderContextAccessor dataLoader)
	{
		dataLoader.AssertNotNull();

		if (!method.Static)
			controller.AssertNotNull();

		if (!method.Return.Type.Is<T>() && !method.Return.Type.Is<Task<T>>() && !method.Return.Type.Is<ValueTask<T>>())
			throw new ArgumentException($"{nameof(ItemLoaderFieldResolver<T>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<T>.Member.GraphQLName()}] instead of [{method.Return.Type.Name}].");

		this._Method = method;
		this._Controller = !method.Static ? controller : null;
		this._DataLoader = dataLoader;
	}

	public async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.Source.AssertNotNull();

		var loaderKey = Invariant($"{context.Source!.GetTypeMember()!.GraphQLName()}.{this._Method.GraphQLName()}");
		var dataLoader = this._DataLoader.Context!.GetOrAddLoader<T>(loaderKey, async () =>
		{
			var arguments = context.GetArguments<T>(this._Method).ToArray();
			var result = this._Method.Invoke(this._Controller, arguments);
			return result switch
			{
				ValueTask<T> valueTask => await valueTask,
				Task<T> task => await task,
				T item => item,
				_ => default!
			};
		});

		return await dataLoader.LoadAsync().GetResultAsync(context.CancellationToken);
	}
}
