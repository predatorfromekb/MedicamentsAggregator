namespace MedicamentsAggregator.Service.Models.Medgorodok
{
    public class MedgorodokPharmacyModel
    {
        public MedgorodokPharmacyModel(int id, string title, string address, double price)
        {
            Id = id;
            Title = title;
            Address = address;
            Price = price;
        }

        public int Id { get; }
        public string Title { get; }
        public string Address { get; }
        public double Price { get; }
    }
}