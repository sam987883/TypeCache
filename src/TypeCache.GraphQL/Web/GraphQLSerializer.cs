// Copyright (c) 2021 Samuel Abraham

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Converters;
using TypeCache.GraphQL.Converters;

namespace TypeCache.GraphQL.Web;

public sealed class GraphQLJsonSerializer : global::GraphQL.IGraphQLSerializer
{
	private readonly JsonSerializerOptions _JsonOptions;

	public GraphQLJsonSerializer(JsonConverter<global::GraphQL.ExecutionError> executionErrorJsonConverter)
	{
		this._JsonOptions = new()
		{
			MaxDepth = 40,
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};
		this._JsonOptions.Converters.Add(new BigIntegerJsonConverter());
		this._JsonOptions.Converters.Add(new DictionaryJsonConverter());
		this._JsonOptions.Converters.Add(new GraphQLExecutionResultJsonConverter());
		this._JsonOptions.Converters.Add(executionErrorJsonConverter);
	}

	public bool IsNativelyAsync => true;

	public ValueTask<T?> ReadAsync<T>(Stream stream, CancellationToken cancellationToken = default)
		=> JsonSerializer.DeserializeAsync<T>(stream, this._JsonOptions, cancellationToken);

	public Task WriteAsync<T>(Stream stream, T? value, CancellationToken cancellationToken = default)
		=> JsonSerializer.SerializeAsync(stream, value, this._JsonOptions, cancellationToken);

	private T? ReadNode<T>(JsonElement jsonElement)
		=> jsonElement.Deserialize<T>(this._JsonOptions);

	public T? ReadNode<T>(object? value)
		=> value is not null ? ((JsonElement)value).Deserialize<T>(this._JsonOptions) : default;
}
