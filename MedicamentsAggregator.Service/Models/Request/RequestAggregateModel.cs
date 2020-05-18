using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Request
{
    public class RequestAggregateModel
    {
        [JsonProperty("medicaments")]
        public RequestMedicamentModel[] Medicaments { get; set; }
        
        [JsonProperty("settings")]
        public RequestAggregateSettings Settings { get; set; }
    }
}