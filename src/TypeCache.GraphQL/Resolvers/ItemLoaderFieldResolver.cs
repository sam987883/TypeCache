// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Resolvers;

public class ItemLoaderFieldResolver<T> : IFieldResolver<IDataLoaderResult<T>>
{
	private readonly MethodMember _Method;
	private readonly object? _Handler;
	private readonly IDataLoaderContextAccessor _DataLoader;

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public ItemLoaderFieldResolver(MethodMember method, object? handler, IDataLoaderContextAccessor dataLoader)
	{
		dataLoader.AssertNotNull();

		if (!method.Static)
			handler.AssertNotNull();

		dataLoader.AssertNotNull();

		if (!method.Return.Type.Is<T>() && !method.Return.Type.Is<Task<T>>() && !method.Return.Type.Is<ValueTask<T>>())
			throw new ArgumentException($"{nameof(ItemLoaderFieldResolver<T>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<T>.Member.GraphName()}] instead of [{method.Return.Type.Name}].");

		this._Method = method;
		this._Handler = handler;
		this._DataLoader = dataLoader;
	}

	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	object IFieldResolver.Resolve(IResolveFieldContext context)
		=> this.Resolve(context);

	/// <exception cref="ArgumentNullException"/>
	public IDataLoaderResult<T> Resolve(IResolveFieldContext context)
	{
		context.Source.AssertNotNull();

		var dataLoader = this._DataLoader!.Context.GetOrAddLoader<T>(
			$"{context.Source!.GetTypeMember().GraphName()}.{this._Method.GraphName()}",
			async () =>
			{
				var arguments = context.GetArguments<T>(this._Method).ToArray();
				var result = this._Method.Invoke(this._Handler, arguments);
				return result switch
				{
					ValueTask<T> valueTask => await valueTask,
					Task<T> task => await task,
					_ => await Task.FromResult((T)result!)
				};
			});

		return dataLoader.LoadAsync();
	}
}
