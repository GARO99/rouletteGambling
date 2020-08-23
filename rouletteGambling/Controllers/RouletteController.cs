﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;
using rouletteGambling.Utils.Validations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rouletteGambling.Controllers
{
    [ApiController]
    [Route("roulette")]
    public class RouletteController : ControllerBase
    {
        private readonly RouletteModel rouletteModel;
        private readonly RouletteValidation rouletteValidation;

        public RouletteController(IDistributedCache distributedCache)
        {
            rouletteModel = new RouletteModel(distributedCache);
            rouletteValidation = new RouletteValidation(distributedCache);
        }

        [HttpGet]
        [Route("getallroulette")]
        public ActionResult GetAllRoulette()
        {
            try
            {
                List<RouletteEntity> objRoulettes = rouletteModel.GetRoulettes();
                var objResponseRoulettes = from roulettes in objRoulettes
                                           select new
                                           {
                                               roulettes.Id,
                                               Status = Enum.GetName(typeof(StatusRouletteEnum), Convert.ToInt32(roulettes.Status))
                                           };

                return Ok(objResponseRoulettes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("createroulette")]
        public ActionResult CreateRoulette()
        {
            try
            {
                int Id = rouletteModel.CreateRoulette();

                return Ok(Id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Route("openroulette/{id}")]
        public ActionResult OpenRoulette(int id)
        {
            try
            {
                if (!rouletteValidation.ValidRouletteExist(id))
                    return NotFound(ErrorEnum.ERROR_ROULETTE_NOT_EXIST.ToString());
                bool successTransaction = rouletteModel.UpdateStatusRoulette(id, true);

                return Ok(successTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
