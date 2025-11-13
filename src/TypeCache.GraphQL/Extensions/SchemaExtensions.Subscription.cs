// Copyright (c) 2021 Samuel Abraham

using System.Data;
using GraphQL;
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
		/// <c>=&gt; @this.Subscription ??= <see langword="new"/> <see cref="ObjectGraphType"/> { Name = <see langword="nameof"/>(<see cref="ISchema.Subscription"/>) };</c>
		/// </remarks>
		public IObjectGraphType GetSubscription()
			=> @this.Subscription ??= new ObjectGraphType { Name = nameof(ISchema.Subscription) };

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <param name="method">Graph endpoint implementation that returns <c><see cref="IObservable{T}"/></c>, <c>Task&lt;<see cref="IObservable{T}"/>&gt;</c> or <c>ValueTask&lt;<see cref="IObservable{T}"/>&gt;</c>.</param>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>.</remarks>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		public FieldType AddSubscription(MethodEntity method)
			=> @this.GetSubscription().AddFieldStream(method);

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <param name="method">Graph endpoint implementation that returns <c><see cref="IObservable{T}"/></c>, <c>Task&lt;<see cref="IObservable{T}"/>&gt;</c> or <c>ValueTask&lt;<see cref="IObservable{T}"/>&gt;</c>.</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		public FieldType AddSubscription(StaticMethodEntity method)
			=> @this.GetSubscription().AddFieldStream(method);

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <typeparam name="T">The class that holds the instance or static method to create a subscription endpoint from.</typeparam>
		/// <param name="method">The name of the method or set of methods to use (each method must have a unique GraphName).</param>
		/// <returns>The added <see cref="FieldType"/>(s).</returns>
		/// <exception cref="ArgumentException"/>
		public FieldType[] AddSubscriptions<T>(string method)
			where T : notnull
		{
			var publicMethods = Type<T>.Methods[method]
				.Where(_ => _.IsPublic)
				.Select(@this.AddSubscription);
			var publicStaticMethods = Type<T>.StaticMethods[method]
				.Where(_ => _.IsPublic)
				.Select(@this.AddSubscription);
			return [.. publicMethods, .. publicStaticMethods];
		}
	}
}
