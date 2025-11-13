// Copyright (c) 2021 Samuel Abraham

using System.Data;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static partial class SchemaExtensions
{
	extension(ISchema @this)
	{
		/// <remarks>
		/// <c>=&gt; @this.Mutation ??= <see langword="new"/> <see cref="ObjectGraphType"/> { Name = <see langword="nameof"/>(<see cref="ISchema.Mutation"/>) };</c>
		/// </remarks>
		public IObjectGraphType GetMutation()
			=> @this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };


		/// <summary>
		/// <b>GraphQL Minimal API.</b>
		/// </summary>
		public FieldType AddMutation<R>(string name, Func<R> handler)
			=> @this.GetMutation().AddField(new()
			{
				Name = name,
				Resolver = new FuncFieldResolver<R>(context => handler()),
				Type = typeof(R).ToGraphQLType(false)
			});

		/// <summary>
		/// <b>GraphQL Minimal API.</b>
		/// </summary>
		public FieldType AddMutation<T, R>(string name, string argument, Func<T, R> handler)
			=> @this.GetMutation().AddField(new()
			{
				Arguments = new(new QueryArgument(typeof(T).ToGraphQLType(true).ToNonNullGraphType()) { Name = argument }),
				Name = name,
				Resolver = new FuncFieldResolver<R>(context => handler(context.GetArgument<T>(argument))),
				Type = typeof(R).ToGraphQLType(false)
			});

		/// <summary>
		/// <b>GraphQL Minimal API.</b>
		/// </summary>
		public FieldType AddMutation<T1, T2, R>(string name, (string, string) arguments, Func<T1, T2, R> handler)
			=> @this.GetMutation().AddField(new()
			{
				Arguments = new(
					new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
					new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 }),
				Name = name,
				Resolver = new FuncFieldResolver<R>(context => handler(
					context.GetArgument<T1>(arguments.Item1),
					context.GetArgument<T2>(arguments.Item2))),
				Type = typeof(R).ToGraphQLType(false)
			});

		/// <summary>
		/// <b>GraphQL Minimal API.</b>
		/// </summary>
		public FieldType AddMutation<T1, T2, T3, R>(string name, (string, string, string) arguments, Func<T1, T2, T3, R> handler)
			=> @this.GetMutation().AddField(new()
			{
				Arguments = new(
					new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
					new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
					new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 }),
				Name = name,
				Resolver = new FuncFieldResolver<R>(context => handler(
					context.GetArgument<T1>(arguments.Item1),
					context.GetArgument<T2>(arguments.Item2),
					context.GetArgument<T3>(arguments.Item3))),
				Type = typeof(R).ToGraphQLType(false)
			});

		/// <summary>
		/// <b>GraphQL Minimal API.</b>
		/// </summary>
		public FieldType AddMutation<T1, T2, T3, T4, R>(string name, (string, string, string, string) arguments, Func<T1, T2, T3, T4, R> handler)
			=> @this.GetMutation().AddField(new()
			{
				Arguments = new(
					new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
					new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
					new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 },
					new QueryArgument(typeof(T4).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item4 }),
				Name = name,
				Resolver = new FuncFieldResolver<R>(context => handler(
					context.GetArgument<T1>(arguments.Item1),
					context.GetArgument<T2>(arguments.Item2),
					context.GetArgument<T3>(arguments.Item3),
					context.GetArgument<T4>(arguments.Item4))),
				Type = typeof(R).ToGraphQLType(false)
			});

		/// <summary>
		/// <b>GraphQL Minimal API.</b>
		/// </summary>
		public FieldType AddMutation<T1, T2, T3, T4, T5, R>(string name, (string, string, string, string, string) arguments, Func<T1, T2, T3, T4, T5, R> handler)
			=> @this.GetMutation().AddField(new()
			{
				Arguments = new(
					new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
					new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
					new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 },
					new QueryArgument(typeof(T4).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item4 },
					new QueryArgument(typeof(T5).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item5 }),
				Name = name,
				Resolver = new FuncFieldResolver<R>(context => handler(
					context.GetArgument<T1>(arguments.Item1),
					context.GetArgument<T2>(arguments.Item2),
					context.GetArgument<T3>(arguments.Item3),
					context.GetArgument<T4>(arguments.Item4),
					context.GetArgument<T5>(arguments.Item5))),
				Type = typeof(R).ToGraphQLType(false)
			});

		/// <summary>
		/// <b>GraphQL Minimal API.</b>
		/// </summary>
		public FieldType AddMutation<T1, T2, T3, T4, T5, T6, R>(string name, (string, string, string, string, string, string) arguments, Func<T1, T2, T3, T4, T5, T6, R> handler)
			=> @this.GetMutation().AddField(new()
			{
				Arguments = new(
					new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
					new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
					new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 },
					new QueryArgument(typeof(T4).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item4 },
					new QueryArgument(typeof(T5).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item5 },
					new QueryArgument(typeof(T6).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item6 }),
				Name = name,
				Resolver = new FuncFieldResolver<R>(context => handler(
					context.GetArgument<T1>(arguments.Item1),
					context.GetArgument<T2>(arguments.Item2),
					context.GetArgument<T3>(arguments.Item3),
					context.GetArgument<T4>(arguments.Item4),
					context.GetArgument<T5>(arguments.Item5),
					context.GetArgument<T6>(arguments.Item6))),
				Type = typeof(R).ToGraphQLType(false)
			});

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <remarks>The method's declaring type must be registered in the <see cref="IServiceCollection"/>.</remarks>
		/// <param name="method">Graph endpoint implementation.</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		public FieldType AddMutation(MethodEntity method)
		{
			if (!method.HasReturnValue)
				throw new ArgumentException($"{nameof(AddMutation)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

			return @this.GetMutation().AddField(method);
		}

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <param name="method">Graph endpoint implementation.</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		public FieldType AddMutation(StaticMethodEntity method)
		{
			if (!method.HasReturnValue)
				throw new ArgumentException($"{nameof(AddMutation)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

			return @this.GetMutation().AddField(method);
		}

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <typeparam name="T">The class that holds the instance or static method to create a mutation endpoint from.</typeparam>
		/// <param name="method">The name of the method or set of methods to use (each method must have a unique GraphName).</param>
		/// <returns>The added <see cref="FieldType"/>(s).</returns>
		/// <exception cref="ArgumentException"/>
		public FieldType[] AddMutations<T>(string method)
			where T : notnull
		{
			var publicMethods = Type<T>.Methods[method]
				.Where(_ => _.IsPublic)
				.Select(@this.AddMutation);
			var publicStaticMethods = Type<T>.StaticMethods[method]
				.Where(_ => _.IsPublic)
				.Select(@this.AddMutation);
			return [.. publicMethods, .. publicStaticMethods];
		}
	}
}
