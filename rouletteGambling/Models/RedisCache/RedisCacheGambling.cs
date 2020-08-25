using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace rouletteGambling.Models.RedisCache
{
    public class RedisCacheGambling
    {
        private readonly IDistributedCache distributedCache;

        public RedisCacheGambling(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public List<GamblingEntity> GetGamblingFromRedis()
        {
            List<GamblingEntity> objGamblings = new List<GamblingEntity>();
            try
            {
                string objJsonGambling = distributedCache.GetString(RedisKeysEnum.Gambling.ToString());
                if (!string.IsNullOrEmpty(objJsonGambling))
                    objGamblings = JsonSerializer.Deserialize<List<GamblingEntity>>(objJsonGambling);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objGamblings;
        }

        public void SetGamblingToRedis(List<GamblingEntity> objGamblings)
        {
            try
            {
                string objJsonGamblings = JsonSerializer.Serialize(objGamblings);
                distributedCache.SetString(RedisKeysEnum.Gambling.ToString(), objJsonGamblings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
