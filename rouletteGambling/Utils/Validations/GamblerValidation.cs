using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Utils.Validations
{
    public class GamblerValidation
    {
        private readonly GamblerModel gamblerModel;

        public GamblerValidation(IDistributedCache distributedCache)
        {
            gamblerModel = new GamblerModel(distributedCache);
        }

        public bool ValidGamblerExist(string id)
        {
            try
            {
                if (gamblerModel.GetGambler().Any(g => g.Id == id))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
