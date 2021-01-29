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
		/// <item><term><see cref="IFieldMapper&lt;FROM, TO&gt;"/></term> <description>Field mapper where types: FROM &lt;&gt; TO.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterFieldMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : notnull
			where TO : notnull
			=> @this.AddSingleton<IFieldMapper<FROM, TO>>(provider => new FieldMapper<FROM, TO>(overrides));

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="DefaultProcessHandler&lt;T&gt;"/>, <see cref="DefaultProcessHandler&lt;M,T&gt;"/></term> <description>Default implementation of IProcessHandler.</description></item>
		/// <item><term><see cref="DefaultRuleHandler&lt;T,R&gt;"/>, <see cref="DefaultRuleHandler&lt;M,T,R&gt;"/></term> <description>Default implementation of IRuleHandler.</description></item>
		/// <item><term><see cref="DefaultRulesHandler&lt;T,R&gt;"/>, <see cref="DefaultRulesHandler&lt;M,T,R&gt;"/></term> <description>Default implementation of IRulesHandler.</description></item>
		/// </list>
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static IServiceCollection RegisterMediator(this IServiceCollection @this)
			=> @this.AddSingleton<IMediator, Mediator>()
				.AddSingleton(typeof(DefaultProcessHandler<>), typeof(DefaultProcessHandler<>))
				.AddSingleton(typeof(DefaultProcessHandler<,>), typeof(DefaultProcessHandler<,>))
				.AddSingleton(typeof(DefaultRuleHandler<,>), typeof(DefaultRuleHandler<,>))
				.AddSingleton(typeof(DefaultRuleHandler<,,>), typeof(DefaultRuleHandler<,,>))
				.AddSingleton(typeof(DefaultRulesHandler<,>), typeof(DefaultRulesHandler<,>))
				.AddSingleton(typeof(DefaultRulesHandler<,,>), typeof(DefaultRulesHandler<,,>));

		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><see cref="IPropertyMapper&lt;FROM, TO&gt;"/></term> <description>Property mapper where types: FROM &lt;&gt; TO.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterPropertyMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : notnull
			where TO : notnull
			=> @this.AddSingleton<IPropertyMapper<FROM, TO>>(provider => new PropertyMapper<FROM, TO>(overrides));
	}
}
