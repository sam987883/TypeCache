// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.Resolvers;
using System;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL
{
	public class MethodFieldResolver : IFieldResolver
	{
		private readonly IServiceProvider _Provider;

		public MethodFieldResolver(IMethodMember method, IServiceProvider provider)
		{
			method.AssertNotNull(nameof(method));
			provider.AssertNotNull(nameof(provider));

			this._Provider = provider;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object Resolve(IResolveFieldContext context)
		{

			return null;
		}
	}
}
