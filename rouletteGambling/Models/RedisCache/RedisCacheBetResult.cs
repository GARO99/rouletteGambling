using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace rouletteGambling.Models.RedisCache
{
    public class RedisCacheBetResult
    {
        private readonly IDistributedCache distributedCache;

        public RedisCacheBetResult(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public List<BetResultEntity> GetBetResultFromRedis()
        {
            List<BetResultEntity> objBetResult = new List<BetResultEntity>();
            try
            {
                string objJsonBetResult = distributedCache.GetString(RedisKeysEnum.BetResult.ToString());
                if (!string.IsNullOrEmpty(objJsonBetResult))
                    objBetResult = JsonSerializer.Deserialize<List<BetResultEntity>>(objJsonBetResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objBetResult;
        }

        public void SetBetResultToRedis(List<BetResultEntity> objBetResult)
        {
            try
            {
                string objJsonBetResult = JsonSerializer.Serialize(objBetResult);
                distributedCache.SetString(RedisKeysEnum.BetResult.ToString(), objJsonBetResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
