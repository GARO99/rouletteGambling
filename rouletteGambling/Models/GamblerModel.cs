using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Models
{
    public class GamblerModel
    {
        private readonly RedisCache.RedisCache redisCache;

        public GamblerModel(IDistributedCache distributedCache)
        {
            redisCache = new RedisCache.RedisCache(distributedCache);
        }

        public List<GamblerEntity> GetGambler()
        {
            try
            {
                List<GamblerEntity> objGamblers = redisCache.GetGamblerFromRedis();

                return objGamblers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidGamblerExist(string id)
        {
            try
            {
                List<GamblerEntity> objGamblers = GetGambler();
                if (objGamblers.Any(g => g.Id == id))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CreateGambler(GamblerEntity objGambler)
        {
            try
            {
                if (ValidGamblerExist(objGambler.Id))
                    return false;
                List<GamblerEntity> objGamblers = GetGambler();
                objGamblers.Add(objGambler);
                redisCache.SetGamblerToRedis(objGamblers);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateGamblerCredits(string id, decimal credits)
        {
            try
            {
                List<GamblerEntity> objGamblers = GetGambler();
                (from gamblers in objGamblers
                 where gamblers.Id == id
                 select gamblers).ToList().ForEach(g => g.Credits = credits);
                redisCache.SetGamblerToRedis(objGamblers);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
