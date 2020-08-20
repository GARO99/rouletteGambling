using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using rouletteGambling.Models;
using rouletteGambling.Models.Entities;
using rouletteGambling.Utils.Enums;

namespace rouletteGambling.Controllers
{
    [ApiController]
    [Route("roulette")]
    public class RouletteController : ControllerBase
    {
        private readonly RouletteModel rouletteModel;

        public RouletteController(IDistributedCache distributedCache)
        {
            rouletteModel = new RouletteModel(distributedCache);
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
                bool rouletteExist = rouletteModel.ValidRouletteExist(id);
                if (!rouletteExist)
                    return NotFound(ErrorEnum.ERROR_ROULETTE_NOT_EXIST.ToString());
                bool successTransaction = rouletteModel.OpenRoulette(id);

                return Ok(successTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
