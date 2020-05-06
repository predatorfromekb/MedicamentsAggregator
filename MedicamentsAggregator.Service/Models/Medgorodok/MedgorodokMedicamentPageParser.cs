using System.Threading;
using MedicamentsAggregator.Service.Models.Client;

namespace MedicamentsAggregator.Service.Models.Medgorodok
{
    public class MedgorodokMedicamentPageParser
    {
        public MedgorodokMedicamentModel Parse(ClientMedicamentModel clientMedicamentModel)
        {
            Thread.Sleep(1000);
            return new MedgorodokMedicamentModel();
        }
    }
}