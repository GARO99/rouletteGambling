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
        [Route("getallroulette")]
        public ActionResult GetAllRoulette()
        {
            try
            {
                // Code
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
            return Ok("ok");
        }

        [HttpPost]
        [Route("createroulette")]
        public ActionResult CreateRoulette()
        {
            try
            {
                // Code
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
            return Ok();
        }

        [HttpPost]
        [Route("openroulette")]
        public ActionResult OpenRoulette()
        {
            try
            {
                // Code
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
            return Ok();
        }
    }
}
