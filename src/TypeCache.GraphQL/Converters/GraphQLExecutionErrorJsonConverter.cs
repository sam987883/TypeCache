// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Converters;

public sealed class GraphQLExecutionErrorJsonConverter : JsonConverter<ExecutionError>
{
	/// <exception cref="NotImplementedException"></exception>
	public override ExecutionError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> throw new NotImplementedException();

	public override void Write(Utf8JsonWriter writer, ExecutionError? value, JsonSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteStartObject();
		writer.WriteString("message", value.Message);

		if (value.Locations is not null)
		{
			writer.WritePropertyName("locations");
			JsonSerializer.Serialize(writer, value.Locations, options);
		}

		if (value.Path?.Any() is true)
		{
			writer.WritePropertyName("path");
			JsonSerializer.Serialize(writer, value.Path, options);
		}

		if (value.Extensions?.Any() is true)
		{
			writer.WritePropertyName("extensions");
			JsonSerializer.Serialize(writer, value.Extensions, options);
		}

		writer.WriteEndObject();
	}
}
