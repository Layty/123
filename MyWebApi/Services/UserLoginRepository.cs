using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using System;
using System.Threading.Tasks;

namespace MyWebApi.Services
{
    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly UserLoginDbContext _userLoginDbContext;


        public UserLoginRepository(UserLoginDbContext userLoginDbContext)
        {
            _userLoginDbContext = userLoginDbContext;
        }

        public async Task<bool> Login(string userName, string password)
        {
            if (userName == string.Empty)
            {
                throw new ArgumentException(nameof(userName));
            }

            if (password == string.Empty)
            {
                throw new ArgumentException(nameof(password));
            }

            var t2 = await _userLoginDbContext.UserLogins.FirstOrDefaultAsync(t =>
                t.UserName.Equals(userName) && t.Password.Equals(password));
            return t2 != null;

        }
    }
}