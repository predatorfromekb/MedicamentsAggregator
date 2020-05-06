using System.Threading;
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
        public ActionResult<ClientSearchModel> Search(ClientSearchModel model)
        {
            _medicamentsSearchProcessor.Process(model);
            return Ok(model);
        }
    }
}