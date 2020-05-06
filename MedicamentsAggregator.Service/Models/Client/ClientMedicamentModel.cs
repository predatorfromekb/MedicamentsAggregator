using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Client
{
    public class ClientMedicamentModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}