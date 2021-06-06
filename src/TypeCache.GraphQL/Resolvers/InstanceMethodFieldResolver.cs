﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Resolvers
{
	public class InstanceMethodFieldResolver : IFieldResolver
	{
		private readonly InstanceMethodMember _Method;
		private readonly object _Handler;

		public InstanceMethodFieldResolver(InstanceMethodMember method, object handler)
		{
			method.AssertNotNull(nameof(method));
			handler.AssertNotNull(nameof(handler));

			if (method.Return.IsVoid)
				throw new NotSupportedException($"{nameof(InstanceMethodFieldResolver)}: Graph endpoints cannot have a return type that is void, Task or ValueTask.");

			this._Method = method;
			this._Handler = handler;
		}

		public object? Resolve(IResolveFieldContext context)
		{
			var arguments = this.GetArguments(context).ToArray();
			return this._Method.Invoke!(this._Handler, arguments);
		}

		private IEnumerable<object> GetArguments(IResolveFieldContext context)
		{
			foreach (var parameter in this._Method.Parameters)
			{
				var graphAttribute = parameter.Attributes.First<GraphNameAttribute>();
				if (parameter.Attributes.Any<GraphIgnoreAttribute>())
					continue;

				if (parameter.Type.Is<IResolveFieldContext>() || parameter.Type.Is(typeof(IResolveFieldContext<>)))
					yield return context;
				else if (parameter.Type.SystemType == SystemType.Unknown)
				{
					var argument = context.GetArgument<IDictionary<string, object?>>(parameter.Name);
					var model = parameter.Type.Create();
					if (argument is not null)
						model.MapProperties(argument);
					yield return model;
				}
				else
					yield return context.GetArgument(parameter.Type, parameter.Name); // TODO: Support a default value?
			}
		}
	}
}
