using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Models
{
    public class RouletteModel
    {
        private readonly RedisCache.RedisCacheRoulette redisCache;

        public RouletteModel(IDistributedCache distributedCache)
        {
            redisCache = new RedisCache.RedisCacheRoulette(distributedCache);
        }

        public List<RouletteEntity> GetRoulettes()
        {
            try
            {
                return redisCache.GetRoulettesFromRedis();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CreateRoulette(int roulettesId, bool status)
        {
            try
            {
                List<RouletteEntity> objRoulettes = GetRoulettes();
                objRoulettes.Add(new RouletteEntity
                {
                    Id = roulettesId,
                    Status = status
                });
                redisCache.SetRoulettesToRedis(objRoulettes);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return roulettesId;
        }

        public bool UpdateRoulette(int id, bool status)
        {
            try
            {
                List<RouletteEntity> objRoulettes = GetRoulettes();
                (from roulettes in objRoulettes
                 where roulettes.Id == id
                 select roulettes).ToList().ForEach(r => r.Status = status);
                redisCache.SetRoulettesToRedis(objRoulettes);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
