using Newtonsoft.Json;

namespace MedicamentsAggregator.Service.Models.Response
{
    public class ResponseMedicamentModel
    {
        public ResponseMedicamentModel(int id, string title, double price)
        {
            Id = id;
            Title = title;
            Price = price;
        }

        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("price")]
        public double Price { get; }
    }
}