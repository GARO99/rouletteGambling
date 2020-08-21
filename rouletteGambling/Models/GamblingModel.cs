using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Models
{
    public class GamblingModel
    {
        private readonly RedisCache.RedisCache redisCache;
        private readonly GamblerModel gamblerModel;
        private readonly RouletteModel rouletteModel;
        public string ErrorMessage { get; set; }

        public GamblingModel(IDistributedCache distributedCache)
        {
            redisCache = new RedisCache.RedisCache(distributedCache);
            gamblerModel = new GamblerModel(distributedCache);
            rouletteModel = new RouletteModel(distributedCache);
        }


        public List<BetEntity> GetBets()
        {
            try
            {
                List<BetEntity> objBets = redisCache.GetBetsFromRedis();

                return objBets;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GamblingEntity> GetGamblings()
        {
            try
            {
                List<GamblingEntity> objGamblings = redisCache.GetGamblingFromRedis();

                return objGamblings;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BetEntity GetOneBet(int RouletteId)
        {
            try
            {
                List<BetEntity> objBets = GetBets();
                return objBets.Where(b => b.RouletteId == RouletteId && b.Status == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GamblingEntity GetOneGambling(int betId, string gamblerId)
        {
            List<GamblingEntity> objGamblings = GetGamblings();
            return objGamblings.Where(g => g.BetId == betId && g.GamblerId == gamblerId).FirstOrDefault();
        }

        public bool ValidBetRequest(BetRequest betRequest)
        {
            try
            {
                if (betRequest.RouletteId <= 0) 
                    return false;
                switch (betRequest.BetType)
                {
                    case (int)BetTypeEnum.Number:
                        if (betRequest.BetNumber == null)
                            return false;
                        if (betRequest.BetNumber < (int)RulesBetEnum.MinBetNumber || betRequest.BetNumber > (int)RulesBetEnum.MaxBetNumber)
                            return false;
                        break;
                    case (int)BetTypeEnum.Color:
                        if (betRequest.BetColor == null)
                            return false;
                        if (betRequest.BetColor != (int)ColorBetEnum.Black && betRequest.BetColor != (int)ColorBetEnum.Red)
                            return false;
                        break;
                    default:
                        return false;
                }
                if (betRequest.CreditsBet <= 0 || betRequest.CreditsBet > (decimal)RulesBetEnum.MaxCreditsBet)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidBetData(string gamblerId, BetRequest betRequest)
        {
            try
            {
                if (!ValidBetRequest(betRequest))
                {
                    ErrorMessage = ErrorEnum.ERROR_RULES_BET.ToString();
                    return false;
                }
                if (!gamblerModel.ValidGamblerExist(gamblerId))
                {
                    ErrorMessage = ErrorEnum.ERROR_GAMBLER_NOT_EXIST.ToString();
                    return false;
                }
                if (!rouletteModel.ValidRouletteExist(betRequest.RouletteId))
                {
                    ErrorMessage = ErrorEnum.ERROR_ROULETTE_NOT_EXIST.ToString();
                    return false;
                }
                if (!rouletteModel.ValidRouletteIsOpen(betRequest.RouletteId, rouletteModel.GetRoulettes()))
                {
                    ErrorMessage = ErrorEnum.ERROR_ROULETTE_IS_NOT_OPEN.ToString();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidBetWithRouletteExist(int RouletteId)
        {
            try
            {
                List<BetEntity> objBets = GetBets();
                BetEntity objBet = objBets.Where(b => b.RouletteId == RouletteId && b.Status == true).FirstOrDefault();
                if (objBet == null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BetEntity CreateBet(int RouletteId)
        {
            int BetId = 0;
            try
            {
                List<BetEntity> objBets = GetBets();
                if (objBets.Count > 0)
                    BetId = objBets.Max(b => b.Id) + 1;
                else
                    BetId++;
                objBets.Add(new BetEntity
                {
                    Id = BetId,
                    RouletteId = RouletteId,
                    Status = true
                });
                redisCache.SetBetsToRedis(objBets);

                return GetOneBet(RouletteId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RegisterGambling(BetEntity objBet, BetRequest betRequest , string gamblerId)
        {
            try
            {
                List<GamblingEntity> objGamblings = GetGamblings();
                objGamblings.Add(new GamblingEntity
                {
                    BetId = objBet.Id,
                    GamblerId = gamblerId,
                    CreditsBet = betRequest.CreditsBet,
                    BetType = betRequest.BetType,
                    BetNumber = betRequest.BetNumber,
                    BetColor = betRequest.BetColor,
                    WonBet = null
                });
                redisCache.SetGamblingToRedis(objGamblings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int RegisterBet(string gamblerId, BetRequest betRequest)
        {
            try
            {
                BetEntity objBet;
                if (ValidBetWithRouletteExist(betRequest.RouletteId))
                    objBet = GetOneBet(betRequest.RouletteId);
                else
                    objBet = CreateBet(betRequest.RouletteId);
                RegisterGambling(objBet, betRequest, gamblerId);
                GamblerEntity objGambler = gamblerModel.GetOneGambler(gamblerId);
                gamblerModel.UpdateGamblerCredits(gamblerId, objGambler.Credits - betRequest.CreditsBet);

                return objBet.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
