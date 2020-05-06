using System.Threading;
using System.Threading.Tasks;
using MedicamentsAggregator.Service.Models.Client;
using MedicamentsAggregator.Service.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace MedicamentsAggregator.Service.Controllers
{
    [ApiController]
    [Route("api")]
    public class MedicamentsController : ControllerBase
    {
        private readonly MedicamentsSearchProcessor _medicamentsSearchProcessor;

        public MedicamentsController(MedicamentsSearchProcessor medicamentsSearchProcessor)
        {
            _medicamentsSearchProcessor = medicamentsSearchProcessor;
        }

        [HttpPost("search")]
        public async Task<ActionResult<ClientSearchModel>> Search(ClientSearchModel model)
        {
            var result = await _medicamentsSearchProcessor.Process(model);
            return Ok(result);
        }
    }
}