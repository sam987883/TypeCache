// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Converters;

namespace TypeCache.Web.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection ConfigureSqlApi(this IServiceCollection @this)
		=> @this.Configure<JsonOptions>(options =>
		{
			options.SerializerOptions.Converters.Add(new DataRowJsonConverter());
			options.SerializerOptions.Converters.Add(new DataTableJsonConverter());
			options.SerializerOptions.Converters.Add(new DataSetJsonConverter());
		});
}
