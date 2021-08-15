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

namespace TypeCache.GraphQL.Resolvers
{
	public class ItemLoaderFieldResolver<T> : IFieldResolver<IDataLoaderResult<T>>
	{
		private readonly MethodMember _Method;
		private readonly object _Handler;
		private readonly IDataLoaderContextAccessor _DataLoader;

		public ItemLoaderFieldResolver(MethodMember method, object handler, IDataLoaderContextAccessor dataLoader)
		{
			handler.AssertNotNull(nameof(handler));
			dataLoader.AssertNotNull(nameof(dataLoader));

			if (!method.Return.Type.Is<T>())
				throw new ArgumentException($"{nameof(ItemLoaderFieldResolver<T>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<T>.Attributes.GraphName() ?? TypeOf<T>.Name}] instead of [{method.Return.Type.Name}].");

			this._Method = method;
			this._Handler = handler;
			this._DataLoader = dataLoader;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		object IFieldResolver.Resolve(IResolveFieldContext context)
			=> this.Resolve(context);

		public IDataLoaderResult<T> Resolve(IResolveFieldContext context)
		{
			context.Source.AssertNotNull($"{nameof(context)}.{nameof(context.Source)}");

			var child = this._Method.Attributes.GraphName() ?? this._Method.Name.TrimStart("Get")!.TrimEnd("Async");
			var parent = context.Source.GetTypeMember().Attributes.GraphName();
			var dataLoader = this._DataLoader!.Context.GetOrAddLoader<T>(
				$"{parent}.{child}",
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
}
