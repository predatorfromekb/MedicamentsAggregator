using System;
using System.Linq;
using System.Threading.Tasks;
using MedicamentsAggregator.Service.DataLayer;
using MedicamentsAggregator.Service.DataLayer.Context;
using MedicamentsAggregator.Service.DataLayer.Tables;
using MedicamentsAggregator.Service.Models.Common;
using MedicamentsAggregator.Service.Models.Extensions;
using MedicamentsAggregator.Service.Models.GeoCoder;
using MedicamentsAggregator.Service.Models.Medgorodok;
using MedicamentsAggregator.Service.Models.Request;
using MedicamentsAggregator.Service.Models.Response;

namespace MedicamentsAggregator.Service.Models.Aggregate
{
    public class AggregateProcessor
    {
        private readonly MedgorodokMedicamentPageParser _medgorodokMedicamentPageParser;
        private readonly MedicamentsAggregatorContextFactory _medicamentsAggregatorContextFactory;
        private readonly Repository _repository;
        private readonly PharmacyCoordinatesUpdater _pharmacyCoordinatesUpdater;
        private readonly Aggregator _aggregator;

        public AggregateProcessor(
            MedgorodokMedicamentPageParser medgorodokMedicamentPageParser,
            MedicamentsAggregatorContextFactory medicamentsAggregatorContextFactory,
            Repository repository,
            PharmacyCoordinatesUpdater pharmacyCoordinatesUpdater,
            Aggregator aggregator
        )
        {
            _medgorodokMedicamentPageParser = medgorodokMedicamentPageParser;
            _medicamentsAggregatorContextFactory = medicamentsAggregatorContextFactory;
            _repository = repository;
            _pharmacyCoordinatesUpdater = pharmacyCoordinatesUpdater;
            _aggregator = aggregator;
        }

        public async Task<ResponseAggregateModel> Process(RequestAggregateModel requestAggregateModel)
        {
            // Надо
            var countById = requestAggregateModel.Medicaments
                .ToDictionary(e => e.Id, e => e.Count);
            var medicamentsFromClient = requestAggregateModel.Medicaments.ToArray();

            await UpdateData(medicamentsFromClient);

            return await _aggregator.Aggregate(requestAggregateModel);
        }
        
        private async Task UpdateData(RequestMedicamentModel[] medicamentsFromClient)
        {
            var medicaments = medicamentsFromClient
                .Select(e => e.ToMedicament())
                .ToArray();
            var entities  = await _repository
                .AddOrUpdate(medicaments,UpdatingRules.ForMedicament);
            
            var listOfTasks = entities.AllEntities
                .Select(medicament => _medgorodokMedicamentPageParser.Parse(medicament));
            
            var parsingResult = await Task.WhenAll(listOfTasks);
            
            var pharmacies = await UpdatePharmacies(parsingResult);
            await UpdateLinks(parsingResult);
            
            var insertedPharmacies = pharmacies.InsertedEntities;

            await _pharmacyCoordinatesUpdater.UpdateCoordinates(insertedPharmacies);
        }

        private async Task<EntityContainer<Pharmacy>> UpdatePharmacies(MedgorodokMedicamentModel[] models)
        {
            var pharmacies = UnionPharmacies(models)
                .Select(e => e.TryGetCityFromTitle())
                .Select(e => e.RemoveBracketsFromTitle())
                .ToArray();
            return await _repository.AddOrUpdate(pharmacies, UpdatingRules.ForPharmacy);
        }

        private Pharmacy[] UnionPharmacies(MedgorodokMedicamentModel[] models)
        {
            return models
                .Aggregate(
                    Enumerable.Empty<Pharmacy>(), 
                    (list, model) => list
                        .Union<Pharmacy>(
                            model.Pharmacies.Select(e => e.ToPharmacy()),
                            EntityEqualityComparer.Instance)
                    )
                .ToArray();
        }
        
        private async Task UpdateLinks(MedgorodokMedicamentModel[] models)
        {
            var tasks = models.Select(UpdateLinkOfOneMedicament);
            await Task.WhenAll(tasks);
        }
        
        private async Task UpdateLinkOfOneMedicament(MedgorodokMedicamentModel model)
        {
            var now = DateTime.Now;
            
            var medicamentId = model.Medicament.Id;

            var links = model.Pharmacies.Select(pharmacy => new PharmacyMedicamentLink
            {
                PharmacyId = pharmacy.Id,
                MedicamentId = medicamentId,
                Price = pharmacy.Price,
                UpdatedDate = now
            });

            using (var context = _medicamentsAggregatorContextFactory.CreateContext())
            {
                var currentMedicamentLinks = context.Set<PharmacyMedicamentLink>()
                    .Where(e => e.MedicamentId == medicamentId);

                var addTask = context.AddRangeAsync(links);
                context.RemoveRange(currentMedicamentLinks);
                await addTask;
                await context.SaveChangesAsync();
            }

        }
    }
}