using Microsoft.AspNetCore.Mvc;
using MyWebApi.Services;
using System;
using System.Threading.Tasks;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private readonly IUserLoginRepository _dbContext;

        public UserLoginController(IUserLoginRepository dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(string userName, string password)
        {
            var result = await _dbContext.Login(userName, password);
            if (result)
            {
                Console.WriteLine($"{DateTime.Now} {userName} 登录成功");
                return Ok();
            }
            else
            {
                Console.WriteLine($"{DateTime.Now} {userName} 不存在");
                return NotFound();
            }
        }
    }
}