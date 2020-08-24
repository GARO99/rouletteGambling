using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Rules;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Requests;
using rouletteGambling.Utils.Responses;
using rouletteGambling.Utils.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Services
{
    public class GamblingService
    {
        private readonly CBet cBet;
        private readonly BetValidation betValidation;
        public string ErrorMessage { get; set; }

        public GamblingService(IDistributedCache distributedCache)
        {
            cBet = new CBet(distributedCache);
            betValidation = new BetValidation(distributedCache);
        }

        public BetResponse Bet(string gamblerId, BetRequest betRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(gamblerId) || betRequest == null)
                {
                    ErrorMessage = ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString();
                    return null;
                }
                if (!betValidation.ValidBetData(gamblerId, betRequest))
                {
                    ErrorMessage = betValidation.ErrorMessage;
                    return null;
                }
                int betId = cBet.RegisterBet(gamblerId, betRequest);
                if (betId == 0)
                {
                    ErrorMessage = ErrorEnum.ERROR_GAMBLER_ALREADY_BET.ToString();
                    return null;
                }

                return cBet.BuilBetResponse(betId, gamblerId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CloseBetResponse CloseBet(int rouletteId, CloseBetRequest closeBetRequest)
        {
            try
            {
                if (rouletteId == 0 || closeBetRequest == null)
                {
                    ErrorMessage = ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString();
                    return null;
                }
                if (!betValidation.ValidCloseBetData(rouletteId, closeBetRequest))
                {
                    ErrorMessage = betValidation.ErrorMessage;
                    return null;
                }
                return cBet.BuilBetResponse(cBet.CloseBet(rouletteId, closeBetRequest));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
