using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Rules;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Requests;
using rouletteGambling.Utils.Validations;
using System;

namespace rouletteGambling.Controllers
{
    [ApiController]
    [Route("gambling")]
    public class GamblingController : ControllerBase
    {
        private readonly CBet cBet;
        private readonly BetValidation betValidation;

        public GamblingController(IDistributedCache distributedCache)
        {
            cBet = new CBet(distributedCache);
            betValidation = new BetValidation(distributedCache);
        }

        [HttpPost]
        [Route("bet")]
        public ActionResult Bet([FromHeader] string gamblerId, [FromBody] BetRequest betRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(gamblerId) || betRequest == null)
                    return BadRequest(ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString());
                if (!betValidation.ValidBetData(gamblerId, betRequest))
                    return BadRequest(betValidation.ErrorMessage);
                int betId = cBet.RegisterBet(gamblerId, betRequest);
                if (betId == 0)
                    return BadRequest(ErrorEnum.ERROR_GAMBLER_ALREADY_BET.ToString());

                return Ok(cBet.BuilBetResponse(betId, gamblerId));
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
                if (rouletteId == 0 || closeBetRequest == null)
                    return BadRequest(ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString());
                if (!betValidation.ValidCloseBetData(rouletteId, closeBetRequest))
                    return BadRequest(betValidation.ErrorMessage);
                int betId = cBet.CloseBet(rouletteId, closeBetRequest);

                return Ok(cBet.BuilBetResponse(betId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
