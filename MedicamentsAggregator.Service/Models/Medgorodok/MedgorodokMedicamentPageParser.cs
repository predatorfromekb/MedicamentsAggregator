using System.Threading;
using System.Threading.Tasks;
using MedicamentsAggregator.Service.Models.Client;

namespace MedicamentsAggregator.Service.Models.Medgorodok
{
    public class MedgorodokMedicamentPageParser
    {
        public async Task<MedgorodokMedicamentModel> Parse(ClientMedicamentModel clientMedicamentModel)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(1000);
                return new MedgorodokMedicamentModel
                {
                    Count =  clientMedicamentModel.Count
                };
            });
        }
    }
}