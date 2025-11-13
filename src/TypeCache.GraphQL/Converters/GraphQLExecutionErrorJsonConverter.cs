// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Converters;

public sealed class GraphQLExecutionErrorJsonConverter : JsonConverter<ExecutionError>
{
	public override bool CanConvert(Type typeToConvert) => typeof(ExecutionError).IsAssignableFrom(typeToConvert);

	/// <exception cref="NotImplementedException"></exception>
	public override ExecutionError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> throw new NotImplementedException();

	public override void Write(Utf8JsonWriter writer, ExecutionError? error, JsonSerializerOptions options)
	{
		if (error is null)
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteStartObject();

		if (error.Code.IsNotBlank)
			writer.WriteString("code", error.Code);

		if (error.Message.IsNotBlank)
			writer.WriteString("message", error.Message);

		if (error.Source.IsNotBlank)
			writer.WriteString("source", error.Source);

		if (error.HelpLink.IsNotBlank)
			writer.WriteString("help-link", error.HelpLink);

		if (error.Path?.Any() is true)
		{
			writer.WritePropertyName("path");

			writer.WriteStartArray();
			error.Path.ForEach(_ => writer.WriteValue(_, options));
			writer.WriteEndArray();
		}

		if (error.Data.Count > 0)
		{
			writer.WritePropertyName("data");

			writer.WriteStartObject();
			error.Data.OfType<DictionaryEntry>().ForEach(_ =>
			{
				writer.WritePropertyName(_.Key.ToString()!);
				writer.WriteValue(_.Value, options);
			});
			writer.WriteEndObject();
		}

		if (error.Locations?.Count > 0)
		{
			writer.WritePropertyName("locations");

			writer.WriteStartArray();
			error.Locations.ForEach(_ =>
			{
				writer.WriteStartObject();
				writer.WriteNumber("line", _.Line);
				writer.WriteNumber("column", _.Column);
				writer.WriteEndObject();
			});
			writer.WriteEndArray();
		}

		if (error.StackTrace.IsNotBlank)
		{
			writer.WritePropertyName("stack-trace");

			writer.WriteStartArray();
			error.StackTrace.SplitEx(Environment.NewLine).ForEach(writer.WriteStringValue);
			writer.WriteEndArray();
		}

		if (error.Extensions?.Count > 0)
		{
			writer.WritePropertyName("extensions");

			writer.WriteStartObject();
			error.Extensions.ForEach(_ =>
			{
				writer.WritePropertyName(_.Key);
				writer.WriteValue(_.Value, options);
			});
			writer.WriteEndObject();
		}

		var innerError = error.InnerException;
		if (innerError is not null)
		{
			writer.WritePropertyName("inner-errors");

			writer.WriteStartArray();
			do
			{
				Write(writer, innerError);
				innerError = innerError.InnerException;
			}
			while (innerError is not null);
			writer.WriteEndArray();
		}

		writer.WriteEndObject();
	}

	private static void Write(Utf8JsonWriter writer, Exception error)
	{
		writer.WriteStartObject();

		writer.WriteString("type", error.GetType().CodeName);

		if (error.Message.IsNotBlank)
			writer.WriteString("message", error.Message);

		if (error.StackTrace.IsNotBlank)
		{
			writer.WritePropertyName("stack-trace");
			writer.WriteStartArray();

			error.StackTrace.SplitEx(Environment.NewLine).ForEach(writer.WriteStringValue);

			writer.WriteEndArray();
		}

		writer.WriteEndObject();
	}
}
