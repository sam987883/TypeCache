// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Sam987883.Reflection.Accessors;
using Sam987883.Reflection.Caches;
using Sam987883.Reflection.Converters;
using Sam987883.Reflection.Mappers;
using System;

namespace Sam987883.Reflection.Extensions
{
	public static class IServiceCollectionExtensions
	{
		private static IFieldAccessor CreateFieldAccessor(IServiceProvider provider, object instance)
		{
			var type = typeof(IFieldAccessorFactory<>).MakeGenericType(instance.GetType());
			dynamic factory = provider.GetRequiredService(type);
			return factory.Create(instance);
		}

		private static IPropertyAccessor CreatePropertyAccessor(IServiceProvider provider, object instance)
		{
			var type = typeof(IPropertyAccessorFactory<>).MakeGenericType(instance.GetType());
			dynamic factory = provider.GetRequiredService(type);
			return factory.Create(instance);
		}

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><see cref="IFieldMapper&lt;FROM, TO&gt;"/></item>
		/// </list>
		/// <i>Requires call to: <see cref="RegisterTypeCache"/></i>
		/// </summary>
		public static IServiceCollection RegisterFieldMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : class
			where TO : class => @this.AddSingleton<IFieldMapper<FROM, TO>>(provider =>
				new FieldMapper<FROM, TO>(provider.GetRequiredService<IFieldCache<FROM>>(), provider.GetRequiredService<IFieldCache<TO>>(), overrides));

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><see cref="IPropertyMapper&lt;FROM, TO&gt;"/></item>
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
		/// <item><see cref="ITypeCache&lt;&gt;"/></item>
		/// <item><see cref="IConstructorCache&lt;&gt;"/></item>
		/// <item><see cref="IIndexerCache&lt;&gt;"/></item>
		/// <item><see cref="IMethodCache&lt;&gt;"/>, <see cref="IStaticMethodCache&lt;&gt;"/></item>
		/// <item><see cref="IPropertyCache&lt;&gt;"/>, <see cref="IStaticPropertyCache&lt;&gt;"/></item>
		/// <item><see cref="IFieldCache&lt;&gt;"/>, <see cref="IStaticFieldCache&lt;&gt;"/></item>
		/// <item><see cref="IEnumCache&lt;&gt;"/></item>
		/// <item><see cref="IPropertyAccessorFactory&lt;&gt;"/>, <see cref="IFieldAccessorFactory&lt;&gt;"/></item>
		/// <item><see cref="Func&lt;object, IPropertyAccessor&gt;"/>, <see cref="Func&lt;object, IFieldAccessor&gt;"/></item>
		/// <item><see cref="IPropertyMapper&lt;,&gt;"/>, <see cref="IFieldMapper&lt;,&gt;"/></item>
		/// <item><see cref="PropertyJsonConverter&lt;&gt;"/>, <see cref="FieldJsonConverter&lt;&gt;"/></item>
		/// <item><see cref="IRowSetConverter&lt;&gt;"/></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterTypeCache(this IServiceCollection @this) => @this
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
			.AddSingleton(typeof(IPropertyAccessorFactory<>), typeof(PropertyAccessorFactory<>))
			.AddSingleton(typeof(IFieldAccessorFactory<>), typeof(FieldAccessorFactory<>))
			.AddSingleton(typeof(Func<object, IPropertyAccessor>), provider => new Func<object, IPropertyAccessor>(instance => CreatePropertyAccessor(provider, instance)))
			.AddSingleton(typeof(Func<object, IFieldAccessor>), provider => new Func<object, IFieldAccessor>(instance => CreateFieldAccessor(provider, instance)))
			.AddSingleton(typeof(IPropertyMapper<,>), typeof(PropertyMapper<,>))
			.AddSingleton(typeof(IFieldMapper<,>), typeof(FieldMapper<,>))
			.AddSingleton(typeof(PropertyJsonConverter<>), typeof(PropertyJsonConverter<>))
			.AddSingleton(typeof(FieldJsonConverter<>), typeof(FieldJsonConverter<>))
			.AddSingleton(typeof(IRowSetConverter<>), typeof(RowSetConverter<>));
	}
}
