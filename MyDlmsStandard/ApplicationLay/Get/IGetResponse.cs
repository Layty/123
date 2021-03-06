using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public interface IGetResponse
    {
        GetResponseType GetResponseType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        bool PduStringInHexConstructor(ref string pduStringInHex);
    }
}