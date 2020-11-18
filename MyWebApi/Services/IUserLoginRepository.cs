using System.Threading.Tasks;

namespace MyWebApi.Services
{
    public interface IUserLoginRepository
    {
        Task<bool> Login(string userName, string password);
    }
}