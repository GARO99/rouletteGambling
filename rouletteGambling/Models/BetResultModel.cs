using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Models
{
    public class BetResultModel
    {
        private readonly RedisCache.RedisCache redisCache;

        public BetResultModel(IDistributedCache distributedCache)
        {
            redisCache = new RedisCache.RedisCache(distributedCache);
        }

        public List<BetResultEntity> GetBetResults()
        {
            try
            {
                return redisCache.GetBetResultFromRedis();
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
                return GetBetResults().Where(b => b.BetId == betId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InsertBetResult(int betId, CloseBetRequest closeBetRequest)
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
    }
}
