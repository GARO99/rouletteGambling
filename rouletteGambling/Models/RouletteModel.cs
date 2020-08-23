﻿using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Models
{
    public class RouletteModel
    {
        private readonly RedisCache.RedisCache redisCache;

        public RouletteModel(IDistributedCache distributedCache)
        {
            redisCache = new RedisCache.RedisCache(distributedCache);
        }

        public List<RouletteEntity> GetRoulettes()
        {
            try
            {
                List<RouletteEntity> objRoulettes = redisCache.GetRoulettesFromRedis();

                return objRoulettes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CreateRoulette()
        {
            int roulettesId = 0;
            List<RouletteEntity> objRoulettes = new List<RouletteEntity>();
            try
            {
                objRoulettes = GetRoulettes();
                if (objRoulettes.Count > 0)
                    roulettesId = objRoulettes.Max(r => r.Id) + 1;
                else
                    roulettesId++;
                objRoulettes.Add(new RouletteEntity
                {
                    Id = roulettesId,
                    Status = false
                });
                redisCache.SetRoulettesToRedis(objRoulettes);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return roulettesId;
        }

        public bool OpenRoulette(int id)
        {
            try
            {
                List<RouletteEntity> objRoulettes = GetRoulettes();
                (from roulettes in objRoulettes
                 where roulettes.Id == id
                 select roulettes).ToList().ForEach(r => r.Status = true);
                redisCache.SetRoulettesToRedis(objRoulettes);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CloseRoulette(int id)
        {
            try
            {
                List<RouletteEntity> objRoulettes = GetRoulettes();
                (from roulettes in objRoulettes
                 where roulettes.Id == id
                 select roulettes).ToList().ForEach(r => r.Status = false);
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
