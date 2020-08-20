// Copyright (c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Sam987883.Common;
using Sam987883.Common.Converters;
using Sam987883.Database;
using Sam987883.Database.Models;
using Sam987883.Reflection;
using Sam987883.Reflection.Accessors;
using Sam987883.Reflection.Converters;
using Sam987883.Reflection.Mappers;
using System;
using System.Collections.Generic;

namespace Sam987883.Dependencies
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

        public static IServiceCollection AddSingleton<T>(this IServiceCollection @this, string name, T service) =>
            @this.AddSingleton(typeof(INamedService<T>), new NamedService<T>(name, service));

        public static IServiceCollection AddSingleton<T>(this IServiceCollection @this, string name, Func<T> serviceFactory) =>
            @this.AddSingleton(typeof(INamedService<T>), new NamedService<T>(name, serviceFactory));

        public static IServiceCollection AddSingleton<T>(this IServiceCollection @this, string name, Func<IServiceProvider, T> serviceFactory) =>
            @this.AddSingleton(typeof(INamedService<T>), provider => new NamedService<T>(name, () => serviceFactory(provider)));

        public static IServiceCollection RegisterDatabaseSchema(this IServiceCollection @this) => @this
            .AddSingleton(typeof(IRowSetConverter<>), typeof(RowSetConverter<>))
            .AddSingleton<ISchemaFactory>(provider => new SchemaFactory(provider.GetRequiredService<IRowSetConverter<ObjectSchema>>()
                , provider.GetRequiredService<IRowSetConverter<ColumnSchema>>()
                , provider.GetRequiredService<IRowSetConverter<ParameterSchema>>()))
            .AddSingleton<ISchemaStore>(provider => new SchemaStore(provider.GetRequiredService<ISchemaFactory>()));

        public static IServiceCollection RegisterDefaultComparers(this IServiceCollection @this) => @this
            .AddSingleton(typeof(IComparer<>), typeof(CustomComparer<>))
            .AddSingleton(typeof(IEqualityComparer<>), typeof(CustomEqualityComparer<>))
            .AddSingleton(typeof(IReferenceEqualityComparer<>), typeof(ReferenceEqualityComparer<>));

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
            .AddSingleton(typeof(PropertyJsonConverter<>), typeof(PropertyJsonConverter<>));
    }
}
