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
        private readonly DlmsDataContext _context;

        public DlmsDataController(DlmsDataContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IEnumerable<DlmsData> GetDlmsDataList()
        {
            return _context.DlmsDataItems.ToList();
        }

        [HttpGet]
        public async Task<DlmsData> GetDlmsDataById(int id)
        {
            return await _context.DlmsDataItems.FirstOrDefaultAsync(t => Equals(t.Id, id));
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
                var newDlmsData = await _context.DlmsDataItems.SingleOrDefaultAsync(t => Equals(t.Id, dlmsData.Id));
                if (newDlmsData == null)
                {
                    return NotFound();
                }
                else
                {
                    _context.DlmsDataItems.Update(newDlmsData);
                    await _context.SaveChangesAsync();
                    return Ok(newDlmsData);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var newDlmsData = await _context.DlmsDataItems.SingleOrDefaultAsync(t => Equals(t.Id, id));
            if (newDlmsData == null)
            {
                return NotFound();
            }
            else
            {
                _context.DlmsDataItems.Remove(newDlmsData);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }
    }
}