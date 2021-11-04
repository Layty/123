using Microsoft.AspNetCore.Mvc;
using MyWebApi.Entities;
using MyWebApi.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class MeterController : ControllerBase
    {
        private readonly IMeterRepository _meterRepository;

        public MeterController(IMeterRepository meterRepository)
        {
            _meterRepository = meterRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Meter>> GetAllMeter()
        {
            return await _meterRepository.GetMeters();
        }
        [HttpPost("{meterId}")]
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

            if (await _meterRepository.MeterIdExistsAsync(meterId))
            {
                return BadRequest();
            }

            _meterRepository.AddMeter(meterId);
            await _meterRepository.SaveAsync();
            return Ok(meterData);
        }
        [HttpDelete("{meterId}")]
        public async Task<IActionResult> RemoveByMeterId([FromRoute] string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentNullException(nameof(meterId));
            }

            var entity = await _meterRepository.GetMeter(meterId);
            if (entity == null)
            {
                return NotFound();
            }
            _meterRepository.DeleteMeter(meterId);
            await _meterRepository.SaveAsync();
            return NoContent();
        }

        [HttpGet("{meterId}")]
        public async Task<Meter> GetMeterByMeterId(string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentException(nameof(meterId));
            }

            return await _meterRepository.GetDataAsync(meterId);
        }

        [HttpGet("EnergyData/{meterId}")]
        public async Task<IEnumerable<Energy>> GetEnergyDataByMeterId(string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentException(nameof(meterId));
            }

            return await _meterRepository.GetEnergyDataAsync(meterId);
        }

        [HttpPost("EnergyData/{meterId}")]
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

            if (!await _meterRepository.MeterIdExistsAsync(meterId))
            {
                return BadRequest();
            }

            _meterRepository.AddEnergyData(meterId, energy);
            await _meterRepository.SaveAsync();
            return Ok(energy);
        }

        [HttpPost("PowerData/{meterId}")]
        public async Task<ActionResult<Power>> CreatePowerData(string meterId, [FromBody] List<Power> powers)
        {
            if (meterId == "")
            {
                return BadRequest();
            }

            if (meterId == null)
            {
                return BadRequest();
            }

            if (!await _meterRepository.MeterIdExistsAsync(meterId))
            {
                return BadRequest();
            }

            _meterRepository.AddPowerData(meterId, powers);
            await _meterRepository.SaveAsync();
            return Ok(powers);
        }


    }
}