using System.ServiceModel;

namespace WcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IUserLogin”。
    [ServiceContract]
    public interface IUserLogin
    {
        [OperationContract]
        bool Login(string userName, string password);
    }
}
