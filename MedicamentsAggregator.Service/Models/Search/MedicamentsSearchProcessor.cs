using System.Linq;
using System.Threading.Tasks;
using MedicamentsAggregator.Service.Models.Client;
using MedicamentsAggregator.Service.Models.Medgorodok;

namespace MedicamentsAggregator.Service.Models.Search
{
    public class MedicamentsSearchProcessor
    {
        private readonly MedgorodokMedicamentPageParser _medgorodokMedicamentPageParser;

        public MedicamentsSearchProcessor(
            MedgorodokMedicamentPageParser medgorodokMedicamentPageParser
            )
        {
            _medgorodokMedicamentPageParser = medgorodokMedicamentPageParser;
        }

        public async Task<MedicamentsSearchResultModel> Process(ClientSearchModel clientSearchModel)
        {
            var listOfTasks = clientSearchModel.Medicaments
                .Select(medicament => _medgorodokMedicamentPageParser.Parse(medicament));
            var result = await Task.WhenAll(listOfTasks);
            return  new MedicamentsSearchResultModel
            {
                Count = result.Sum(e => e.Count)
            };
        }
    }
}