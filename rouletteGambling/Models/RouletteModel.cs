using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace rouletteGambling.Models
{
    public class RouletteModel
    {
        private readonly IDistributedCache distributedCache;

        public RouletteModel(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public List<RouletteEntity> GetRoulettes()
        {
            List<RouletteEntity> objRoulettes = new List<RouletteEntity>();
            try
            {
                string objJsonRoulettes = distributedCache.GetString(RedisKeysEnum.Roulette.ToString());
                if (!string.IsNullOrEmpty(objJsonRoulettes))
                {
                    objRoulettes = JsonSerializer.Deserialize<List<RouletteEntity>>(objJsonRoulettes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objRoulettes;
        }

        public int CreateRoulette()
        {
            int roulettesId = 0;
            string objJsonRoulettes;
            List<RouletteEntity> objRoulettes = new List<RouletteEntity>();
            try
            {
                objJsonRoulettes = distributedCache.GetString(RedisKeysEnum.Roulette.ToString());
                if (!string.IsNullOrEmpty(objJsonRoulettes))
                {
                    objRoulettes = JsonSerializer.Deserialize<List<RouletteEntity>>(objJsonRoulettes);
                    roulettesId = objRoulettes.Max(r => r.Id) + 1;
                    objRoulettes.Add(new RouletteEntity
                    {
                        Id = roulettesId,
                        Status = false
                    });
                    objJsonRoulettes = JsonSerializer.Serialize(objRoulettes);
                    distributedCache.SetString(RedisKeysEnum.Roulette.ToString(), objJsonRoulettes);
                }
                else
                {
                    roulettesId++;
                    objRoulettes.Add(new RouletteEntity
                    {
                        Id = roulettesId,
                        Status = false
                    });
                    objJsonRoulettes = JsonSerializer.Serialize(objRoulettes);
                    distributedCache.SetString(RedisKeysEnum.Roulette.ToString(), objJsonRoulettes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return roulettesId;
        }
    }
}
