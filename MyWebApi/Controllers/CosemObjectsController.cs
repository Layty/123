using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApi.Entities;
using MyWebApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyWebApi.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class CosemObjectsController : ControllerBase
    {
        private readonly ICosemRepository _cosemRepository;
        private readonly ILogger<CosemObjectsController> _logger;

        public CosemObjectsController(ICosemRepository cosemRepository,ILogger<CosemObjectsController> logger)
        {
            _cosemRepository = cosemRepository;
            _logger = logger;
        }

        // GET: api/<CosemObjectsController>
        [HttpGet]
        public async Task<IActionResult> GetAllCosemObjects()
        {
            var cosemObjects = await _cosemRepository.GetCosemObjectsAsync();
            _logger.LogInformation("有人请求了GetAllCosemObjects");
            Console.WriteLine("有人请求了GetAllCosemObjects");
            return new JsonResult(cosemObjects);
        }

        // GET api/<CosemObjectsController>/5
        [HttpGet("ByObis/{obis}")]
        public async Task<CosemObject> GetCosemObjectByObis(string obis)
        {
            if (obis == String.Empty)
            {
                throw new ArgumentException(nameof(obis));
            }

            return await _cosemRepository.GetCosemObjectAsync(obis);
        }

        // GET api/<CosemObjectsController>/5
        [HttpGet("ByName/{name}")]
        public async Task<IEnumerable<CosemObject>> GetCosemObjectByName(string name)
        {
            if (name == String.Empty)
            {
                throw new ArgumentException(nameof(name));
            }

            return await _cosemRepository.GetCosemObjectsByNameAsync(name);
        }

        [HttpGet("ByClassId/{classId}")]
        public async Task<IEnumerable<CosemObject>> GetCosemObjectByClassId(int classId)
        {
            if (classId == 0)
            {
                throw new ArgumentException(nameof(classId));
            }

            return await _cosemRepository.GetCosemObjectsByClassIdAsync(classId);
        }

        // POST api/<CosemObjectsController>
        [HttpPost("{Obis}")]
        public async Task<ActionResult<CosemObject>> CreateCosemObject(string obis, [FromBody] CosemObject cosemObject)
        {
            if (obis == "")
            {
                return BadRequest();
            }

            if (cosemObject == null)
            {
                return BadRequest();
            }

            if (await _cosemRepository.CosemObjectExistsAsync(cosemObject.Obis))
            {
                return BadRequest();
            }

            _cosemRepository.AddCosemObject(cosemObject);
            await _cosemRepository.SaveAsync();
            return Ok(cosemObject);
        }

        [HttpPut("{obis}")]
        public async Task<ActionResult<CosemObject>> Update(string obis, [FromBody] CosemObject cosemObject)
        {
            if (cosemObject == null)
            {
                return BadRequest();
            }
            else
            {
                var isExist = await _cosemRepository.CosemObjectExistsAsync(obis);
                if (!isExist)
                {
                    return NotFound();
                }
                else
                {
                    var todo = await _cosemRepository.GetCosemObjectAsync(obis);
                    todo.ClassId = cosemObject.ClassId;
                    todo.Name = cosemObject.Name;
                    _cosemRepository.UpdateCosemObject(todo);
                    await _cosemRepository.SaveAsync();
                    return Ok(todo);
                }
            }
        }

        // DELETE api/<CosemObjectsController>/5
        [HttpDelete("{obis}")]
        public async Task<IActionResult> DeleteCosemObjectByObis(string obis)
        {
            var entity = await _cosemRepository.GetCosemObjectAsync(obis);
            if (entity == null)
            {
                return NotFound();
            }

            _cosemRepository.DeleteCosemObject(entity);
            await _cosemRepository.SaveAsync();
            return NoContent();
        }
    }
}