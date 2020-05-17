using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Client
{
    public class AggregateSettings
    {
        [JsonProperty("searchRadius")]
        public int SearchRadius { get; set; }
        
        [JsonProperty("useSearchRadius")]
        public bool UseSearchRadius { get; set; }
        
        [JsonProperty("pharmaciesCount")]
        public int PharmaciesCount { get; set; }
        
        [JsonProperty("limitedPharmaciesCount")]
        public bool LimitedPharmaciesCount { get; set; }
        
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
}