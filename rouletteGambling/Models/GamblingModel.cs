using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<BetResultEntity> GetBetResults()
        {
            try
            {
                List<BetResultEntity> objBets = redisCache.GetBetResultFromRedis();

                return objBets;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BetEntity GetOneBet(int rouletteId)
        {
            try
            {
                List<BetEntity> objBets = GetBets();
                return objBets.Where(b => b.RouletteId == rouletteId && b.Status == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BetResultEntity GetOneBetResult(int betId)
        {
            try
            {
                List<BetResultEntity> objBets = GetBetResults();
                return objBets.Where(b => b.BetId == betId).FirstOrDefault();
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

        public List<GamblingEntity> GetGamblingxBet(int betId)
        {
            List<GamblingEntity> objGamblings = GetGamblings();
            return objGamblings.Where(g => g.BetId == betId).ToList();
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

        public bool ValidCloseBetRequest(CloseBetRequest closeBetRequest)
        {
            try
            {
                if (closeBetRequest.BetResultNumber < (int)RulesBetEnum.MinBetNumber || closeBetRequest.BetResultNumber > (int)RulesBetEnum.MaxBetNumber)
                    return false;
                if (closeBetRequest.BetResultColor != (int)ColorBetEnum.Black && closeBetRequest.BetResultColor != (int)ColorBetEnum.Red)
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
                GamblerEntity objGambler = gamblerModel.GetOneGambler(gamblerId);
                if ((objGambler.Credits - betRequest.CreditsBet) < 0)
                {
                    ErrorMessage = ErrorEnum.ERROR_GAMBLER_DOES_NOT_HAVE_CREDITS.ToString();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidCloseBetData(int rouletteId, CloseBetRequest closeBetRequest)
        {
            try
            {
                if (!ValidCloseBetRequest(closeBetRequest))
                {
                    ErrorMessage = ErrorEnum.ERROR_RULES_BET.ToString();
                    return false;
                }
                if (!rouletteModel.ValidRouletteExist(rouletteId))
                {
                    ErrorMessage = ErrorEnum.ERROR_ROULETTE_NOT_EXIST.ToString();
                    return false;
                }
                if (!rouletteModel.ValidRouletteIsOpen(rouletteId, rouletteModel.GetRoulettes()))
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

        public void RegisterGambling(BetEntity objBet, BetRequest betRequest, string gamblerId)
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

        public bool ValidGamblerAlreadyBet(int betId, string gamblerId)
        {
            try
            {
                GamblingEntity objGambling = GetOneGambling(betId, gamblerId);
                if (objGambling != null)
                    return true;

                return false;
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
                if (ValidGamblerAlreadyBet(objBet.Id, gamblerId))
                    return 0;
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

        public void RegisterBetResult(int betId, CloseBetRequest closeBetRequest)
        {
            try
            {
                List<BetResultEntity> betResults = GetBetResults();
                betResults.Add(new BetResultEntity
                {
                    BetId = betId,
                    Number = closeBetRequest.BetResultNumber,
                    Color = closeBetRequest.BetResultColor
                });
                redisCache.SetBetResultToRedis(betResults);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RegistarGamblingResult(int betId, CloseBetRequest closeBetRequest)
        {
            try
            {
                List<GamblingEntity> objGamblings = GetGamblings();
                (from gamblings in objGamblings
                 where gamblings.BetId == betId && (gamblings.BetColor == closeBetRequest.BetResultColor || gamblings.BetNumber == closeBetRequest.BetResultNumber)
                 select gamblings).ToList().ForEach(g => g.WonBet = true);
                redisCache.SetGamblingToRedis(objGamblings);
                objGamblings = GetGamblings();
                (from gamblings in objGamblings
                 where gamblings.BetId == betId && gamblings.WonBet == null
                 select gamblings).ToList().ForEach(g => g.WonBet = false);
                redisCache.SetGamblingToRedis(objGamblings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CloseBet(int rouletteId, CloseBetRequest closeBetRequest)
        {
            try
            {
                if (!rouletteModel.CloseRoulette(rouletteId))
                    return 0;
                List<BetEntity> objBets = GetBets();
                BetEntity objBet = GetOneBet(rouletteId);
                (from bets in objBets
                 where bets.RouletteId == rouletteId && bets.Status == true
                 select bets).ToList().ForEach(b => b.Status = false);
                redisCache.SetBetsToRedis(objBets);
                RegisterBetResult(objBet.Id, closeBetRequest);
                RegistarGamblingResult(objBet.Id, closeBetRequest);

                return objBet.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
