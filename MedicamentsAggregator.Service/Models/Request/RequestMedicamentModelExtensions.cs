using MedicamentsAggregator.Service.DataLayer.Tables;

namespace MedicamentsAggregator.Service.Models.Request
{
    public static class RequestMedicamentModelExtensions
    {
        public static Medicament ToMedicament(this RequestMedicamentModel model)
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