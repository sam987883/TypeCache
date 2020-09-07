// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Sam987883.Reflection.Caches;
using Sam987883.Reflection.Converters;
using Sam987883.Reflection.Mappers;

namespace Sam987883.Reflection.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><see cref="IFieldMapper&lt;FROM, TO&gt;"/></term> <description>Field mapper where types: FROM &lt;&gt; TO.</description></item>
		/// </list>
		/// <i>Requires call to: <see cref="RegisterTypeCache"/></i>
		/// </summary>
		public static IServiceCollection RegisterFieldMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : class
			where TO : class => @this.AddSingleton<IFieldMapper<FROM, TO>>(provider =>
				new FieldMapper<FROM, TO>(provider.GetRequiredService<IFieldCache<FROM>>(), provider.GetRequiredService<IFieldCache<TO>>(), overrides));

		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><see cref="IPropertyMapper&lt;FROM, TO&gt;"/></term> <description>Property mapper where types: FROM &lt;&gt; TO.</description></item>
		/// </list>
		/// <i>Requires call to: <see cref="RegisterTypeCache"/></i>
		/// </summary>
		public static IServiceCollection RegisterPropertyMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : class
			where TO : class => @this.AddSingleton<IPropertyMapper<FROM, TO>>(provider =>
				new PropertyMapper<FROM, TO>(provider.GetRequiredService<IPropertyCache<FROM>>(), provider.GetRequiredService<IPropertyCache<TO>>(), overrides));

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="ITypeCache"/></term> <description>Useful utility class.</description></item>
		/// <item><term><see cref="ITypeCache&lt;&gt;"/></term> <description>Class type information.</description></item>
		/// <item><term><see cref="IConstructorCache&lt;&gt;"/></term> <description>Class constructors.</description></item>
		/// <item><term><see cref="IIndexerCache&lt;&gt;"/></term> <description>Class indexers.</description></item>
		/// <item><term><see cref="IMethodCache&lt;&gt;"/>, <see cref="IStaticMethodCache&lt;&gt;"/></term> <description>Class methods.</description></item>
		/// <item><term><see cref="IPropertyCache&lt;&gt;"/>, <see cref="IStaticPropertyCache&lt;&gt;"/></term> <description>Class properties.</description></item>
		/// <item><term><see cref="IFieldCache&lt;&gt;"/>, <see cref="IStaticFieldCache&lt;&gt;"/></term> <description>Class fields.</description></item>
		/// <item><term><see cref="IEnumCache&lt;&gt;"/></term> <description>Enum fields and information.</description></item>
		/// <item><term><see cref="IPropertyMapper&lt;,&gt;"/>, <see cref="IFieldMapper&lt;,&gt;"/></term> <description>Mappers where types: FROM = TO.</description></item>
		/// <item><term><see cref="PropertyJsonConverter&lt;&gt;"/>, <see cref="FieldJsonConverter&lt;&gt;"/></term> <description>TypeCache based JSON converters.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterTypeCache(this IServiceCollection @this) => @this
			.AddSingleton<ITypeCache>(serviceProvider => new TypeCache(serviceProvider))
			.AddSingleton(typeof(ITypeCache<>), typeof(TypeCache<>))
			.AddSingleton(typeof(IConstructorCache<>), typeof(ConstructorCache<>))
			.AddSingleton(typeof(IIndexerCache<>), typeof(IndexerCache<>))
			.AddSingleton(typeof(IMethodCache<>), typeof(MethodCache<>))
			.AddSingleton(typeof(IStaticMethodCache<>), typeof(StaticMethodCache<>))
			.AddSingleton(typeof(IPropertyCache<>), typeof(PropertyCache<>))
			.AddSingleton(typeof(IStaticPropertyCache<>), typeof(StaticPropertyCache<>))
			.AddSingleton(typeof(IFieldCache<>), typeof(FieldCache<>))
			.AddSingleton(typeof(IStaticFieldCache<>), typeof(StaticFieldCache<>))
			.AddSingleton(typeof(IEnumCache<>), typeof(EnumCache<>))
			.AddSingleton(typeof(IPropertyMapper<,>), typeof(PropertyMapper<,>))
			.AddSingleton(typeof(IFieldMapper<,>), typeof(FieldMapper<,>))
			.AddSingleton(typeof(PropertyJsonConverter<>), typeof(PropertyJsonConverter<>))
			.AddSingleton(typeof(FieldJsonConverter<>), typeof(FieldJsonConverter<>));
	}
}
