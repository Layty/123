using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;

namespace MyDlmsNetCore.ApplicationLay.Get
{
    public interface IGetRequest
    {
        GetRequestType GetRequestType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        string ToPduStringInHex();
    }
}