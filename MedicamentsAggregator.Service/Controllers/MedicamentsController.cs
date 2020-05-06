using System.Threading;
using MedicamentsAggregator.Service.Models.Client;
using Microsoft.AspNetCore.Mvc;

namespace MedicamentsAggregator.Service.Controllers
{
    [ApiController]
    [Route("api")]
    public class MedicamentsController : ControllerBase
    {
        [HttpPost("search")]
        public ActionResult<ClientSearchModel> Search(ClientSearchModel model)
        {
            Thread.Sleep(7000);
            return Ok(model.Medicaments.Length);
        }
    }
}