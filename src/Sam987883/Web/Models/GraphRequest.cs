// Copyright (c) 2020 Samuel Abraham

using System.Text.Json.Serialization;

namespace Sam987883.Web.Models
{
    public class GraphRequest
    {
        [JsonPropertyName("operationName")]
        public string OperationName { get; set; } = string.Empty;

        [JsonPropertyName("query")]
        public string Query { get; set; } = string.Empty;

        [JsonPropertyName("variables")]
        public object Variables { get; set; } = new object();
    }
}
