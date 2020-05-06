using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models
{
    public class SearchModel
    {
        [JsonProperty("medicaments")]
        public Medicament[] Medicaments { get; set; }
    }
}