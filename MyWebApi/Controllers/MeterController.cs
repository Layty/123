using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.Entities;
using MyWebApi.Services;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class MeterController : ControllerBase
    {
        private readonly IMeterRepository _dbContext;

        public MeterController(IMeterRepository dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Meter>> GetAllMeter()
        {
            return await _dbContext.GetMeters();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveByMeterId(string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentNullException(nameof(meterId));
            }

            var entity = await _dbContext.GetMeter(meterId);
            if (entity==null)
            {
                return NotFound();
            }
            _dbContext.RemoveMeter(meterId);
            await _dbContext.SaveAsync();
            return NoContent();
        }

        [HttpGet("ByMeterId/{meterId}")]
        public async Task<Meter> GetDataByMeterId(string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentException(nameof(meterId));
            }

            return await _dbContext.GetDataAsync(meterId);
        }

        [HttpGet("GetEnergyDataByMeterId/{meterId}")]
        public async Task<IEnumerable<Energy>> GetEnergyDataByMeterId(string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentException(nameof(meterId));
            }

            return await _dbContext.GetEnergyDataAsync(meterId);
        }

        [HttpPost("CreateEnergyData{meterId}")]
        public async Task<ActionResult<Energy>> CreateEnergyData(string meterId, [FromBody] List<Energy> energy)
        {
            if (meterId == "")
            {
                return BadRequest();
            }

            if (meterId == null)
            {
                return BadRequest();
            }

            if (!await _dbContext.MeterIdExistsAsync(meterId))
            {
                return BadRequest();
            }

            _dbContext.AddEnergyData(meterId, energy);
            await _dbContext.SaveAsync();
            return Ok(energy);
        }

        [HttpPost("CreatePowerData{meterId}")]
        public async Task<ActionResult<Energy>> CreatePowerData(string meterId, [FromBody] List<Power> powers)
        {
            if (meterId == "")
            {
                return BadRequest();
            }

            if (meterId == null)
            {
                return BadRequest();
            }

            if (!await _dbContext.MeterIdExistsAsync(meterId))
            {
                return BadRequest();
            }

            _dbContext.AddPowerData(meterId, powers);
            await _dbContext.SaveAsync();
            return Ok(powers);
        }

        [HttpPost("CreateMeter{meterId}")]
        public async Task<ActionResult<Meter>> CreateMeter(string meterId, [FromBody] Meter meterData)
        {
            if (meterId == "")
            {
                return BadRequest();
            }

            if (meterId == null)
            {
                return BadRequest();
            }

            if (await _dbContext.MeterIdExistsAsync(meterId))
            {
                return BadRequest();
            }

            _dbContext.AddMeter(meterId);
            await _dbContext.SaveAsync();
            return Ok(meterData);
        }
    }
}