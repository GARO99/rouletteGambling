using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Models
{
    public class BetModel
    {
        private readonly RedisCache.RedisCacheBet redisCache;

        public BetModel(IDistributedCache distributedCache)
        {
            redisCache = new RedisCache.RedisCacheBet(distributedCache);
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
                return GetBets().Where(b => b.Id == betId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BetEntity CreateBet(int betId, int RouletteId, bool status)
        {
            try
            {
                List<BetEntity> objBets = GetBets();
                objBets.Add(new BetEntity
                {
                    Id = betId,
                    RouletteId = RouletteId,
                    Status = status
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
    }
}
