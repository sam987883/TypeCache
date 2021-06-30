// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.Reflection;
using TypeCache.Reflection.Mappers;

namespace TypeCache.Business.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><see cref="IFieldMapper{FROM, TO}"/></term> <description>Field mapper where types: FROM &lt;&gt; TO.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterFieldMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : notnull
			where TO : notnull
			=> @this.AddSingleton<IFieldMapper<FROM, TO>>(provider => new FieldMapper<FROM, TO>(overrides));

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

		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><see cref="IPropertyMapper{FROM, TO}"/></term> <description>Property mapper where types: FROM &lt;&gt; TO.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterPropertyMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : notnull
			where TO : notnull
			=> @this.AddSingleton<IPropertyMapper<FROM, TO>>(provider => new PropertyMapper<FROM, TO>(overrides));
	}
}
