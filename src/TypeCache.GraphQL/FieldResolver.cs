// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Extensions;
using System;
using System.Runtime.CompilerServices;

namespace TypeCache.GraphQL
{
	public class FieldResolver : IFieldResolver
	{
		private readonly Func<IResolveFieldContext, object> _Resolve;

		public FieldResolver(Func<IResolveFieldContext, object> resolve)
		{
			resolve.AssertNotNull(nameof(resolve));
			this._Resolve = resolve;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object Resolve(IResolveFieldContext context)
			=> this._Resolve(context);
	}
}
