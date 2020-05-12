namespace MedicamentsAggregator.Service.Models.Medgorodok
{
    public class MedgorodokPharmacyModel
    {
        public MedgorodokPharmacyModel(int id, string title, string address, double price, string city)
        {
            Id = id;
            Title = title;
            Address = address;
            Price = price;
            City = city;
        }

        public int Id { get; }
        public string Title { get; }
        public string Address { get; }
        public double Price { get; }
        public string City { get; }
    }
}