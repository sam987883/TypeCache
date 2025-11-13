// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL;
using GraphQL.Execution;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Converters;

public sealed class GraphQLExecutionResultJsonConverter : JsonConverter<ExecutionResult>
{
	public override bool CanConvert(Type typeToConvert) => typeof(ExecutionResult).IsAssignableFrom(typeToConvert);

	/// <exception cref="NotImplementedException"></exception>
	public override ExecutionResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> throw new NotImplementedException();

	public override void Write(Utf8JsonWriter writer, ExecutionResult? value, JsonSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteStartObject();

		if (value.Errors?.Any() is true)
		{
			writer.WritePropertyName("errors");

			writer.WriteStartArray();
			value.Errors.ForEach(error => JsonSerializer.Serialize(writer, error, options));
			writer.WriteEndArray();
		}

		if (value.Executed)
		{
			writer.WritePropertyName("data");

			if (value.Data is ExecutionNode executionNode)
				Write(writer, executionNode, options);
			else
				JsonSerializer.Serialize(writer, value.Data, options);
		}

		if (value.Extensions?.Any() is true)
		{
			writer.WritePropertyName("extensions");

			JsonSerializer.Serialize(writer, value.Extensions, options);
		}

		writer.WriteEndObject();
	}

	private static void Write(Utf8JsonWriter writer, ExecutionNode executionNode, JsonSerializerOptions options)
	{
		switch (executionNode)
		{
			case ValueExecutionNode valueExecutionNode:
				JsonSerializer.Serialize(writer, valueExecutionNode.ToValue(), options);
				break;
			case ObjectExecutionNode objectExecutionNode when objectExecutionNode.SubFields is not null:
				writer.WriteStartObject();
				objectExecutionNode.SubFields.ForEach(childNode =>
				{
					var propertyName = options.PropertyNamingPolicy?.ConvertName(childNode.Name!) ?? childNode.Name!;
					writer.WritePropertyName(propertyName);
					Write(writer, childNode, options);
				});
				writer.WriteEndObject();
				break;
			case ArrayExecutionNode arrayExecutionNode when arrayExecutionNode.Items is not null:
				writer.WriteStartArray();
				arrayExecutionNode.Items.ForEach(childNode => Write(writer, childNode, options));
				writer.WriteEndArray();
				break;
			case ArrayExecutionNode arrayExecutionNode when arrayExecutionNode.SerializedResult is not null:
				JsonSerializer.Serialize(writer, arrayExecutionNode.SerializedResult, options);
				break;
			case ArrayExecutionNode arrayExecutionNode:
			case ObjectExecutionNode:
			case NullExecutionNode:
			case null:
				writer.WriteNullValue();
				break;
			default:
				JsonSerializer.Serialize(writer, executionNode.ToValue(), options);
				break;
		}
	}
}
