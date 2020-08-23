using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Requests;
using rouletteGambling.Utils.Validations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Models
{
    public class BetModel
    {
        private readonly RedisCache.RedisCache redisCache;
        private readonly BetResultModel betResultModel;
        private readonly GamblerModel gamblerModel;
        private readonly RouletteModel rouletteModel;
        private readonly GamblingModel gamblingModel;
        private readonly BetValidation betValidation;
        private readonly GamblingValidation gamblingValidation;

        public BetModel(IDistributedCache distributedCache)
        {
            redisCache = new RedisCache.RedisCache(distributedCache);
            betResultModel = new BetResultModel(distributedCache);
            gamblerModel = new GamblerModel(distributedCache);
            rouletteModel = new RouletteModel(distributedCache);
            gamblingModel = new GamblingModel(distributedCache);
            betValidation = new BetValidation(distributedCache);
            gamblingValidation = new GamblingValidation(distributedCache);
        }

        public List<BetEntity> GetBets()
        {
            try
            {
                return redisCache.GetBetsFromRedis();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BetEntity GetOneBet(int betId)
        {
            try
            {
                return GetBets().Where(b => b.RouletteId == betId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int RegisterBet(string gamblerId, BetRequest betRequest)
        {
            try
            {
                BetEntity objBet;
                if (betValidation.ValidBetWithRouletteExist(betRequest.RouletteId))
                    objBet = GetBets().Where(b => b.RouletteId == betRequest.RouletteId && b.Status == true).FirstOrDefault();
                else
                    objBet = CreateBet(betRequest.RouletteId);
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

        public BetEntity CreateBet(int RouletteId)
        {
            int betId = 0;
            try
            {
                List<BetEntity> objBets = GetBets();
                if (objBets.Count > 0)
                    betId = objBets.Max(b => b.Id) + 1;
                else
                    betId++;
                objBets.Add(new BetEntity
                {
                    Id = betId,
                    RouletteId = RouletteId,
                    Status = true
                });
                redisCache.SetBetsToRedis(objBets);

                return GetOneBet(betId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatusBet(int Id, bool status)
        {
            try
            {
                List<BetEntity> objBets = GetBets();
                (from bets in objBets
                 where bets.Id == Id
                 select bets).ToList().ForEach(b => b.Status = status);
                redisCache.SetBetsToRedis(objBets);
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
                BetEntity objBet = GetBets().Where(b => b.RouletteId == rouletteId && b.Status == true).FirstOrDefault();
                UpdateStatusBet(objBet.Id, status:false);
                betResultModel.InsertBetResult(objBet.Id, closeBetRequest);
                gamblingModel.UpdateGamblingsToSaveResult(objBet.Id, closeBetRequest);

                return objBet.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
