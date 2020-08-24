using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Services;
using System;

namespace rouletteGambling.Controllers
{
    [ApiController]
    [Route("gambler")]
    public class GamblerController : ControllerBase
    {
        private readonly GamblerService gamblerService;

        public GamblerController(IDistributedCache distributedCache)
        {
            gamblerService = new GamblerService(distributedCache);
        }

        [HttpGet]
        [Route("getgambler")]
        public ActionResult GetGambler()
        {
            try
            {
                return Ok(gamblerService.GetGambler());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("creategambler")]
        public ActionResult CreateGambler([FromBody] GamblerEntity objGamblerRequest)
        {
            try
            {
                bool successTransaction = gamblerService.CreateGambler(objGamblerRequest);
                if (!successTransaction && !string.IsNullOrEmpty(gamblerService.ErrorMessage))
                {
                    return BadRequest(gamblerService.ErrorMessage);
                }

                return Ok(successTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{credits}")]
        [Route("updategamblercredits/{credits}")]
        public ActionResult UpdateGamblerCredits([FromHeader] string id, decimal credits)
        {
            try
            {
                bool successTransaction = gamblerService.UpdateGamblerCredits(id, credits);
                if (!successTransaction && !string.IsNullOrEmpty(gamblerService.ErrorMessage))
                {
                    return BadRequest(gamblerService.ErrorMessage);
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
