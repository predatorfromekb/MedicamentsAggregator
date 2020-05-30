using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Response
{
    public class ResponseMedicamentModel
    {
        public ResponseMedicamentModel(int id, string title, double price, int count)
        {
            Id = id;
            Title = title;
            Price = price;
            Count = count;
        }

        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("price")]
        public double Price { get; }
        
        [JsonProperty("count")]
        public int Count { get; }
    }
}