using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;

namespace MyDlmsNetCore.ApplicationLay.Set
{
    public interface ISetRequest
    {
        SetRequestType SetRequestType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }

        string ToPduStringInHex();
    }
}