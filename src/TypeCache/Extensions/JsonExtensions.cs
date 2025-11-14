// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class JsonExtensions
{
	private const char ANY_NODE = '*';
	private const char ROOT_NODE = '$';
	private const char SEPARATOR = '.';
	private const string EVERY = "..";

	[StringSyntax(StringSyntaxAttribute.Regex)]
	private const string ARRAY_REGEX = @"^\[(?'index'(((\^?\d*)?\.{2}(\^?\d*)?)|(\d+))(\,(((\^?\d*)?\.{2}(\^?\d*)?)|(\d+)))*)\]$";

	[StringSyntax(StringSyntaxAttribute.Regex)]
	private const string BRACKET_PATH = @"^\$((\[\'(?'token'[^']+)\'\])|(?'token'\[[\^\,\.\d]+\]))*$";

	[StringSyntax(StringSyntaxAttribute.Regex)]
	private const string DOT_PATH = @"^\$(\.(?'token'[^\.\[]+)?(?'token'\[[\^\.\,\d]+\])*)*$";

	extension(JsonElement @this)
	{
		public JsonElement[] GetArrayElements()
		{
			@this.ValueKind.ThrowIfNotEqual(JsonValueKind.Array);

			var array = new JsonElement[@this.GetArrayLength()];
			var i = -1;
			using var enumerator = @this.EnumerateArray();
			while (enumerator.MoveNext())
				array[++i] = enumerator.Current;

			return array;
		}

		public object?[] GetArrayValues()
		{
			@this.ValueKind.ThrowIfNotEqual(JsonValueKind.Array);

			var array = new object?[@this.GetArrayLength()];
			var i = -1;
			using var enumerator = @this.EnumerateArray();
			while (enumerator.MoveNext())
				array[++i] = enumerator.Current.GetValue();

			return array;
		}

		public IDictionary<string, JsonElement> GetObjectElements(StringComparison comparison)
		{
			@this.ValueKind.ThrowIfNotEqual(JsonValueKind.Object);

			var properties = new Dictionary<string, JsonElement>(comparison.ToComparer());
			using var enumerator = @this.EnumerateObject();
			while (enumerator.MoveNext())
				properties.Add(enumerator.Current.Name, enumerator.Current.Value);

			return properties;
		}

		/// <remarks>
		/// =&gt; @this.GetObjectElements(<see cref="StringComparison.OrdinalIgnoreCase"/>);
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IDictionary<string, JsonElement> GetObjectElementsIgnoreCase()
			=> @this.GetObjectElements(StringComparison.OrdinalIgnoreCase);

		/// <remarks>
		/// =&gt; @this.GetObjectElements(<see cref="StringComparison.Ordinal"/>);
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IDictionary<string, JsonElement> GetObjectElementsOrdinal()
			=> @this.GetObjectElements(StringComparison.Ordinal);

		public IDictionary<string, object?> GetObjectValues(StringComparison comparison)
		{
			@this.ValueKind.ThrowIfNotEqual(JsonValueKind.Object);

			var properties = new Dictionary<string, object?>(comparison.ToComparer());
			using var enumerator = @this.EnumerateObject();
			while (enumerator.MoveNext())
				properties.Add(enumerator.Current.Name, enumerator.Current.Value.GetValue());

			return properties;
		}

		/// <remarks>
		/// =&gt; @this.GetObjectValues(<see cref="StringComparison.OrdinalIgnoreCase"/>);
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IDictionary<string, object?> GetObjectValuesIgnoreCase()
			=> @this.GetObjectValues(StringComparison.OrdinalIgnoreCase);

		/// <remarks>
		/// =&gt; @this.GetObjectValues(<see cref="StringComparison.Ordinal"/>);
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IDictionary<string, object?> GetObjectValuesOrdinal()
			=> @this.GetObjectValues(StringComparison.Ordinal);

		public object? GetValue()
			=> @this.ValueKind switch
			{
				JsonValueKind.Undefined => throw new UnreachableException(Invariant($"{nameof(@this.ValueKind)} is {JsonValueKind.Undefined.Name}")),
				JsonValueKind.True => true,
				JsonValueKind.False => false,
				JsonValueKind.Number when @this.TryGetInt32(out var value) => value,
				JsonValueKind.Number when @this.TryGetInt64(out var value) => value,
				JsonValueKind.Number => @this.GetDecimal(),
				JsonValueKind.String when @this.TryGetDateTime(out var value) => value,
				JsonValueKind.String when @this.TryGetDateTimeOffset(out var value) => value,
				JsonValueKind.String when @this.TryGetGuid(out var value) => value,
				JsonValueKind.String => @this.GetString()!,
				JsonValueKind.Array => @this.GetArrayValues(),
				JsonValueKind.Object => @this.GetObjectValuesIgnoreCase(),
				_ => null
			};
	}

	extension(JsonNode @this)
	{
		/// <summary>
		/// Gets the <c><see cref="JsonNode"/></c>s that match the <paramref name="path"/>.
		/// <list type="bullet">
		/// <item>Allowed path separator: <c>.</c></item>
		/// <item>Use <c>$</c> to reference the root object or array (ie. <c>$.Customer.Addresses.City</c>).<br/>
		/// Not doing so will return an empty collection.</item>
		/// <item>Use name syntax (ie. <c>$.Customer.Addresses.City</c>) to select a particular property of a <c><see cref="JsonObject"/></c> by name.</item>
		/// <item>Use numerical positional syntax (ie. <c>$.Customer.Addresses.2</c>) to select a particular property of a <c><see cref="JsonObject"/></c> by position.</item>
		/// <item>You can use array syntax (ie. <c>$.Customer.Addresses[2]</c>) to select particular elements from a <c><see cref="JsonArray"/></c>.</item>
		/// <item>Use <c>*</c> by itself to match any <c><see cref="JsonNode"/></c>.</item>
		/// </list>
		/// </summary>
		/// <param name="path">
		/// A string that represents a path to a value or set of values from within the JSON DOM.<br/>
		/// Examples:
		/// <list type="bullet">
		/// <item><term><c>$.Customers.Addresses[2]</c></term> Get the third Address from each Customer's Addresses array.</item>
		/// <item><term><c>$.Customers.Advisors.Names.1</c></term> Get the second property of the Names object from each customer's Advisor.</item>
		/// <item><term><c>$.Customers.Advisors.Names.FirstName</c></term> Get the FirstName property from each customer's Advisor Names object.</item>
		/// </list>
		/// </param>
		/// <exception cref="FormatException"/>
		/// <exception cref="InvalidOperationException"/>
		/// <exception cref="OverflowException"/>
		/// <returns>All <c><see cref="JsonNode"/></c>s that match the <paramref name="path"/>.</returns>
		public JsonNode[] GetNodes(string path)
		{
			if (path.IsBlank)
				return [];

			var match = BRACKET_PATH.ToRegex().Match(path);
			if (!match.Success)
				match = DOT_PATH.ToRegex().Match(path);

			if (!match.Success)
				return [];

			Group? token;
			if (!match.Groups.TryGetValue(nameof(token), out token))
				return [];

			var tokens = token.Captures.Select(_ => _.Value).ToArray();

			return tokens.Length is 0 ? [@this] : getNodes(@this, tokens);

			static JsonNode[] getNodes(JsonNode? jsonNode, IEnumerable<string> tokens)
			{
				var token = tokens.First();
				tokens = tokens.Skip(1);

				if (token.EqualsIgnoreCase(ANY_NODE.ToString()))
				{
					var selectedNodes = jsonNode?.GetValueKind() switch
					{
						JsonValueKind.Object => jsonNode.AsObject().Select(pair => pair.Value).WhereNotNull().ToArray(),
						JsonValueKind.Array => jsonNode.AsArray().WhereNotNull().ToArray(),
						_ => []
					};
					return tokens.Any() ? selectedNodes.SelectMany(_ => getNodes(_, tokens)).ToArray() : selectedNodes;
				}

				if (jsonNode is JsonObject jsonObject)
				{
					var selectedNode = int.TryParse(token, InvariantCulture, out var i) ? jsonObject[i] ?? jsonObject[token] : jsonObject[token];
					if (selectedNode is null)
						return [];

					return tokens.Any() ? getNodes(selectedNode, tokens) : [selectedNode];
				}

				if (jsonNode is JsonArray jsonArray)
				{
					var match = ARRAY_REGEX.ToRegex().Match(token);
					if (!match.Success)
						return [];

					Group? index;
					if (!match.Groups.TryGetValue(nameof(index), out index) || index.Captures.Count is not 1)
						return [];

					var ranges = index.Captures[0].Value.SplitEx(',').Select(_ => _.ToRange()).WhereHasValue().ToArray();
					var jsonArrayNodes = ranges.SelectMany(jsonArray.Take).WhereNotNull().ToArray();

					return tokens.Any()
						? jsonArrayNodes.SelectMany(_ => getNodes(_, tokens)).ToArray()
						: jsonArrayNodes;
				}

				return [];
			}
		}

		public IDictionary<string, object?> ToDictionary()
		{
			var dictionary = new Dictionary<string, object?>();
			populate(dictionary, @this);

			return dictionary;

			static void populate(Dictionary<string, object?> dictionary, JsonNode jsonNode)
			{
				if (jsonNode is JsonObject jsonObject)
					jsonObject.ForEach(pair =>
					{
						if (pair.Value is not null)
							populate(dictionary, pair.Value);
						else
							dictionary.Add(Invariant($"{jsonNode.GetPath()}.{pair.Key}"), null);
					});
				else if (jsonNode is JsonArray jsonArray)
					jsonArray.ForEach((_, i) =>
					{
						if (_ is not null)
							populate(dictionary, _);
						else
							dictionary.Add(Invariant($"{jsonArray.GetPath()}[{i}]"), null);
					});
				else
					dictionary.Add(jsonNode.GetPath(), jsonNode.GetValue<object?>() switch
					{
						null => null,
						JsonElement jsonElement => jsonElement.GetValue(),
						var value => value
					});
			}
		}
	}

	extension(JsonArray @this)
	{
		/// <summary>
		/// =&gt; <see cref="JsonSerializer"/>.Deserialize&lt;<typeparamref name="T"/>[]?&gt;(@this, <paramref name="options"/>);
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T[]? ToArray<T>(JsonSerializerOptions? options = null)
			=> JsonSerializer.Deserialize<T[]?>(@this, options);
	}

	extension(JsonSerializerOptions @this)
	{
		/// <inheritdoc cref="JsonSerializerOptions.Converters"/>
		public JsonSerializerOptions AddConverter(JsonConverter converter)
		{
			@this.Converters.AddIfNotNull(converter);
			return @this;
		}

		/// <inheritdoc cref="JsonCommentHandling.Allow"/>
		public JsonSerializerOptions AllowComments()
		{
			@this.ReadCommentHandling = JsonCommentHandling.Allow;
			return @this;
		}

		/// <inheritdoc cref="JsonNumberHandling.AllowNamedFloatingPointLiterals"/>
		public JsonSerializerOptions AllowNamedFloatingPointLiterals()
		{
			@this.NumberHandling |= JsonNumberHandling.AllowNamedFloatingPointLiterals;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.AllowOutOfOrderMetadataProperties"/>
		public JsonSerializerOptions AllowOutOfOrderMetadataProperties()
		{
			@this.AllowOutOfOrderMetadataProperties = true;
			return @this;
		}

		/// <inheritdoc cref="JsonNumberHandling.AllowReadingFromString"/>
		public JsonSerializerOptions AllowReadingNumberFromString()
		{
			@this.NumberHandling |= JsonNumberHandling.AllowReadingFromString;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.AllowTrailingCommas"/>
		public JsonSerializerOptions AllowTrailingCommas()
		{
			@this.AllowTrailingCommas = true;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.DefaultBufferSize"/>
		public JsonSerializerOptions DefaultBufferSize(int bufferSize)
		{
			@this.DefaultBufferSize = bufferSize;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.DictionaryKeyPolicy"/>
		public JsonSerializerOptions DictionaryKeyNamingPolicy(JsonNamingPolicy policy)
		{
			@this.DictionaryKeyPolicy = policy;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.PropertyNamingPolicy"/>
		public JsonSerializerOptions DictionaryKeysAsCamelCase()
		{
			@this.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			return @this;
		}

		/// <inheritdoc cref="JsonNamingPolicy.KebabCaseLower"/>
		public JsonSerializerOptions DictionaryKeysAsKebabCaseLower()
		{
			@this.PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower;
			return @this;
		}

		/// <inheritdoc cref="JsonNamingPolicy.KebabCaseUpper"/>
		public JsonSerializerOptions DictionaryKeysAsKebabCaseUpper()
		{
			@this.PropertyNamingPolicy = JsonNamingPolicy.KebabCaseUpper;
			return @this;
		}

		/// <inheritdoc cref="JsonNamingPolicy.SnakeCaseLower"/>
		public JsonSerializerOptions DictionaryKeysAsSnakeCaseLower()
		{
			@this.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
			return @this;
		}

		/// <inheritdoc cref="JsonNamingPolicy.SnakeCaseUpper"/>
		public JsonSerializerOptions DictionaryKeysAsSnakeCaseUpper()
		{
			@this.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.Encoder"/>
		public JsonSerializerOptions Encoder(JavaScriptEncoder? encoder)
		{
			@this.Encoder = encoder;
			return @this;
		}

		/// <inheritdoc cref="JsonIgnoreCondition.WhenWritingDefault"/>
		public JsonSerializerOptions IgnoreDefaultValues()
		{
			@this.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
			return @this;
		}

		/// <inheritdoc cref="JsonIgnoreCondition.WhenWritingNull"/>
		public JsonSerializerOptions IgnoreNullValues()
		{
			@this.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			return @this;
		}

		/// <inheritdoc cref="JsonIgnoreCondition.Always"/>
		public JsonSerializerOptions IgnoreNullOrDefaultValues()
		{
			@this.DefaultIgnoreCondition = JsonIgnoreCondition.Always;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.IgnoreReadOnlyFields"/>
		public JsonSerializerOptions IgnoreReadOnlyFields()
		{
			@this.IgnoreReadOnlyFields = true;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.IgnoreReadOnlyProperties"/>
		public JsonSerializerOptions IgnoreReadOnlyProperties()
		{
			@this.IgnoreReadOnlyProperties = true;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.IncludeFields"/>
		public JsonSerializerOptions IncludeFields()
		{
			@this.IncludeFields = true;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.MaxDepth"/>
		public JsonSerializerOptions MaxDepth(int depth)
		{
			@this.MaxDepth = depth;
			return @this;
		}

		/// <inheritdoc cref="JsonObjectCreationHandling.Populate"/>
		public JsonSerializerOptions PreferObjectPopulating()
		{
			@this.PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate;
			return @this;
		}

		/// <inheritdoc cref="JsonObjectCreationHandling.Replace"/>
		public JsonSerializerOptions PreferObjectReplacing()
		{
			@this.PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.PropertyNamingPolicy"/>
		public JsonSerializerOptions PropertyNamesAsCamelCase()
		{
			@this.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			return @this;
		}

		/// <inheritdoc cref="JsonNamingPolicy.KebabCaseLower"/>
		public JsonSerializerOptions PropertyNamesAsKebabCaseLower()
		{
			@this.PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower;
			return @this;
		}

		/// <inheritdoc cref="JsonNamingPolicy.KebabCaseUpper"/>
		public JsonSerializerOptions PropertyNamesAsKebabCaseUpper()
		{
			@this.PropertyNamingPolicy = JsonNamingPolicy.KebabCaseUpper;
			return @this;
		}

		/// <inheritdoc cref="JsonNamingPolicy.SnakeCaseLower"/>
		public JsonSerializerOptions PropertyNamesAsSnakeCaseLower()
		{
			@this.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
			return @this;
		}

		/// <inheritdoc cref="JsonNamingPolicy.SnakeCaseUpper"/>
		public JsonSerializerOptions PropertyNamesAsSnakeCaseUpper()
		{
			@this.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.PropertyNamingPolicy"/>
		public JsonSerializerOptions PropertyNamingPolicy(JsonNamingPolicy policy)
		{
			@this.PropertyNamingPolicy = policy;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.PropertyNameCaseInsensitive"/>
		public JsonSerializerOptions ReadPropertyNamesAsCaseInsensitive()
		{
			@this.PropertyNameCaseInsensitive = true;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.RespectNullableAnnotations"/>
		public JsonSerializerOptions RespectNullableAnnotations()
		{
			@this.RespectNullableAnnotations = true;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.RespectRequiredConstructorParameters"/>
		public JsonSerializerOptions RespectRequiredConstructorParameters()
		{
			@this.RespectRequiredConstructorParameters = true;
			return @this;
		}

		/// <inheritdoc cref="JsonCommentHandling.Skip"/>
		public JsonSerializerOptions SkipComments()
		{
			@this.ReadCommentHandling = JsonCommentHandling.Skip;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.TypeInfoResolver"/>
		public JsonSerializerOptions TypeInfoResolver(IJsonTypeInfoResolver resolver)
		{
			@this.TypeInfoResolver = resolver;
			return @this;
		}

		/// <inheritdoc cref="JsonUnknownTypeHandling.JsonElement"/>
		public JsonSerializerOptions UnknownTypeAsJsonElement()
		{
			@this.UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement;
			return @this;
		}

		/// <inheritdoc cref="JsonUnknownTypeHandling.JsonNode"/>
		public JsonSerializerOptions UnknownTypeAsJsonNode()
		{
			@this.UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode;
			return @this;
		}

		/// <inheritdoc cref="JsonUnknownTypeHandling.JsonNode"/>
		public JsonSerializerOptions UnknownMembersNotAllowed()
		{
			@this.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
			return @this;
		}

		/// <inheritdoc cref="JsonSerializerOptions.WriteIndented"/>
		public JsonSerializerOptions WriteIndented(char? indentCharacter = null, int? indentSize = null, string? newLine = null)
		{
			@this.WriteIndented = true;
			if (indentCharacter.HasValue)
				@this.IndentCharacter = indentCharacter.Value;

			if (indentSize.HasValue)
				@this.IndentSize = indentSize.Value;

			if (newLine is not null)
				@this.NewLine = newLine;

			return @this;
		}

		/// <inheritdoc cref="JsonNumberHandling.WriteAsString"/>
		public JsonSerializerOptions WriteNumberAsString()
		{
			@this.NumberHandling |= JsonNumberHandling.WriteAsString;
			return @this;
		}
	}

	extension(ref Utf8JsonReader @this)
	{
		public object? GetValue()
			=> @this.TokenType switch
			{
				JsonTokenType.True => true,
				JsonTokenType.False => false,
				JsonTokenType.Number when @this.TryGetInt32(out var value) => value,
				JsonTokenType.Number when @this.TryGetInt64(out var value) => value,
				JsonTokenType.Number => @this.GetDecimal(),
				JsonTokenType.String when @this.TryGetDateTime(out var value) => value,
				JsonTokenType.String when @this.TryGetDateTimeOffset(out var value) => value,
				JsonTokenType.String when @this.TryGetGuid(out var value) => value,
				JsonTokenType.String => @this.GetString(),
				_ => null
			};

		public void ReadToEnd()
		{
			while (@this.Read()) { }
		}

		public bool ReadUntil(JsonTokenType tokenType)
		{
			while (@this.Read())
				if (@this.TokenType == tokenType)
					return true;

			return false;
		}
	}

	extension(Utf8JsonWriter @this)
	{
		public void WriteValue(object? value, JsonSerializerOptions options)
		{
			Action writeValue = value switch
			{
				null or DBNull => @this.WriteNullValue,
				bool success => () => @this.WriteBooleanValue(success),
				sbyte number => () => @this.WriteNumberValue((int)number),
				short number => () => @this.WriteNumberValue((int)number),
				int number => () => @this.WriteNumberValue(number),
				long number => () => @this.WriteNumberValue(number),
				Int128 number => () => @this.WriteNumberValue((decimal)number),
				BigInteger number => () => @this.WriteNumberValue((decimal)number),
				nint number => () => @this.WriteNumberValue((long)number),
				byte number => () => @this.WriteNumberValue((uint)number),
				ushort number => () => @this.WriteNumberValue((uint)number),
				uint number => () => @this.WriteNumberValue(number),
				ulong number => () => @this.WriteNumberValue(number),
				UInt128 number => () => @this.WriteNumberValue((decimal)number),
				nuint number => () => @this.WriteNumberValue((ulong)number),
				Half number => () => @this.WriteNumberValue((float)number),
				float number => () => @this.WriteNumberValue(number),
				double number => () => @this.WriteNumberValue(number),
				decimal number => () => @this.WriteNumberValue(number),
				DateOnly dateOnly => () => @this.WriteStringValue(dateOnly.ToISO8601()),
				DateTime dateTime => () => @this.WriteStringValue(dateTime.ToISO8601()),
				DateTimeOffset dateTimeOffset => () => @this.WriteStringValue(dateTimeOffset.ToISO8601()),
				TimeOnly timeOnly => () => @this.WriteStringValue(timeOnly.ToISO8601()),
				TimeSpan timeSpan => () => @this.WriteStringValue(timeSpan.ToText()),
				Guid id => () => @this.WriteStringValue(id),
				char or Index or Range or Uri => () => @this.WriteStringValue(value.ToString()),
				string text => () => @this.WriteStringValue(text),
				_ => () => JsonSerializer.Serialize(@this, value, options)
			};
			writeValue();
		}
	}
}
