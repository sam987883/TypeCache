using System.Text.Json.Serialization;

namespace sam987883.Web
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
