using System.Threading.Tasks;
using MedicamentsAggregator.Service.Models.Aggregate;
using MedicamentsAggregator.Service.Models.Client;
using Microsoft.AspNetCore.Mvc;

namespace MedicamentsAggregator.Service.Controllers
{
    [ApiController]
    [Route("api")]
    public class MedicamentsController : ControllerBase
    {
        private readonly MedicamentsAggregateProcessor _medicamentsAggregateProcessor;

        public MedicamentsController(MedicamentsAggregateProcessor medicamentsAggregateProcessor)
        {
            _medicamentsAggregateProcessor = medicamentsAggregateProcessor;
        }

        [HttpPost("aggregate")]
        public async Task<ActionResult<ClientAggregateModel>> Aggregate(ClientAggregateModel model)
        {
            var result = await _medicamentsAggregateProcessor.Process(model);
            return Ok(result);
        }
    }
}