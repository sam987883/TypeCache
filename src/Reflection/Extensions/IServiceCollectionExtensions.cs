// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Sam987883.Reflection.Accessors;
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

		public static IServiceCollection RegisterFieldMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : class
			where TO : class => @this.AddSingleton<IFieldMapper<FROM, TO>>(provider =>
				new FieldMapper<FROM, TO>(provider.GetRequiredService<IFieldCache<FROM>>(), provider.GetRequiredService<IFieldCache<TO>>(), overrides));

		public static IServiceCollection RegisterPropertyMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : class
			where TO : class => @this.AddSingleton<IPropertyMapper<FROM, TO>>(provider =>
				new PropertyMapper<FROM, TO>(provider.GetRequiredService<IPropertyCache<FROM>>(), provider.GetRequiredService<IPropertyCache<TO>>(), overrides));

		public static IServiceCollection RegisterTypeCache(this IServiceCollection @this) => @this
			.AddSingleton(typeof(ITypeCache<>), typeof(TypeCache<>))
			.AddSingleton(typeof(IConstructorCache<>), typeof(ConstructorCache<>))
			.AddSingleton(typeof(IEnumCache<>), typeof(EnumCache<>))
			.AddSingleton(typeof(IFieldCache<>), typeof(FieldCache<>))
			.AddSingleton(typeof(IMethodCache<>), typeof(MethodCache<>))
			.AddSingleton(typeof(IIndexerCache<>), typeof(IndexerCache<>))
			.AddSingleton(typeof(IPropertyCache<>), typeof(PropertyCache<>))
			.AddSingleton(typeof(IStaticFieldCache<>), typeof(StaticFieldCache<>))
			.AddSingleton(typeof(IStaticMethodCache<>), typeof(StaticMethodCache<>))
			.AddSingleton(typeof(IStaticPropertyCache<>), typeof(StaticPropertyCache<>))
			.AddSingleton(typeof(IFieldAccessorFactory<>), typeof(FieldAccessorFactory<>))
			.AddSingleton(typeof(Func<object, IFieldAccessor>), provider => new Func<object, IFieldAccessor>(instance => CreateFieldAccessor(provider, instance)))
			.AddSingleton(typeof(IFieldMapper<,>), typeof(FieldMapper<,>))
			.AddSingleton(typeof(IPropertyAccessorFactory<>), typeof(PropertyAccessorFactory<>))
			.AddSingleton(typeof(Func<object, IPropertyAccessor>), provider => new Func<object, IPropertyAccessor>(instance => CreatePropertyAccessor(provider, instance)))
			.AddSingleton(typeof(IPropertyMapper<,>), typeof(PropertyMapper<,>))
			.AddSingleton(typeof(FieldJsonConverter<>), typeof(FieldJsonConverter<>))
			.AddSingleton(typeof(PropertyJsonConverter<>), typeof(PropertyJsonConverter<>))
			.AddSingleton(typeof(IRowSetConverter<>), typeof(RowSetConverter<>));
	}
}
