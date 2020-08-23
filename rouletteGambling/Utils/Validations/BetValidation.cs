using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Utils.Validations
{
    public class BetValidation
    {
        private readonly BetModel betModel;
        private readonly RouletteModel rouletteModel;
        private readonly GamblerModel gamblerModel;
        private readonly GamblerValidation gamblerValidation;
        private readonly RouletteValidation rouletteValidation;
        public string ErrorMessage { get; set; }

        public BetValidation(IDistributedCache distributedCache)
        {
            betModel = new BetModel(distributedCache);
            gamblerValidation = new GamblerValidation(distributedCache);
            rouletteValidation = new RouletteValidation(distributedCache);
            gamblerModel = new GamblerModel(distributedCache);
            rouletteModel = new RouletteModel(distributedCache);
        }

        public bool ValidBetRequest(BetRequest betRequest)
        {
            try
            {
                if (betRequest.RouletteId <= 0)
                    return false;
                switch (betRequest.BetType)
                {
                    case (int)BetTypeEnum.Number:
                        if (betRequest.BetNumber == null)
                            return false;
                        if (betRequest.BetNumber < (int)RulesBetEnum.MinBetNumber || betRequest.BetNumber > (int)RulesBetEnum.MaxBetNumber)
                            return false;
                        break;
                    case (int)BetTypeEnum.Color:
                        if (betRequest.BetColor == null)
                            return false;
                        if (betRequest.BetColor != (int)ColorBetEnum.Black && betRequest.BetColor != (int)ColorBetEnum.Red)
                            return false;
                        break;
                    default:
                        return false;
                }
                if (betRequest.CreditsBet <= 0 || betRequest.CreditsBet > (decimal)RulesBetEnum.MaxCreditsBet)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidCloseBetRequest(CloseBetRequest closeBetRequest)
        {
            try
            {
                if (closeBetRequest.BetResultNumber < (int)RulesBetEnum.MinBetNumber || closeBetRequest.BetResultNumber > (int)RulesBetEnum.MaxBetNumber)
                    return false;
                if (closeBetRequest.BetResultColor != (int)ColorBetEnum.Black && closeBetRequest.BetResultColor != (int)ColorBetEnum.Red)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidBetData(string gamblerId, BetRequest betRequest)
        {
            try
            {
                if (!ValidBetRequest(betRequest))
                {
                    ErrorMessage = ErrorEnum.ERROR_RULES_BET.ToString();
                    return false;
                }
                if (!gamblerValidation.ValidGamblerExist(gamblerId))
                {
                    ErrorMessage = ErrorEnum.ERROR_GAMBLER_NOT_EXIST.ToString();
                    return false;
                }
                if (!rouletteValidation.ValidRouletteExist(betRequest.RouletteId))
                {
                    ErrorMessage = ErrorEnum.ERROR_ROULETTE_NOT_EXIST.ToString();
                    return false;
                }
                if (!rouletteValidation.ValidRouletteIsOpen(betRequest.RouletteId, rouletteModel.GetRoulettes()))
                {
                    ErrorMessage = ErrorEnum.ERROR_ROULETTE_IS_NOT_OPEN.ToString();
                    return false;
                }
                if ((gamblerModel.GetOneGambler(gamblerId).Credits - betRequest.CreditsBet) < 0)
                {
                    ErrorMessage = ErrorEnum.ERROR_GAMBLER_DOES_NOT_HAVE_CREDITS.ToString();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidCloseBetData(int rouletteId, CloseBetRequest closeBetRequest)
        {
            try
            {
                if (!ValidCloseBetRequest(closeBetRequest))
                {
                    ErrorMessage = ErrorEnum.ERROR_RULES_BET.ToString();
                    return false;
                }
                if (!rouletteValidation.ValidRouletteExist(rouletteId))
                {
                    ErrorMessage = ErrorEnum.ERROR_ROULETTE_NOT_EXIST.ToString();
                    return false;
                }
                if (!rouletteValidation.ValidRouletteIsOpen(rouletteId, rouletteModel.GetRoulettes()))
                {
                    ErrorMessage = ErrorEnum.ERROR_ROULETTE_IS_NOT_OPEN.ToString();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidBetWithRouletteExist(int RouletteId)
        {
            try
            {
                BetEntity objBet = betModel.GetBets().Where(b => b.RouletteId == RouletteId && b.Status == true).FirstOrDefault();
                if (objBet == null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
