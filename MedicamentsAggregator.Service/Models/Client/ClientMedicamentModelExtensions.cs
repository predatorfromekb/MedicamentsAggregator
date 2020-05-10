using MedicamentsAggregator.Service.DataLayer.Tables;

namespace MedicamentsAggregator.Service.Models.Client
{
    public static class ClientMedicamentModelExtensions
    {
        public static Medicament ToMedicament(this ClientMedicamentModel model)
        {
            return new Medicament
            {
                Id = model.Id,
                Title = model.Title,
                Url = model.Url
            };
        }
    }
}