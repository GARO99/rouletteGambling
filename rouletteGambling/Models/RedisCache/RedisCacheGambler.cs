using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace rouletteGambling.Models.RedisCache
{
    public class RedisCacheGambler
    {
        private readonly IDistributedCache distributedCache;

        public RedisCacheGambler(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public List<GamblerEntity> GetGamblerFromRedis()
        {
            List<GamblerEntity> objGamblers = new List<GamblerEntity>();
            try
            {
                string objJsonRoulettes = distributedCache.GetString(RedisKeysEnum.Gambler.ToString());
                if (!string.IsNullOrEmpty(objJsonRoulettes))
                    objGamblers = JsonSerializer.Deserialize<List<GamblerEntity>>(objJsonRoulettes);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objGamblers;
        }

        public void SetGamblerToRedis(List<GamblerEntity> objGamblers)
        {
            try
            {
                string objJsonGamblers = JsonSerializer.Serialize(objGamblers);
                distributedCache.SetString(RedisKeysEnum.Gambler.ToString(), objJsonGamblers);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
