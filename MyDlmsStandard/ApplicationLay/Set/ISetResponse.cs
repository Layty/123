using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public interface ISetResponse
    {
        SetResponseType SetResponseType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        bool PduStringInHexConstructor(ref string pduStringInHex);
    }
}