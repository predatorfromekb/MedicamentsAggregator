using System;
using System.Linq;
using System.Threading.Tasks;
using MedicamentsAggregator.Service.DataLayer;
using MedicamentsAggregator.Service.DataLayer.Tables;
using MedicamentsAggregator.Service.Models.Client;
using MedicamentsAggregator.Service.Models.Common;
using MedicamentsAggregator.Service.Models.GeoCoder;
using MedicamentsAggregator.Service.Models.Medgorodok;

namespace MedicamentsAggregator.Service.Models.Aggregate
{
    public class MedicamentsAggregateProcessor
    {
        private readonly MedgorodokMedicamentPageParser _medgorodokMedicamentPageParser;
        private readonly MedicamentsAggregatorContextFactory _medicamentsAggregatorContextFactory;
        private readonly Repository _repository;
        private readonly PharmacyCoordinatesUpdater _pharmacyCoordinatesUpdater;

        public MedicamentsAggregateProcessor(
            MedgorodokMedicamentPageParser medgorodokMedicamentPageParser,
            MedicamentsAggregatorContextFactory medicamentsAggregatorContextFactory,
            Repository repository,
            PharmacyCoordinatesUpdater pharmacyCoordinatesUpdater
        )
        {
            _medgorodokMedicamentPageParser = medgorodokMedicamentPageParser;
            _medicamentsAggregatorContextFactory = medicamentsAggregatorContextFactory;
            _repository = repository;
            _pharmacyCoordinatesUpdater = pharmacyCoordinatesUpdater;
        }

        public async Task<MedicamentsAggregateResultModel> Process(ClientAggregateModel clientAggregateModel)
        {
            // Надо
            var countById = clientAggregateModel.Medicaments
                .ToDictionary(e => e.Id, e => e.Count);
            var medicamentsFromClient = clientAggregateModel.Medicaments.ToArray();

            return await Process(medicamentsFromClient);
        }
        
        private async Task<MedicamentsAggregateResultModel> Process(ClientMedicamentModel[] medicamentsFromClient)
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

            return  new MedicamentsAggregateResultModel
            {
                Count =223,
            };
        }

        private async Task<EntityContainer<Pharmacy>> UpdatePharmacies(MedgorodokMedicamentModel[] models)
        {
            var pharmacies = UnionPharmacies(models);
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