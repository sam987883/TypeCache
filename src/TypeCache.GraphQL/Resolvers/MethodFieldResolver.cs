// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Resolvers
{
	public class MethodFieldResolver : IFieldResolver
	{
		private readonly MethodMember _Method;
		private readonly object? _Handler;

		public MethodFieldResolver(MethodMember method, object? handler)
		{
			if (!method.Static)
				handler.AssertNotNull(nameof(handler));

			if (method.Return.IsVoid)
				throw new NotSupportedException($"{nameof(MethodFieldResolver)}: Graph endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

			this._Method = method;
			this._Handler = handler;
		}

		public object? Resolve(IResolveFieldContext context)
		{
			var arguments = context.GetArguments<object>(this._Method).ToArray();
			return this._Method.Invoke(this._Handler, arguments);
		}
	}
}
