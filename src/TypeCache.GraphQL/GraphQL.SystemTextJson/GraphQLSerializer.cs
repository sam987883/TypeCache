using System.Text.Json;
using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQL.Transport;
using TypeCache.Converters;

namespace GraphQL.SystemTextJson;

/// <summary>
/// Serializes an <see cref="ExecutionResult"/> (or any other object) to a stream using
/// the <see cref="System.Text.Json"/> library.
/// </summary>
public class GraphQLSerializer : IGraphQLTextSerializer
{
	/// <summary>
	/// Returns the set of options used by the underlying serializer.
	/// </summary>
	protected JsonSerializerOptions JsonOptions { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="GraphQLSerializer"/> class with default settings:
	/// no indenting and a default instance of the <see cref="ErrorInfoProvider"/> class.
	/// </summary>
	public GraphQLSerializer()
		: this(indent: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GraphQLSerializer"/> class with the specified settings
	/// and a default instance of the <see cref="ErrorInfoProvider"/> class.
	/// </summary>
	/// <param name="indent">Indicates if child objects should be indented</param>
	public GraphQLSerializer(bool indent)
		: this(GetDefaultSerializerOptions(indent))
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GraphQLSerializer"/> class with the specified settings.
	/// </summary>
	/// <param name="indent">Indicates if child objects should be indented</param>
	/// <param name="errorInfoProvider">Specifies the <see cref="IErrorInfoProvider"/> instance to use to serialize GraphQL errors</param>
	public GraphQLSerializer(bool indent, IErrorInfoProvider errorInfoProvider)
		: this(GetDefaultSerializerOptions(indent), errorInfoProvider ?? throw new ArgumentNullException(nameof(errorInfoProvider)))
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GraphQLSerializer"/> class with no indenting and the
	/// specified <see cref="IErrorInfoProvider"/>.
	/// </summary>
	/// <param name="errorInfoProvider">Specifies the <see cref="IErrorInfoProvider"/> instance to use to serialize GraphQL errors</param>
	public GraphQLSerializer(IErrorInfoProvider errorInfoProvider)
		: this(false, errorInfoProvider)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GraphQLSerializer"/> class configured with the specified callback.
	/// Configuration defaults to no indenting and a default instance of the <see cref="ErrorInfoProvider"/> class.
	/// </summary>
	/// <param name="configureSerializerOptions">Specifies a callback used to configure the JSON serializer</param>
	public GraphQLSerializer(Action<JsonSerializerOptions> configureSerializerOptions)
	{
		ArgumentNullException.ThrowIfNull(configureSerializerOptions);

		this.JsonOptions = GetDefaultSerializerOptions(indent: false);
		configureSerializerOptions.Invoke(this.JsonOptions);

		this.ConfigureOptions(null);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GraphQLSerializer"/> class with the specified settings
	/// and a default instance of the <see cref="ErrorInfoProvider"/> class.
	/// <br/><br/>
	/// Note that <see cref="JsonSerializerOptions"/> is designed in such a way to reuse created objects.
	/// This leads to a massive speedup in subsequent calls to the serializer. The downside is you can't
	/// change properties on the options object after you've passed it in a
	/// <see cref="JsonSerializer.Serialize(object, Type, JsonSerializerOptions)">Serialize()</see> or
	/// <see cref="JsonSerializer.Deserialize(string, Type, JsonSerializerOptions)">Deserialize()</see> call.
	/// You'll get the exception: <i><see cref="InvalidOperationException"/>: Serializer options cannot be changed
	/// once serialization or deserialization has occurred</i>. To get around this problem we create a copy of
	/// options on NET5 platform and above. Passed options object remains unchanged and any changes you
	/// make are unobserved.
	/// </summary>
	/// <param name="serializerOptions">Specifies the JSON serializer settings</param>
	public GraphQLSerializer(JsonSerializerOptions serializerOptions)
	{
		ArgumentNullException.ThrowIfNull(serializerOptions);

		this.JsonOptions = new(serializerOptions);
		this.ConfigureOptions(null);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GraphQLSerializer"/> class with the specified settings.
	/// <br/><br/>
	/// Note that <see cref="JsonSerializerOptions"/> is designed in such a way to reuse created objects.
	/// This leads to a massive speedup in subsequent calls to the serializer. The downside is you can't
	/// change properties on the options object after you've passed it in a
	/// <see cref="JsonSerializer.Serialize(object, Type, JsonSerializerOptions)">Serialize()</see> or
	/// <see cref="JsonSerializer.Deserialize(string, Type, JsonSerializerOptions)">Deserialize()</see> call.
	/// You'll get the exception: <i><see cref="InvalidOperationException"/>: Serializer options cannot be changed
	/// once serialization or deserialization has occurred</i>. To get around this problem we create a copy of
	/// options on NET5 platform and above. Passed options object remains unchanged and any changes you
	/// make are unobserved.
	/// </summary>
	/// <param name="serializerOptions">Specifies the JSON serializer settings</param>
	/// <param name="errorInfoProvider">Specifies the <see cref="IErrorInfoProvider"/> instance to use to serialize GraphQL errors</param>
	public GraphQLSerializer(JsonSerializerOptions serializerOptions, IErrorInfoProvider errorInfoProvider)
	{
		ArgumentNullException.ThrowIfNull(serializerOptions);
		ArgumentNullException.ThrowIfNull(errorInfoProvider);

		this.JsonOptions = new(serializerOptions);
		this.ConfigureOptions(errorInfoProvider);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GraphQLSerializer"/> class with the specified settings.
	/// Configuration defaults to no indenting and the specified instance of the <see cref="ErrorInfoProvider"/> class.
	/// </summary>
	/// <param name="configureSerializerOptions">Specifies a callback used to configure the JSON serializer</param>
	/// <param name="errorInfoProvider">Specifies the <see cref="IErrorInfoProvider"/> instance to use to serialize GraphQL errors</param>
	public GraphQLSerializer(Action<JsonSerializerOptions> configureSerializerOptions, IErrorInfoProvider errorInfoProvider)
	{
		ArgumentNullException.ThrowIfNull(configureSerializerOptions);
		ArgumentNullException.ThrowIfNull(errorInfoProvider);

		this.JsonOptions = GetDefaultSerializerOptions(indent: false);
		configureSerializerOptions.Invoke(JsonOptions);

		this.ConfigureOptions(errorInfoProvider);
	}

	private void ConfigureOptions(IErrorInfoProvider? errorInfoProvider)
	{
		if (!this.JsonOptions.Converters.Any(_ => _.CanConvert(typeof(ExecutionResult))))
			this.JsonOptions.Converters.Add(new ExecutionResultJsonConverter());

		if (!this.JsonOptions.Converters.Any(_ => _.CanConvert(typeof(ExecutionError))))
			this.JsonOptions.Converters.Add(new ExecutionErrorJsonConverter(errorInfoProvider ?? new ErrorInfoProvider()));

		if (!this.JsonOptions.Converters.Any(_ => _.CanConvert(typeof(ApolloTrace))))
			this.JsonOptions.Converters.Add(new ApolloTraceJsonConverter());

		if (!this.JsonOptions.Converters.Any(_ => _.CanConvert(typeof(System.Numerics.BigInteger))))
			this.JsonOptions.Converters.Add(new BigIntegerJsonConverter());

		if (!this.JsonOptions.Converters.Any(_ => _.CanConvert(typeof(IReadOnlyDictionary<string, object?>))))
			this.JsonOptions.Converters.Add(new ReadOnlyDictionaryJsonConverter());

		if (!this.JsonOptions.Converters.Any(_ => _.CanConvert(typeof(OperationMessage))))
			this.JsonOptions.Converters.Add(new OperationMessageJsonConverter());
	}

	private static JsonSerializerOptions GetDefaultSerializerOptions(bool indent)
		=> new() { WriteIndented = indent };

	/// <inheritdoc/>
	public Task WriteAsync<T>(Stream stream, T? value, CancellationToken cancellationToken = default)
		=> JsonSerializer.SerializeAsync(stream, value, JsonOptions, cancellationToken);

	/// <inheritdoc/>
	public ValueTask<T?> ReadAsync<T>(Stream stream, CancellationToken cancellationToken = default)
		=> JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);

	/// <inheritdoc/>
	public string Serialize<T>(T? value)
		=> JsonSerializer.Serialize(value, JsonOptions);

	/// <inheritdoc/>
	public T? Deserialize<T>(string? json)
		=> json is null ? default : JsonSerializer.Deserialize<T>(json, JsonOptions);

	/// <summary>
	/// Converts the <see cref="JsonElement"/> representing a single JSON value into a <typeparamref name="T"/>.
	/// </summary>
	private T? ReadNode<T>(JsonElement jsonElement)
		=> JsonSerializer.Deserialize<T>(jsonElement, JsonOptions);

	/// <summary>
	/// Converts the <see cref="JsonElement"/> representing a single JSON value into a <typeparamref name="T"/>.
	/// A <paramref name="value"/> of <see langword="null"/> returns <see langword="default"/>.
	/// Throws a <see cref="InvalidCastException"/> if <paramref name="value"/> is not a <see cref="JsonElement"/>.
	/// </summary>
	public T? ReadNode<T>(object? value)
		=> value is null ? default : ReadNode<T>((JsonElement)value);

	/// <inheritdoc/>
	public bool IsNativelyAsync => true;
}
