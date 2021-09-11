// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Business.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="DefaultProcessIntermediary{I}"/></term> Default implementation of <see cref="IProcessIntermediary{I}"/>.</item>
		/// <item><term><see cref="DefaultRuleIntermediary{I, O}"/></term> Default implementation of <see cref="IRuleIntermediary{I, O}"/>.</item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterMediator(this IServiceCollection @this)
			=> @this.AddSingleton<IMediator, Mediator>()
				.AddSingleton(typeof(DefaultProcessIntermediary<>), typeof(DefaultProcessIntermediary<>))
				.AddSingleton(typeof(DefaultRuleIntermediary<,>), typeof(DefaultRuleIntermediary<,>));
	}
}
