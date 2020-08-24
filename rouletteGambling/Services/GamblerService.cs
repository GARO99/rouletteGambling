using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Validations;
using System;
using System.Collections.Generic;

namespace rouletteGambling.Services
{
    public class GamblerService
    {
        private readonly GamblerModel gamblerModel;
        private readonly GamblerValidation gamblerValidation;
        public string ErrorMessage { get; set; }

        public GamblerService(IDistributedCache distributedCache)
        {
            gamblerModel = new GamblerModel(distributedCache);
            gamblerValidation = new GamblerValidation(distributedCache);
        }

        public List<GamblerEntity> GetGambler()
        {
            try
            {
                return gamblerModel.GetGambler();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CreateGambler(GamblerEntity objGamblerRequest)
        {
            try
            {
                if (objGamblerRequest == null || string.IsNullOrEmpty(objGamblerRequest.Id) || 
                    string.IsNullOrEmpty(objGamblerRequest.FullName) || objGamblerRequest.Credits <= 0)
                {
                    ErrorMessage = ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString();
                    return false;
                }
                if (gamblerValidation.ValidGamblerExist(objGamblerRequest.Id)) 
                    return false;

                return gamblerModel.CreateGambler(objGamblerRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateGamblerCredits(string id, decimal credits)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ErrorMessage = ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString();
                    return false;
                }
                if (!gamblerValidation.ValidGamblerExist(id))
                {
                    ErrorMessage = ErrorEnum.ERROR_GAMBLER_NOT_EXIST.ToString();
                    return false;
                }

                return gamblerModel.UpdateGamblerCredits(id, credits);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
