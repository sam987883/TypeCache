﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
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
			method.AssertNotNull(nameof(method));
			handler.AssertNotNull(nameof(handler));
			dataLoader.AssertNotNull(nameof(dataLoader));

			if (!method.Return.Type.Is<T>())
				throw new ArgumentException($"{nameof(ItemLoaderFieldResolver<T>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<T>.Name}] instead of [{method.Return.Type.Name}].");

			this._Method = method;
			this._Handler = handler;
			this._DataLoader = dataLoader;
		}

		public IDataLoaderResult<T> Resolve(IResolveFieldContext context)
		{
			context.Source.AssertNotNull($"{nameof(context)}.{nameof(context.Source)}");

			var child = this._Method.Attributes.GraphName() ?? this._Method.Name.TrimStart("Get")!.TrimEnd("Async");
			var parent = context.Source.GetTypeMember().Attributes.GraphName();
			var dataLoader = this._DataLoader!.Context.GetOrAddLoader<T>(
				$"{parent}.{child}",
				() =>
				{
					var arguments = this.GetArguments(context).ToArray();
					var result = this._Method.Invoke(this._Handler, arguments);
					return result switch
					{
						ValueTask<T> valueTask => valueTask.AsTask(),
						Task<T> task => task,
						_ => Task.FromResult((T)result!)!
					};
				});

			return dataLoader.LoadAsync();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		object IFieldResolver.Resolve(IResolveFieldContext context)
			=> this.Resolve(context);

		private IEnumerable<object?> GetArguments(IResolveFieldContext context)
		{
			foreach (var parameter in this._Method.Parameters)
			{
				var graphAttribute = parameter.Attributes.GraphName() ?? parameter.Name;
				if (parameter.Attributes.GraphIgnore())
					continue;

				if (parameter.Type.Is<IResolveFieldContext>() || parameter.Type.Is(typeof(IResolveFieldContext<>).MakeGenericType(context.Source.GetType())))
					yield return context;
				else if (parameter.Type.Is<T>())
					yield return context.Source;
				else if (parameter.Type.SystemType == SystemType.Unknown)
				{
					var argument = context.GetArgument<IDictionary<string, object?>>(parameter.Name);
					if (argument is not null)
					{
						var model = parameter.Type.Create();
						model.ReadProperties(argument);
						yield return model;
					}
					yield return null;
				}
				else
					yield return context.GetArgument(parameter.Type, parameter.Name); // TODO: Support a default value?
			}
		}
	}
}
