using System;
using System.Data.OleDb;
using System.ServiceModel;

namespace ConsoleWcfServer
{
    [ServiceContract]
    public interface ILogin
    {
        [OperationContract]
        bool Login(string userName, string password);
    }
}