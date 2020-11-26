using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;

namespace MyDlmsNetCore.ApplicationLay.Set
{
    public interface ISetResponse
    {
        SetResponseType SetResponseType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        bool PduStringInHexConstructor(ref string pduStringInHex);
    }
}