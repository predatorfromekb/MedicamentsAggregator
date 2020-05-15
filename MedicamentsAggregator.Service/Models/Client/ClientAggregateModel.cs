using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Client
{
    public class ClientAggregateModel
    {
        [JsonProperty("medicaments")]
        public ClientMedicamentModel[] Medicaments { get; set; }
        [JsonProperty("settings")]
        public AggregateSettings Settings { get; set; }
    }
}