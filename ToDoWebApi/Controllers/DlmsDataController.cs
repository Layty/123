using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoWebApi.Core;

namespace ToDoWebApi.Controllers
{
    [Route("api/[controller]/[action]"), ApiController]
    public class DlmsDataController : ControllerBase
    {
        private readonly DlmsDataContext context;

        public DlmsDataController(DlmsDataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IEnumerable<DlmsData> GetDlmsDataList()
        {
            return context.DlmsDatas.ToList();
        }

        [HttpGet]
        public async Task<DlmsData> GetDlmsDataById(int id)
        {
            return await context.DlmsDatas.FirstOrDefaultAsync(t => Equals(t.Id, id));
        }

        [HttpPost]
        public async Task<bool> Add([FromBody] DlmsData dlmsData)
        {
            if (dlmsData != null)
            {
//                var d = await context.DlmsDatas.FirstOrDefaultAsync(t => Equals(t.LogicName, dlmsData.LogicName));
//                var b = context.DlmsDatas.FindAsync(dlmsData.LogicName);
//

                await context.AddAsync(dlmsData);
                return await context.SaveChangesAsync() > 0;
            }

            return false;
        }

        [HttpPost]
        public async Task<bool> Update(DlmsData dlmsData)
        {
            var newDlmsData = await context.DlmsDatas.FirstOrDefaultAsync(t => Equals(t.Id, dlmsData.Id));
            if (newDlmsData != null)
            {
                newDlmsData = dlmsData;
                return await context.SaveChangesAsync() > 0;
            }

            return false;
        }

        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            var newDlmsData = await context.DlmsDatas.FirstOrDefaultAsync(t => Equals(t.Id, id));
            if (newDlmsData != null)
            {
                context.DlmsDatas.Remove(newDlmsData);
                return await context.SaveChangesAsync() > 0;
            }

            return false;
        }
    }
}