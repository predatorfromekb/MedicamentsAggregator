using System.Collections.Generic;
using MedicamentsAggregator.Service.Models.Client;
using MedicamentsAggregator.Service.Models.Medgorodok;

namespace MedicamentsAggregator.Service.Models.Search
{
    public class MedicamentsSearchProcessor
    {
        private readonly MedgorodokMedicamentPageParser _medgorodokMedicamentPageParser;

        public MedicamentsSearchProcessor(MedgorodokMedicamentPageParser medgorodokMedicamentPageParser)
        {
            _medgorodokMedicamentPageParser = medgorodokMedicamentPageParser;
        }

        public MedicamentsSearchResultModel Process(ClientSearchModel clientSearchModel)
        {
            var list = new List<MedgorodokMedicamentModel>();
            foreach (var medicament in clientSearchModel.Medicaments)
            {
                list.Add(_medgorodokMedicamentPageParser.Parse(medicament));
            }
            return new MedicamentsSearchResultModel();
        }
    }
}