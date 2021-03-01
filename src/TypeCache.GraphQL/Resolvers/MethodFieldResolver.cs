// Copyright (c) 2021 Samuel Abraham

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
	public class MethodFieldResolver<T> : IFieldResolver<Task<T>>
	{
		private readonly MethodMember _Method;
		private readonly object _Handler;

		public MethodFieldResolver(MethodMember method, object handler)
		{
			handler.AssertNotNull(nameof(handler));

			this._Method = method;
			this._Handler = handler;
		}

		public Task<T> Resolve(IResolveFieldContext context)
		{
			var arguments = this.GetArguments(context).ToArray();
			var value = this._Method.Invoke!(this._Handler, arguments);
			return this._Method.Return.IsValueTask
				? ((ValueTask<T>)value!).AsTask()
				: (Task<T>)value!;
		}

		object? IFieldResolver.Resolve(IResolveFieldContext context)
		{
			var arguments = this.GetArguments(context).ToArray();
			return this._Method.Invoke!(this._Handler, arguments);
		}

		private IEnumerable<object> GetArguments(IResolveFieldContext context)
		{
			foreach (var parameter in this._Method.Parameters)
			{
				var graphAttribute = parameter.Attributes.First<GraphAttribute>();
				if (graphAttribute?.Ignore == true)
					continue;

				var parameterType = parameter.Type.Handle.ToType();
				if (parameterType.Is<IResolveFieldContext>() || parameterType.Is(typeof(IResolveFieldContext<>)))
					yield return context;
				else if (parameter.Type.SystemType == SystemType.Unknown)
				{
					var argument = context.GetArgument<IDictionary<string, object?>>(parameter.Name);
					var model = parameterType.Create(parameterType);
					if (argument != null)
						model.MapProperties(argument);
					yield return model;
				}
				else
					yield return context.GetArgument(parameterType, parameter.Name); // TODO: Support a default value?
			}
		}
	}

	public class MethodFieldResolver : IFieldResolver
	{
		private readonly MethodMember _Method;
		private readonly object _Handler;

		public MethodFieldResolver(MethodMember method, object handler)
		{
			handler.AssertNotNull(nameof(handler));

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
				var graphAttribute = parameter.Attributes.First<GraphAttribute>();
				if (graphAttribute?.Ignore == true)
					continue;

				var parameterType = parameter.Type.Handle.ToType();
				if (parameterType.Is<IResolveFieldContext>() || parameterType.Is(typeof(IResolveFieldContext<>)))
					yield return context;
				else if (parameter.Type.SystemType == SystemType.Unknown)
				{
					var argument = context.GetArgument<IDictionary<string, object?>>(parameter.Name);
					var model = parameterType.Create();
					if (argument != null)
						model.MapProperties(argument);
					yield return model;
				}
				else
					yield return context.GetArgument(parameterType, parameter.Name); // TODO: Support a default value?
			}
		}
	}
}
