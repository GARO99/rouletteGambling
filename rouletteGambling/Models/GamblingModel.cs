using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Models
{
    public class GamblingModel
    {
        private readonly RedisCache.RedisCache redisCache;

        public GamblingModel(IDistributedCache distributedCache)
        {
            redisCache = new RedisCache.RedisCache(distributedCache);
        }

        public List<GamblingEntity> GetGamblings()
        {
            try
            {
                List<GamblingEntity> objGamblings = redisCache.GetGamblingFromRedis();

                return objGamblings;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GamblingEntity GetOneGambling(int betId, string gamblerId)
        {
            try
            {
                List<GamblingEntity> objGamblings = GetGamblings();
                return objGamblings.Where(g => g.BetId == betId && g.GamblerId == gamblerId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<GamblingEntity> GetGamblingxBet(int betId)
        {
            try
            {
                List<GamblingEntity> objGamblings = GetGamblings();
                return objGamblings.Where(g => g.BetId == betId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RegisterGambling(BetEntity objBet, BetRequest betRequest, string gamblerId)
        {
            try
            {
                List<GamblingEntity> objGamblings = GetGamblings();
                objGamblings.Add(new GamblingEntity
                {
                    BetId = objBet.Id,
                    GamblerId = gamblerId,
                    CreditsBet = betRequest.CreditsBet,
                    BetType = betRequest.BetType,
                    BetNumber = betRequest.BetNumber,
                    BetColor = betRequest.BetColor,
                    WonBet = null
                });
                redisCache.SetGamblingToRedis(objGamblings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidGamblerAlreadyBetOnGambling(int betId, string gamblerId)
        {
            try
            {
                GamblingEntity objGambling = GetOneGambling(betId, gamblerId);
                if (objGambling != null)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RegistarGamblingResult(int betId, CloseBetRequest closeBetRequest)
        {
            try
            {
                List<GamblingEntity> objGamblings = GetGamblings();
                (from gamblings in objGamblings
                 where gamblings.BetId == betId && (gamblings.BetColor == closeBetRequest.BetResultColor || gamblings.BetNumber == closeBetRequest.BetResultNumber)
                 select gamblings).ToList().ForEach(g => g.WonBet = true);
                redisCache.SetGamblingToRedis(objGamblings);
                objGamblings = GetGamblings();
                (from gamblings in objGamblings
                 where gamblings.BetId == betId && gamblings.WonBet == null
                 select gamblings).ToList().ForEach(g => g.WonBet = false);
                redisCache.SetGamblingToRedis(objGamblings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
