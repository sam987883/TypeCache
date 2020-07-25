// Copyright (c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using sam987883.Common;
using sam987883.Common.Converters;
using sam987883.Database;
using sam987883.Database.Models;
using sam987883.Reflection;
using sam987883.Reflection.Mappers;
using System;
using System.Collections.Generic;

namespace sam987883.Dependencies
{
    public static class IServiceCollectionExtensions
    {
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

        public static IServiceCollection RegisterFieldMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides) => @this
            .AddSingleton<IFieldMapper<FROM, TO>>(provider =>
                new FieldMapper<FROM, TO>(provider.GetRequiredService<IFieldCache<FROM>>(), provider.GetRequiredService<IFieldCache<TO>>(), overrides));

        public static IServiceCollection RegisterPropertyMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides) => @this
            .AddSingleton<IPropertyMapper<FROM, TO>>(provider =>
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
            .AddSingleton(typeof(IFieldMapper<,>), typeof(FieldMapper<,>))
            .AddSingleton(typeof(IPropertyMapper<,>), typeof(PropertyMapper<,>));
    }
}
