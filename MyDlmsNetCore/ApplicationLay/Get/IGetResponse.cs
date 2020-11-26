using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;

namespace MyDlmsNetCore.ApplicationLay.Get
{
    public interface IGetResponse
    {
        GetResponseType GetResponseType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        bool PduStringInHexConstructor(ref string pduStringInHex);
    }
}