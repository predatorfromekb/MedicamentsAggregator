using MedicamentsAggregator.Service.DataLayer.Tables;

namespace MedicamentsAggregator.Service.Models.Medgorodok
{
    public static class MedgorodokPharmacyModelExtensions
    {
        public static Pharmacy ToPharmacy(this MedgorodokPharmacyModel model)
        {
            return new Pharmacy
            {
                Address = model.Address,
                Id = model.Id,
                Title = model.Title,
                City = model.City
            };
        }
    }
}