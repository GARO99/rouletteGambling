using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Utils.Validations
{
    public class GamblingValidation
    {
        private readonly GamblingModel gamblingModel;

        public GamblingValidation(IDistributedCache distributedCache)
        {
            gamblingModel = new GamblingModel(distributedCache);
        }

        public bool ValidGamblerAlreadyBetOnGambling(int betId, string gamblerId)
        {
            try
            {
                if (gamblingModel.GetOneGambling(betId, gamblerId) != null)
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
