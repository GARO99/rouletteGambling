using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Services;
using rouletteGambling.Utils.Requests;
using rouletteGambling.Utils.Responses;
using System;

namespace rouletteGambling.Controllers
{
    [ApiController]
    [Route("gambling")]
    public class GamblingController : ControllerBase
    {
        private readonly GamblingService gamblingService;

        public GamblingController(IDistributedCache distributedCache)
        {
            gamblingService = new GamblingService(distributedCache);
        }

        [HttpPost]
        [Route("bet")]
        public ActionResult Bet([FromHeader] string gamblerId, [FromBody] BetRequest betRequest)
        {
            try
            {
                BetResponse objBetResponse = gamblingService.Bet(gamblerId, betRequest);
                if (objBetResponse == null && !string.IsNullOrEmpty(gamblingService.ErrorMessage))
                    return BadRequest(gamblingService.ErrorMessage);

                return Ok(objBetResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{rouletteId}")]
        [Route("closebet/{rouletteId}")]
        public ActionResult CloseBet(int rouletteId, [FromBody] CloseBetRequest closeBetRequest)
        {
            try
            {
                CloseBetResponse closeBetResponse = gamblingService.CloseBet(rouletteId, closeBetRequest);
                if (closeBetResponse == null && !string.IsNullOrEmpty(gamblingService.ErrorMessage))
                    return BadRequest(gamblingService.ErrorMessage);

                return Ok(closeBetResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
