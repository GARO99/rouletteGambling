using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace rouletteGambling.Models.RedisCache
{
    public class RedisCacheBet
    {
        private readonly IDistributedCache distributedCache;

        public RedisCacheBet(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public List<BetEntity> GetBetsFromRedis()
        {
            List<BetEntity> objBets = new List<BetEntity>();
            try
            {
                string objJsonBets = distributedCache.GetString(RedisKeysEnum.Bet.ToString());
                if (!string.IsNullOrEmpty(objJsonBets))
                    objBets = JsonSerializer.Deserialize<List<BetEntity>>(objJsonBets);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objBets;
        }

        public void SetBetsToRedis(List<BetEntity> objBets)
        {
            try
            {
                string objJsonBets = JsonSerializer.Serialize(objBets);
                distributedCache.SetString(RedisKeysEnum.Bet.ToString(), objJsonBets);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
