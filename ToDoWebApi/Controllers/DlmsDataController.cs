using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DlmsWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DlmsWebApi.Controllers
{
    [Route("api/[controller]/[action]"), ApiController]
    public class DlmsDataController : ControllerBase
    {
        private readonly CosemContext _context;

        public DlmsDataController(CosemContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IEnumerable<DlmsData> GetAll()
        {
            return _context.CosemItems.ToList();
        }


//        [HttpGet]
//        public async Task<DlmsData> GetDlmsDataById(int id)
//        {
//            return await _context.DlmsDataItems.FirstOrDefaultAsync(t => Equals(t.Id, id));
//        }


        [HttpGet]
        public async Task<DlmsData> Get(string obis)
        {
            return await _context.CosemItems.FirstOrDefaultAsync(t => Equals(t.LogicName, obis));
        }


        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DlmsData dlmsData)
        {
            if (dlmsData == null)
            {
                return BadRequest();
            }
            else
            {
                await _context.AddAsync(dlmsData);
                await _context.SaveChangesAsync();
                return Ok(dlmsData);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(DlmsData dlmsData)
        {
            if (dlmsData == null)
            {
                return BadRequest();
            }
            else
            {
                var newDlmsData = await _context.CosemItems.SingleOrDefaultAsync(t => Equals(t.Id, dlmsData.Id));
                if (newDlmsData == null)
                {
                    return NotFound();
                }
                else
                {
                    _context.CosemItems.Update(newDlmsData);
                    await _context.SaveChangesAsync();
                    return Ok(newDlmsData);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string obis)
        {
            var newDlmsData = await _context.CosemItems.SingleOrDefaultAsync(t => Equals(t.LogicName, obis));
            if (newDlmsData == null)
            {
                return NotFound();
            }
            else
            {
                _context.CosemItems.Remove(newDlmsData);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }
    }
}