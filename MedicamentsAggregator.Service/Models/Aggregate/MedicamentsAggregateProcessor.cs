using System.Linq;
using System.Threading.Tasks;
using MedicamentsAggregator.Service.Models.Client;
using MedicamentsAggregator.Service.Models.Medgorodok;

namespace MedicamentsAggregator.Service.Models.Aggregate
{
    public class MedicamentsAggregateProcessor
    {
        private readonly MedgorodokMedicamentPageParser _medgorodokMedicamentPageParser;

        public MedicamentsAggregateProcessor(
            MedgorodokMedicamentPageParser medgorodokMedicamentPageParser
            )
        {
            _medgorodokMedicamentPageParser = medgorodokMedicamentPageParser;
        }

        public async Task<MedicamentsAggregateResultModel> Process(ClientAggregateModel clientAggregateModel)
        {
            var listOfTasks = clientAggregateModel.Medicaments
                .Select(medicament => _medgorodokMedicamentPageParser.Parse(medicament));
            var result = await Task.WhenAll(listOfTasks);
            return  new MedicamentsAggregateResultModel
            {
                Count = result.Sum(e => e.Count)
            };
        }
    }
}