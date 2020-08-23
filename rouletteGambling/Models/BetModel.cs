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
        private readonly GamblerModel gamblerModel;
        private readonly RouletteModel rouletteModel;
        private readonly GamblingModel gamblingModel;
        private readonly BetValidation betValidation;
        private readonly GamblingValidation gamblingValidation;

        public BetModel(IDistributedCache distributedCache)
        {
            redisCache = new RedisCache.RedisCache(distributedCache);
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
                List<BetEntity> objBets = redisCache.GetBetsFromRedis();

                return objBets;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BetResultEntity> GetBetResults()
        {
            try
            {
                List<BetResultEntity> objBets = redisCache.GetBetResultFromRedis();

                return objBets;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BetEntity GetOneBet(int rouletteId)
        {
            try
            {
                List<BetEntity> objBets = GetBets();
                return objBets.Where(b => b.RouletteId == rouletteId && b.Status == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BetResultEntity GetOneBetResult(int betId)
        {
            try
            {
                List<BetResultEntity> objBets = GetBetResults();
                return objBets.Where(b => b.BetId == betId).FirstOrDefault();
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
                    objBet = GetOneBet(betRequest.RouletteId);
                else
                    objBet = CreateBet(betRequest.RouletteId);
                if (gamblingValidation.ValidGamblerAlreadyBetOnGambling(objBet.Id, gamblerId))
                    return 0;
                gamblingModel.RegisterGambling(objBet, betRequest, gamblerId);
                GamblerEntity objGambler = gamblerModel.GetOneGambler(gamblerId);
                gamblerModel.UpdateGamblerCredits(gamblerId, objGambler.Credits - betRequest.CreditsBet);

                return objBet.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RegisterBetResult(int betId, CloseBetRequest closeBetRequest)
        {
            try
            {
                List<BetResultEntity> betResults = GetBetResults();
                betResults.Add(new BetResultEntity
                {
                    BetId = betId,
                    Number = closeBetRequest.BetResultNumber,
                    Color = closeBetRequest.BetResultColor
                });
                redisCache.SetBetResultToRedis(betResults);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BetEntity CreateBet(int RouletteId)
        {
            int BetId = 0;
            try
            {
                List<BetEntity> objBets = GetBets();
                if (objBets.Count > 0)
                    BetId = objBets.Max(b => b.Id) + 1;
                else
                    BetId++;
                objBets.Add(new BetEntity
                {
                    Id = BetId,
                    RouletteId = RouletteId,
                    Status = true
                });
                redisCache.SetBetsToRedis(objBets);

                return GetOneBet(RouletteId);
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
                if (!rouletteModel.ChangeStatusRoulette(rouletteId, false))
                    return 0;
                List<BetEntity> objBets = GetBets();
                BetEntity objBet = GetOneBet(rouletteId);
                (from bets in objBets
                 where bets.RouletteId == rouletteId && bets.Status == true
                 select bets).ToList().ForEach(b => b.Status = false);
                redisCache.SetBetsToRedis(objBets);
                RegisterBetResult(objBet.Id, closeBetRequest);
                gamblingModel.RegistarGamblingResult(objBet.Id, closeBetRequest);

                return objBet.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
