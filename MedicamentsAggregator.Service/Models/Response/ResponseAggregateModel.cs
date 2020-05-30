using System.Collections.Generic;
using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Response
{
    public class ResponseAggregateModel
    {
        public ResponseAggregateModel(List<ResponseCoordinateModel> coordinates, double totalPrice)
        {
            Coordinates = coordinates;
            TotalPrice = totalPrice;
        }

        [JsonProperty("coordinates")]
        public List<ResponseCoordinateModel> Coordinates { get; }
        
        [JsonProperty("totalPrice")]
        public double TotalPrice { get; }
    }
}