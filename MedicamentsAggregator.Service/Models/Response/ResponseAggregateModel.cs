using System.Collections.Generic;
using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Response
{
    public class ResponseAggregateModel
    {
        public ResponseAggregateModel(List<ResponsePharmacyModel> pharmacies)
        {
            Pharmacies = pharmacies;
        }

        [JsonProperty("pharmacies")]
        public List<ResponsePharmacyModel> Pharmacies { get; }

        [JsonProperty("count")] 
        public int Count => Pharmacies.Count;
    }
}