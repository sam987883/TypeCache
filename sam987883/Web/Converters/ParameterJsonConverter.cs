// Copyright (c) 2020 Samuel Abraham

using sam987883.Database.Requests;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;

namespace sam987883.Web.Middleware
{
	public class ParameterJsonConverter : JsonConverter<Parameter>
	{
        public override Parameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var parameter = JsonSerializer.Deserialize<Parameter>(ref reader, null);
			return parameter.Value is JsonElement json ? new Parameter
            {
                Name = parameter.Name,
                Value = json.ValueKind switch
                {
                    JsonValueKind.Undefined => throw new ArgumentException($"Invalid value for [{nameof(Parameter)}] specified in [{nameof(SelectRequest)}]: {parameter.Value}.", parameter.Name),
                    JsonValueKind.Object => json.ToString(),
                    JsonValueKind.Array => json.ToString(),
                    JsonValueKind.String => json.GetString(),
                    JsonValueKind.Number => json.TryGetInt64(out var value) ? value : json.GetDecimal(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => throw new NotImplementedException()
                }
            } : parameter;
		}

		public override void Write(Utf8JsonWriter writer, Parameter value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value, null);
	}
}
