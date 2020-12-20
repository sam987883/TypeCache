// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using TypeCache.Business;
using TypeCache.Common;
using TypeCache.Converters;
using TypeCache.Data;
using TypeCache.Data.Schema;
using TypeCache.Reflection;
using TypeCache.Reflection.Caches;
using TypeCache.Reflection.Converters;
using TypeCache.Reflection.Mappers;
using TypeCache.Security;
using TypeCache.Security.Coders;

namespace TypeCache.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Makes a call to:
		/// <code>DbProviderFactories.RegisterFactory(databaseProvider, dbProviderFactory);</code>
		/// </summary>
		/// <param name="databaseProvider">ie. <c>"Microsoft.Data.SqlClient"</c></param>
		/// <param name="dbProviderFactory">ie. <c>SqlClientFactory.Instance</c></param>
		public static IServiceCollection RegisterDatabaseProviderFactory(this IServiceCollection @this, string databaseProvider, DbProviderFactory dbProviderFactory)
		{
			DbProviderFactories.RegisterFactory(databaseProvider, dbProviderFactory);
			return @this;
		}

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><see cref="ISchemaFactory"/></item>
		/// <item><see cref="ISchemaStore"/></item>
		/// </list>
		/// <i>Requires call to: <see cref="Reflection.Extensions.IServiceCollectionExtensions.RegisterTypeCache"/></i>
		/// </summary>
		public static IServiceCollection RegisterDatabaseSchema(this IServiceCollection @this)
			=> @this
				.AddSingleton<ISchemaFactory>(provider => new SchemaFactory(provider.GetRequiredService<ITypeCache>()))
				.AddSingleton<ISchemaStore>(provider => new SchemaStore(provider.GetRequiredService<ISchemaFactory>()));

		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><see cref="IFieldMapper&lt;FROM, TO&gt;"/></term> <description>Field mapper where types: FROM &lt;&gt; TO.</description></item>
		/// </list>
		/// <i>Requires call to: <see cref="RegisterTypeCache"/></i>
		/// </summary>
		public static IServiceCollection RegisterFieldMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : class
			where TO : class
			=> @this.AddSingleton<IFieldMapper<FROM, TO>>(provider =>
				new FieldMapper<FROM, TO>(provider.GetRequiredService<IFieldCache<FROM>>(), provider.GetRequiredService<IFieldCache<TO>>(), overrides));

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="IProcess&lt;&gt;"/></term> <description>All implementations of IProcess.</description></item>
		/// <item><term><see cref="IProcessHandler&lt;&gt;"/>, <see cref="IProcessHandler&lt;,&gt;"/></term> <description>All implementations of IProcessHandler.</description></item>
		/// <item><term><see cref="IRule&lt;,&gt;"/></term> <description>All implementations of IRule.</description></item>
		/// <item><term><see cref="IRuleHandler&lt,;&gt;"/>, <see cref="IRuleHandler&lt;,,&gt;"/></term> <description>All implementations of IRuleHandler.</description></item>
		/// </list>
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static IServiceCollection RegisterMediator(this IServiceCollection @this)
			=> @this.AddSingleton(typeof(IProcess<>))
				.AddSingleton(typeof(IProcessHandler<>))
				.AddSingleton(typeof(IProcessHandler<,>))
				.AddSingleton(typeof(IRule<,>))
				.AddSingleton(typeof(IRuleHandler<,>))
				.AddSingleton(typeof(IRuleHandler<,,>));

		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><see cref="IPropertyMapper&lt;FROM, TO&gt;"/></term> <description>Property mapper where types: FROM &lt;&gt; TO.</description></item>
		/// </list>
		/// <i>Requires call to: <see cref="RegisterTypeCache"/></i>
		/// </summary>
		public static IServiceCollection RegisterPropertyMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : class
			where TO : class
			=> @this.AddSingleton<IPropertyMapper<FROM, TO>>(provider =>
				new PropertyMapper<FROM, TO>(provider.GetRequiredService<IPropertyCache<FROM>>(), provider.GetRequiredService<IPropertyCache<TO>>(), overrides));

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="IHashMaker"/></term> <description>Utility class that encrypts a long to a simple string hashed ID and back.</description></item>
		/// </list>
		/// </summary>
		/// <param name="rgbKey">Any random 16 bytes</param>
		/// <param name="rgbIV">Any random 16 bytes</param>
		public static IServiceCollection RegisterSecurity(this IServiceCollection @this, byte[] rgbKey, byte[] rgbIV)
			=> @this.AddSingleton<IHashMaker>(new HashMaker(rgbKey, rgbIV));

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="IHashMaker"/></term> <description>Utility class that encrypts a long to a simple string hashed ID and back.</description></item>
		/// </list>
		/// </summary>
		/// <param name="rgbKey">Any random decimal value (gets cionverted to 16 byte array)</param>
		/// <param name="rgbIV">Any random decimal value (gets cionverted to 16 byte array)</param>
		public static IServiceCollection RegisterSecurity(this IServiceCollection @this, decimal rgbKey, decimal rgbIV)
			=> @this.AddSingleton<IHashMaker>(new HashMaker(rgbKey, rgbIV));

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="ITypeCache"/></term> <description>Utility class that handles Relection for anonymous types.</description></item>
		/// <item><term><see cref="IConstructorCache&lt;&gt;"/></term> <description>Class constructors.</description></item>
		/// <item><term><see cref="IIndexerCache&lt;&gt;"/></term> <description>Class indexers.</description></item>
		/// <item><term><see cref="IMethodCache&lt;&gt;"/>, <see cref="IStaticMethodCache&lt;&gt;"/></term> <description>Class methods.</description></item>
		/// <item><term><see cref="IPropertyCache&lt;&gt;"/>, <see cref="IStaticPropertyCache&lt;&gt;"/></term> <description>Class properties.</description></item>
		/// <item><term><see cref="IFieldCache&lt;&gt;"/>, <see cref="IStaticFieldCache&lt;&gt;"/></term> <description>Class fields.</description></item>
		/// <item><term><see cref="IEnumCache&lt;&gt;"/></term> <description>Enum fields and information.</description></item>
		/// <item><term><see cref="IPropertyMapper&lt;,&gt;"/>, <see cref="IFieldMapper&lt;,&gt;"/></term> <description>Mappers where types: FROM = TO.</description></item>
		/// <item><term><see cref="PropertyJsonConverter&lt;&gt;"/>, <see cref="FieldJsonConverter&lt;&gt;"/></term> <description>TypeCache based JSON converters.</description></item>
		/// <item><term><see cref="IEnumEqualityComparer&lt;&gt;"/>, <see cref="IEnumComparer&lt;&gt;"/></term> <description>EnumCache based IEqualityComparer&lt;&gt; and IComparer&lt;&gt;.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterTypeCache(this IServiceCollection @this)
			=> @this
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
				.AddSingleton<ITypeCache>(new Reflection.Caches.TypeCache(((ServiceCollection)@this).BuildServiceProvider()))
				.AddSingleton(typeof(PropertyJsonConverter<>), typeof(PropertyJsonConverter<>))
				.AddSingleton(typeof(FieldJsonConverter<>), typeof(FieldJsonConverter<>))
				.AddSingleton(typeof(IEnumEqualityComparer<>), typeof(EnumEqualityComparer<>))
				.AddSingleton(typeof(IEnumComparer<>), typeof(EnumComparer<>));
	}
}
