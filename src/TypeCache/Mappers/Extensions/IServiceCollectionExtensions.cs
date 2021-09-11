// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Mappers.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><c><see cref="IFieldMapper{FROM, TO}"/></c></term> Default field mapper implementation matching by field names only.</item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterDefaultFieldMapper(this IServiceCollection @this)
			=> @this.AddSingleton(typeof(IFieldMapper<,>), typeof(DefaultFieldMapper<,>));

		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><c><see cref="IPropertyMapper{FROM, TO}"/></c></term> Default property mapper implementation matching by property names only.</item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterDefaultPropertyMapper(this IServiceCollection @this)
			=> @this.AddSingleton(typeof(IPropertyMapper<,>), typeof(DefaultPropertyMapper<,>));
	}
}
