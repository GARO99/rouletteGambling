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
                if (betId == 0)
                    return BadRequest(ErrorEnum.ERROR_GAMBLER_ALREADY_BET.ToString());
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

        [HttpPut("{rouletteId}")]
        [Route("closebet/{rouletteId}")]
        public ActionResult CloseBet(int rouletteId, [FromBody] CloseBetRequest closeBetRequest)
        {
            try
            {
                if (rouletteId == 0)
                    return BadRequest(ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString());
                if (closeBetRequest == null)
                    return BadRequest(ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString());
                if (!gamblingModel.ValidCloseBetData(rouletteId, closeBetRequest)) 
                    return BadRequest(gamblingModel.ErrorMessage);
                int betId = gamblingModel.CloseBet(rouletteId, closeBetRequest);
                List<GamblingEntity> objGambling = gamblingModel.GetGamblingxBet(betId);
                List<GamblingResultResponse> gamblingResultResponse = new List<GamblingResultResponse>();
                foreach (GamblingEntity gambling in objGambling)
                {
                    GamblerEntity objGambler = gamblerModel.GetOneGambler(gambling.GamblerId);
                    gamblingResultResponse.Add(new GamblingResultResponse
                    {
                        GamblerId = objGambler.Id,
                        GamblerFullName = objGambler.FullName,
                        CreditsBet = gambling.CreditsBet,
                        BetType = Enum.GetName(typeof(BetTypeEnum), gambling.BetType),
                        BetNumber = gambling.BetNumber,
                        BetColor = gambling.BetColor != null ? Enum.GetName(typeof(ColorBetEnum), gambling.BetColor) : null,
                        WontBet = gambling.WonBet.Value
                    });
                }
                BetResultEntity objBetResult = gamblingModel.GetOneBetResult(betId);
                CloseBetResponse closeBetResponse = new CloseBetResponse
                {
                    BetResult = new BetResultResponse
                    {
                        Number = objBetResult.Number,
                        Color = Enum.GetName(typeof(ColorBetEnum), objBetResult.Color)
                    },
                   GamblingResult = gamblingResultResponse
                };

                return Ok(closeBetResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
