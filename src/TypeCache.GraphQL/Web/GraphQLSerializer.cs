// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using System.Text.Json;
using global::GraphQL;

namespace TypeCache.GraphQL.Web;

/// <summary>
/// If using your own instance of this class, the <paramref name="jsonOptions"/> requires the following JSON converters to be
/// implemented/added for the following types:
/// <list type="bullet">
/// <item><c><see cref="ExecutionResult"/></c></item>
/// <item><c><see cref="ExecutionError"/></c></item>
/// <item><c><see cref="BigInteger"/></c> <b>(if using the <c><see cref="BigInteger"/></c> type)</b></item>
/// <item><c>IDictionary&lt;string, object?&gt;</c> <b>(can be helpful to support <c><see cref="ExecutionResult"/></c>)</b></item>
/// </list>
/// </summary>
/// <param name="jsonOptions"></param>
public sealed class GraphQLJsonSerializer(JsonSerializerOptions jsonOptions) : IGraphQLSerializer
{
	public bool IsNativelyAsync => true;

	public ValueTask<T?> ReadAsync<T>(Stream stream, CancellationToken cancellationToken = default)
		=> JsonSerializer.DeserializeAsync<T>(stream, jsonOptions, cancellationToken);

	public Task WriteAsync<T>(Stream stream, T? value, CancellationToken cancellationToken = default)
		=> JsonSerializer.SerializeAsync(stream, value, jsonOptions, cancellationToken);

	private T? ReadNode<T>(JsonElement jsonElement)
		=> jsonElement.Deserialize<T>(jsonOptions);

	public T? ReadNode<T>(object? value)
		=> value is not null ? ((JsonElement)value).Deserialize<T>(jsonOptions) : default;
}
