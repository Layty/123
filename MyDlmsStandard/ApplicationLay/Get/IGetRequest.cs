using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public interface IGetRequest
    {
        GetRequestType GetRequestType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        string ToPduStringInHex();
    }
}