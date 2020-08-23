using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Validations;
using System;
using System.Collections.Generic;

namespace rouletteGambling.Controllers
{
    [ApiController]
    [Route("gambler")]
    public class GamblerController : ControllerBase
    {
        private readonly GamblerModel gamblerModel;
        private readonly GamblerValidation gamblerValidation;

        public GamblerController(IDistributedCache distributedCache)
        {
            gamblerModel = new GamblerModel(distributedCache);
            gamblerValidation = new GamblerValidation(distributedCache);
        }

        [HttpGet]
        [Route("getgambler")]
        public ActionResult GetGambler()
        {
            try
            {
                List<GamblerEntity> objGamblers = gamblerModel.GetGambler();

                return Ok(objGamblers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("creategambler")]
        public ActionResult CreateGambler([FromBody] GamblerEntity objGamblerRequest)
        {
            try
            {
                if (objGamblerRequest == null)
                    return BadRequest(ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString());
                if (string.IsNullOrEmpty(objGamblerRequest.Id) || string.IsNullOrEmpty(objGamblerRequest.FullName))
                    return BadRequest(ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString());
                if (objGamblerRequest.Credits <= 0)
                    return BadRequest(ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString());
                bool successTransaction = gamblerModel.CreateGambler(objGamblerRequest);

                return Ok(successTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{credits}")]
        [Route("updategamblercredits/{credits}")]
        public ActionResult UpdateGamblerCredits([FromHeader] string id, decimal credits)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest(ErrorEnum.ERROR_REQUEST_INCOMPLETE.ToString());
                if (!gamblerValidation.ValidGamblerExist(id))
                    return BadRequest(ErrorEnum.ERROR_GAMBLER_NOT_EXIST.ToString());
                bool successTransaction = gamblerModel.UpdateGamblerCredits(id, credits);

                return Ok(successTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
