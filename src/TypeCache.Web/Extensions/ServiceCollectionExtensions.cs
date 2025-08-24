// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Converters;
using TypeCache.Mediation;
using TypeCache.Web.Mediation;

namespace TypeCache.Web.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddHttpClientRule(this IServiceCollection @this, string name, Action<HttpClient> configure)
	{
		@this.AddHttpClient(name, configure);
		@this.AddKeyedScoped(name, (provider, key) => provider.GetRequiredService<IHttpClientFactory>().CreateClient(key.ToString()!));
		return @this.AddKeyedScoped<IRule<HttpClientRequest, HttpResponseMessage>>(name,
			(provider, key) => new HttpClientRule(provider.GetRequiredKeyedService<HttpClient>((string)key)));
	}

	public static IServiceCollection ConfigureSqlApi(this IServiceCollection @this)
		=> @this.Configure<JsonOptions>(options =>
		{
			options.SerializerOptions.Converters.Add(new DataRowJsonConverter());
			options.SerializerOptions.Converters.Add(new DataTableJsonConverter());
			options.SerializerOptions.Converters.Add(new DataSetJsonConverter());
		});
}
