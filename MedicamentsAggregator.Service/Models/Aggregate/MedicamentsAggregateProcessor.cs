using System;
using System.Linq;
using System.Threading.Tasks;
using MedicamentsAggregator.Service.DataLayer;
using MedicamentsAggregator.Service.DataLayer.Tables;
using MedicamentsAggregator.Service.Models.Client;
using MedicamentsAggregator.Service.Models.Common;
using MedicamentsAggregator.Service.Models.Medgorodok;

namespace MedicamentsAggregator.Service.Models.Aggregate
{
    public class MedicamentsAggregateProcessor
    {
        private readonly MedgorodokMedicamentPageParser _medgorodokMedicamentPageParser;
        private readonly MedicamentsAggregatorContextFactory _medicamentsAggregatorContextFactory;
        private readonly Repository _repository;

        public MedicamentsAggregateProcessor(
            MedgorodokMedicamentPageParser medgorodokMedicamentPageParser,
            MedicamentsAggregatorContextFactory medicamentsAggregatorContextFactory,
            Repository repository
        )
        {
            _medgorodokMedicamentPageParser = medgorodokMedicamentPageParser;
            _medicamentsAggregatorContextFactory = medicamentsAggregatorContextFactory;
            _repository = repository;
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
            var entities  = _repository
                .AddOrUpdate(medicaments,UpdatingRules.ForMedicament);
            
            var listOfTasks = entities.AllEntities
                .Select(medicament => _medgorodokMedicamentPageParser.Parse(medicament));
            
            var parsingResult = await Task.WhenAll(listOfTasks);
            
            UpdatePharmacies(parsingResult);
            await UpdateLinks(parsingResult);

            return  new MedicamentsAggregateResultModel
            {
                Count =223,
            };
        }

        private void UpdatePharmacies(MedgorodokMedicamentModel[] models)
        {
            foreach (var model in models)
            {
                var pharmacies = model.Pharmacies
                    .Select(e => e.ToPharmacy())
                    .ToArray();
                _repository.AddOrUpdate(pharmacies, UpdatingRules.ForPharmacy);
            }
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
                context.RemoveRange(currentMedicamentLinks);
                context.AddRange(links);
                await context.SaveChangesAsync();
            }

        }
    }
}