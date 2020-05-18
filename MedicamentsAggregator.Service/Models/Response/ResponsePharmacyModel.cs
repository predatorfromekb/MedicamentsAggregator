using System.Collections.Generic;
using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Response
{
    public class ResponsePharmacyModel
    {
        public ResponsePharmacyModel(int id, string title, string address, double latitude, double longitude, List<ResponseMedicamentModel> medicaments)
        {
            Id = id;
            Title = title;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            Medicaments = medicaments;
        }

        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("address")]
        public string Address { get; }

        [JsonProperty("latitude")]
        public double Latitude { get; }

        [JsonProperty("longitude")]
        public double Longitude { get; }
        
        [JsonProperty("medicaments")]
        public List<ResponseMedicamentModel> Medicaments { get; }
    }
}