using System.Threading.Tasks;

namespace JobMaster.Services
{
    public interface ILinkLayer
    {
        Task<byte[]> SendAsync(string sendHexString);
        Task<byte[]> SendAsync(byte[] sendBytes);
    }
}