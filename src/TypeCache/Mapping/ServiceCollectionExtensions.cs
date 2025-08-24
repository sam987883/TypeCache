// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Mapping;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddFieldMapper(this IServiceCollection @this)
		=> @this.AddSingleton<IMapper, FieldMapper>();

	public static IServiceCollection AddPropertyMapper(this IServiceCollection @this)
		=> @this.AddSingleton<IMapper, PropertyMapper>();
}
