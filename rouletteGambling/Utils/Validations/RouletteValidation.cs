using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Utils.Validations
{
    public class RouletteValidation
    {
        private readonly RouletteModel rouletteModel;

        public RouletteValidation(IDistributedCache distributedCache)
        {
            rouletteModel = new RouletteModel(distributedCache);
        }

        public bool ValidRouletteExist(int id)
        {
            try
            {
                if (rouletteModel.GetRoulettes().Any(r => r.Id == id))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidRouletteIsOpen(int id, List<RouletteEntity> objRoulettes)
        {
            try
            {
                RouletteEntity objRoulette = objRoulettes.Where(r => r.Id == id).FirstOrDefault();
                if (objRoulette != null)
                {
                    if (objRoulette.Status)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
