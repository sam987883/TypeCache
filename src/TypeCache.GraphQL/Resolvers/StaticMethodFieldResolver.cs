// Copyright (c) 2021 Samuel Abraham

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
	public class StaticMethodFieldResolver : IFieldResolver
	{
		private readonly StaticMethodMember _Method;

		public StaticMethodFieldResolver(StaticMethodMember method)
		{
			method.AssertNotNull(nameof(method));

			if (method.Return.IsVoid)
				throw new NotSupportedException($"{nameof(StaticMethodFieldResolver)}: Graph endpoints cannot have a return type that is void, Task or ValueTask.");

			this._Method = method;
		}

		public object? Resolve(IResolveFieldContext context)
		{
			var arguments = this.GetArguments(context).ToArray();
			return this._Method.Invoke!(arguments);
		}

		private IEnumerable<object> GetArguments(IResolveFieldContext context)
		{
			foreach (var parameter in this._Method.Parameters)
			{
				var graphAttribute = parameter.Attributes.First<GraphAttribute>();
				if (parameter.Attributes.Any<GraphIgnoreAttribute>())
					continue;

				var parameterType = parameter.Type.Handle.ToType();
				if (parameterType.Is<IResolveFieldContext>() || parameterType.Is(typeof(IResolveFieldContext<>)))
					yield return context;
				else if (parameter.Type.SystemType == SystemType.Unknown)
				{
					var argument = context.GetArgument<IDictionary<string, object?>>(parameter.Name);
					var model = parameterType.Create();
					if (argument is not null)
						model.MapProperties(argument);
					yield return model;
				}
				else
					yield return context.GetArgument(parameterType, parameter.Name); // TODO: Support a default value?
			}
		}
	}
}
