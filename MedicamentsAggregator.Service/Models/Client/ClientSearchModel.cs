using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Client
{
    public class ClientSearchModel
    {
        [JsonProperty("medicaments")]
        public ClientMedicamentModel[] Medicaments { get; set; }
    }
}