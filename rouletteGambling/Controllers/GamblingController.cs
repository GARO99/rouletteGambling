using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Requests;
using rouletteGambling.Utils.Responses;

namespace rouletteGambling.Controllers
{
    [ApiController]
    [Route("gambling")]
    public class GamblingController : ControllerBase
    {
        private readonly GamblingModel gamblingModel;
        private readonly GamblerModel gamblerModel;

        public GamblingController(IDistributedCache distributedCache)
        {
            gamblingModel = new GamblingModel(distributedCache);
            gamblerModel = new GamblerModel(distributedCache);
        }

        [HttpPost]
        [Route("bet")]
        public ActionResult Bet([FromHeader] string gamblerId , [FromBody] BetRequest betRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(gamblerId))
                    return BadRequest(ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString());
                if (betRequest == null)
                    return BadRequest(ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString());
                if (!gamblingModel.ValidBetData(gamblerId, betRequest))
                    return BadRequest(gamblingModel.ErrorMessage);
                int betId = gamblingModel.RegisterBet(gamblerId, betRequest);
                GamblingEntity objGambling = gamblingModel.GetOneGambling(betId, gamblerId);
                GamblerEntity objGambler =  gamblerModel.GetOneGambler(objGambling.GamblerId);
                BetResponse objResponse = new BetResponse
                {
                    GamblerId = objGambler.Id,
                    GamblerFullName = objGambler.FullName,
                    CreditsBet = objGambling.CreditsBet,
                    BetType = Enum.GetName(typeof(BetTypeEnum), objGambling.BetType),
                    BetNumber = objGambling.BetNumber,
                    BetColor = objGambling.BetColor != null? Enum.GetName(typeof(ColorBetEnum), objGambling.BetColor) : null                   
                };

                return Ok(objResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("closebet")]
        public ActionResult CloseBet()
        {
            try
            {

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
