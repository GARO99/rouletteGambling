using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Rules
{
    public class CGambling
    {

        private readonly GamblingModel gamblingModel;
        private readonly GamblerModel gamblerModel;

        public CGambling(IDistributedCache distributedCache)
        {
            gamblingModel = new GamblingModel(distributedCache);
            gamblerModel = new GamblerModel(distributedCache);
        }

        public List<GamblingResultResponse> BuildGamblingResultResponse(int betId)
        {
            try
            {
                List<GamblingResultResponse> gamblingResultResponse = new List<GamblingResultResponse>();
                foreach (GamblingEntity gambling in gamblingModel.GetGamblings().Where(g => g.BetId == betId).ToList())
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

                return gamblingResultResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
