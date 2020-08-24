using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Services;
using System;

namespace rouletteGambling.Controllers
{
    [ApiController]
    [Route("roulette")]
    public class RouletteController : ControllerBase
    {
        private readonly RouletteService rouletteService;

        public RouletteController(IDistributedCache distributedCache)
        {
            rouletteService = new RouletteService(distributedCache);
        }

        [HttpGet]
        [Route("getallroulette")]
        public ActionResult GetAllRoulette()
        {
            try
            {
                return Ok(rouletteService.GetAllRoulette());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("createroulette")]
        public ActionResult CreateRoulette()
        {
            try
            {
                return Ok(rouletteService.CreateRoulette());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Route("openroulette/{id}")]
        public ActionResult OpenRoulette(int id)
        {
            try
            {
                bool successTransaction = rouletteService.OpenRoulette(id);
                if (!successTransaction && !string.IsNullOrEmpty(rouletteService.ErrorMessage))
                {
                    return BadRequest(rouletteService.ErrorMessage);
                }

                return Ok(successTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
