using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace rouletteGambling.Models.RedisCache
{
    public class RedisCacheRoulette
    {
        private readonly IDistributedCache distributedCache;

        public RedisCacheRoulette(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public List<RouletteEntity> GetRoulettesFromRedis()
        {
            List<RouletteEntity> objRoulettes = new List<RouletteEntity>();
            try
            {
                string objJsonRoulettes = distributedCache.GetString(RedisKeysEnum.Roulette.ToString());
                if (!string.IsNullOrEmpty(objJsonRoulettes))
                    objRoulettes = JsonSerializer.Deserialize<List<RouletteEntity>>(objJsonRoulettes);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objRoulettes;
        }

        public void SetRoulettesToRedis(List<RouletteEntity> objRoulettes)
        {
            try
            {
                string objJsonRoulettes = JsonSerializer.Serialize(objRoulettes);
                distributedCache.SetString(RedisKeysEnum.Roulette.ToString(), objJsonRoulettes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
