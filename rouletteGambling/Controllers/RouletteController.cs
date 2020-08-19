using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace rouletteGambling.Controllers
{
    [ApiController]
    [Route("roulette")]
    public class RouletteController : ControllerBase
    {
        [HttpGet]
        [Route("getroulette")]
        public ActionResult GetRoulette()
        {
            return Ok("ok");
        }
    }
}
