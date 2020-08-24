using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using System;
using System.Linq;

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
