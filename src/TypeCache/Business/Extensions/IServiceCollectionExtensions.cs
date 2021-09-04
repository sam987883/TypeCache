// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.Reflection;
using TypeCache.Mappers;

namespace TypeCache.Business.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="DefaultProcessIntermediary{I}"/></term> <description>Default implementation of <see cref="IProcessIntermediary{I}"/>.</description></item>
		/// <item><term><see cref="DefaultRuleIntermediary{I, O}"/></term> <description>Default implementation of <see cref="IRuleIntermediary{I, O}"/>.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterMediator(this IServiceCollection @this)
			=> @this.AddSingleton<IMediator, Mediator>()
				.AddSingleton(typeof(DefaultProcessIntermediary<>), typeof(DefaultProcessIntermediary<>))
				.AddSingleton(typeof(DefaultRuleIntermediary<,>), typeof(DefaultRuleIntermediary<,>));
	}
}
