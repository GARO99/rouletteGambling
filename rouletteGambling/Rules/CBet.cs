using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Requests;
using rouletteGambling.Utils.Responses;
using rouletteGambling.Utils.Validations;
using System;
using System.Linq;

namespace rouletteGambling.Rules
{
    public class CBet
    {
        private readonly BetModel betModel;
        private readonly BetResultModel betResultModel;
        private readonly GamblerModel gamblerModel;
        private readonly RouletteModel rouletteModel;
        private readonly GamblingModel gamblingModel;
        private readonly BetValidation betValidation;
        private readonly GamblingValidation gamblingValidation;
        private readonly CGambling cGambling;

        public CBet(IDistributedCache distributedCache)
        {
            betModel = new BetModel(distributedCache);
            betResultModel = new BetResultModel(distributedCache);
            gamblerModel = new GamblerModel(distributedCache);
            rouletteModel = new RouletteModel(distributedCache);
            gamblingModel = new GamblingModel(distributedCache);
            betValidation = new BetValidation(distributedCache);
            gamblingValidation = new GamblingValidation(distributedCache);
            cGambling = new CGambling(distributedCache);
        }

        public int RegisterBet(string gamblerId, BetRequest betRequest)
        {
            try
            {
                BetEntity objBet;
                if (betValidation.ValidBetWithRouletteExist(betRequest.RouletteId))
                    objBet = betModel.GetBets().Where(b => b.RouletteId == betRequest.RouletteId && b.Status == true).FirstOrDefault();
                else
                    objBet = betModel.CreateBet(betRequest.RouletteId);
                if (gamblingValidation.ValidGamblerAlreadyBetOnGambling(objBet.Id, gamblerId))
                    return 0;
                gamblingModel.InsertGambling(objBet, betRequest, gamblerId);
                gamblerModel.UpdateGamblerCredits(gamblerId, gamblerModel.GetOneGambler(gamblerId).Credits - betRequest.CreditsBet);

                return objBet.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BetResponse BuilBetResponse(int betId, string gamblerId)
        {
            try
            {
                GamblingEntity objGambling = gamblingModel.GetOneGambling(betId, gamblerId);
                GamblerEntity objGambler = gamblerModel.GetOneGambler(objGambling.GamblerId);
                BetResponse objResponse = new BetResponse
                {
                    GamblerId = objGambler.Id,
                    GamblerFullName = objGambler.FullName,
                    CreditsBet = objGambling.CreditsBet,
                    BetType = Enum.GetName(typeof(BetTypeEnum), objGambling.BetType),
                    BetNumber = objGambling.BetNumber,
                    BetColor = objGambling.BetColor != null ? Enum.GetName(typeof(ColorBetEnum), objGambling.BetColor) : null
                };

                return objResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CloseBet(int rouletteId, CloseBetRequest closeBetRequest)
        {
            try
            {
                if (!rouletteModel.UpdateStatusRoulette(rouletteId, false))
                    return 0;
                BetEntity objBet = betModel.GetBets().Where(b => b.RouletteId == rouletteId && b.Status == true).FirstOrDefault();
                betModel.UpdateStatusBet(objBet.Id, status: false);
                betResultModel.InsertBetResult(objBet.Id, closeBetRequest);
                gamblingModel.UpdateGamblingsToSaveResult(objBet.Id, closeBetRequest);

                return objBet.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CloseBetResponse BuilBetResponse(int betId)
        {
            try
            {
                BetResultEntity objBetResult = betResultModel.GetOneBetResult(betId);
                CloseBetResponse closeBetResponse = new CloseBetResponse
                {
                    BetResult = new BetResultResponse
                    {
                        Number = objBetResult.Number,
                        Color = Enum.GetName(typeof(ColorBetEnum), objBetResult.Color)
                    },
                    GamblingResult = cGambling.BuildGamblingResultResponse(betId)
                };

                return closeBetResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
