// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.Reflection;
using TypeCache.Mappers;

namespace TypeCache.Mappers.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><c><see cref="IFieldMapper{FROM, TO}"/></c></term> <description>Default field mapper implementation matching by field names only.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterDefaultFieldMapper(this IServiceCollection @this)
			=> @this.AddSingleton(typeof(IFieldMapper<,>), typeof(DefaultFieldMapper<,>));

		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><c><see cref="IPropertyMapper{FROM, TO}"/></c></term> <description>Default property mapper implementation matching by property names only.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterDefaultPropertyMapper(this IServiceCollection @this)
			=> @this.AddSingleton(typeof(IPropertyMapper<,>), typeof(DefaultPropertyMapper<,>));
	}
}
