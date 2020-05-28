using System.Collections.Generic;
using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Response
{
    public class ResponseCoordinateModel
    {
        public ResponseCoordinateModel(List<ResponsePharmacyModel> pharmacies, double latitude, double longitude, string address)
        {
            Pharmacies = pharmacies;
            Latitude = latitude;
            Longitude = longitude;
            Address = address;
        }

        [JsonProperty("pharmacies")]
        public List<ResponsePharmacyModel> Pharmacies { get; }
        
        [JsonProperty("latitude")]
        public double Latitude { get; }

        [JsonProperty("longitude")]
        public double Longitude { get; }
        
        [JsonProperty("address")]
        public string Address { get; }
    }
}