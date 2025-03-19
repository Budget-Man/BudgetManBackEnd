using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Model.Request
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EdenAiPayload
    {
        public string? query { get; set; }
        public string? llm_provider { get; set; }
        public string? llm_model { get; set; }
        public int k { get; set; }
        public FilterDocuments? filter_documents { get; set; }
        public int max_tokens { get; set; }
        public double temperature { get; set; }
        public string? chatbot_global_action { get; set; }
        public string? conversation_id { get; set; }
    }
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class FilterDocuments
    {
        [JsonPropertyName("metadata1")]
        public string? metadata1 { get; set; }
    }
}
