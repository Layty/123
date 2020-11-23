using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public interface ISetRequest
    {
        SetRequestType SetRequestType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }

        string ToPduStringInHex();
    }
}