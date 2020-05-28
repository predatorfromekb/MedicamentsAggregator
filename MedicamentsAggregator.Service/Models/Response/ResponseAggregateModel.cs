using System.Collections.Generic;
using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Response
{
    public class ResponseAggregateModel
    {
        public ResponseAggregateModel(List<ResponseCoordinateModel> coordinates)
        {
            Coordinates = coordinates;
        }

        [JsonProperty("coordinates")]
        public List<ResponseCoordinateModel> Coordinates { get; }
    }
}