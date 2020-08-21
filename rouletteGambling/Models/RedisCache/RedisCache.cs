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
    public class RedisCache
    {
        private readonly IDistributedCache distributedCache;

        public RedisCache(IDistributedCache distributedCache)
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
