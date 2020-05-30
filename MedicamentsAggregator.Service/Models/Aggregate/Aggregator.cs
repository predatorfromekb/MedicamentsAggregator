using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicamentsAggregator.Service.DataLayer;
using MedicamentsAggregator.Service.DataLayer.Context;
using MedicamentsAggregator.Service.DataLayer.Tables;
using MedicamentsAggregator.Service.Models.Extensions;
using MedicamentsAggregator.Service.Models.Request;
using MedicamentsAggregator.Service.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace MedicamentsAggregator.Service.Models.Aggregate
{
    public class Aggregator
    {
        private readonly MedicamentsAggregatorContextFactory _contextFactory;

        public Aggregator(MedicamentsAggregatorContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ResponseAggregateModel> Aggregate(RequestAggregateModel requestAggregateModel)
        {
            var data = GetData(requestAggregateModel);
            var pharmaciesList = new Dictionary<int, ResponsePharmacyModel>();
            double totalPrice = 0;
            foreach (var medicament in requestAggregateModel.Medicaments)
            {
                var cheapestLink = data
                    .Where(e => e.MedicamentId == medicament.Id)
                    .OrderBy(e => e.Price)
                    .FirstOrDefault();
                if (cheapestLink == null)
                    continue;
                if (!pharmaciesList.ContainsKey(cheapestLink.PharmacyId))
                {
                    var pharmacy = cheapestLink.Pharmacy;
                    var model = new ResponsePharmacyModel(pharmacy.Id, pharmacy.Title, pharmacy.FormattedAddress, 
                        pharmacy.Latitude.Value, pharmacy.Longitude.Value, new List<ResponseMedicamentModel>());
                    pharmaciesList.Add(cheapestLink.PharmacyId, model);
                }
                pharmaciesList[cheapestLink.PharmacyId].Medicaments.Add(new ResponseMedicamentModel(cheapestLink.MedicamentId, medicament.Title, cheapestLink.Price, medicament.Count));
                totalPrice += cheapestLink.Price * medicament.Count;
            }

            var coordinates = pharmaciesList.Values
                .GroupBy(e => (e.Latitude, e.Longitude, e.Address),
                    (e, i) => new ResponseCoordinateModel(i.ToList(), e.Latitude, e.Longitude, e.Address))
                .ToList();
            return new ResponseAggregateModel(coordinates, totalPrice);
        }

        private PharmacyMedicamentLink[] GetData(RequestAggregateModel requestAggregateModel)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var ids = requestAggregateModel.Medicaments.Select(e => e.Id).ToArray();
                return context
                    .Set<PharmacyMedicamentLink>()
                    .Where(e => ids.Contains(e.MedicamentId))
                    .Include(e => e.Pharmacy)
                    .Where(e => e.Pharmacy.Latitude != null && e.Pharmacy.Longitude != null)
                    .Include(e => e.Medicament)
                    .FilterByRange(requestAggregateModel.Settings)
                    .Where(e => !(e.Pharmacy.Latitude == 56.838011 && e.Pharmacy.Longitude == 60.597465))
                    .ToArray();
            }
        }
    }
}