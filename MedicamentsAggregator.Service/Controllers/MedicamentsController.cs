using System.Threading.Tasks;
using MedicamentsAggregator.Service.Models.Aggregate;
using MedicamentsAggregator.Service.Models.Request;
using MedicamentsAggregator.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace MedicamentsAggregator.Service.Controllers
{
    [ApiController]
    [Route("api")]
    public class MedicamentsController : ControllerBase
    {
        private readonly AggregateProcessor _aggregateProcessor;

        public MedicamentsController(AggregateProcessor aggregateProcessor)
        {
            _aggregateProcessor = aggregateProcessor;
        }

        [HttpPost("aggregate")]
        public async Task<ActionResult<ResponseAggregateModel>> Aggregate(RequestAggregateModel model)
        {
            var result = await _aggregateProcessor.Process(model);
            return Ok(result);
        }
    }
}