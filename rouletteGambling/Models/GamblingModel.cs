using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Models
{
    public class GamblingModel
    {
        private readonly RedisCache.RedisCacheGambling redisCache;

        public GamblingModel(IDistributedCache distributedCache)
        {
            redisCache = new RedisCache.RedisCacheGambling(distributedCache);
        }

        public List<GamblingEntity> GetGamblings()
        {
            try
            {
                return redisCache.GetGamblingFromRedis();
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
                return GetGamblings().Where(g => g.BetId == betId && g.GamblerId == gamblerId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void InsertGambling(BetEntity objBet, BetRequest betRequest, string gamblerId)
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

        public void UpdateGamblingsToSaveResult(int betId, CloseBetRequest closeBetRequest)
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
