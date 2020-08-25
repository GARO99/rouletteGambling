using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Validations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Services
{
    public class RouletteService
    {
        private readonly RouletteModel rouletteModel;
        private readonly RouletteValidation rouletteValidation;
        public string ErrorMessage { get; set; }

        public RouletteService(IDistributedCache distributedCache)
        {
            rouletteModel = new RouletteModel(distributedCache);
            rouletteValidation = new RouletteValidation(distributedCache);
        }

        public IEnumerable<object> GetAllRoulette()
        {
            try
            {
                List<RouletteEntity> objRoulettes = rouletteModel.GetRoulettes();
                IEnumerable<object> objResponseRoulettes = from roulettes in objRoulettes
                                                           select new
                                                           {
                                                               roulettes.Id,
                                                               Status = Enum.GetName(typeof(StatusRouletteEnum), Convert.ToInt32(roulettes.Status))
                                                           };
                return objResponseRoulettes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CreateRoulette()
        {
            int roulettesId = 0;
            try
            {
                List<RouletteEntity> objRoulettes = rouletteModel.GetRoulettes();
                if (objRoulettes.Count > 0)
                    roulettesId = objRoulettes.Max(r => r.Id) + 1;
                else
                    roulettesId++;
                return rouletteModel.CreateRoulette(roulettesId, status: false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool OpenRoulette(int id)
        {
            try
            {
                if (!rouletteValidation.ValidRouletteExist(id))
                {
                    ErrorMessage = ErrorEnum.ERROR_ROULETTE_NOT_EXIST.ToString();
                    return false;
                }

                return rouletteModel.UpdateRoulette(id, status: true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
