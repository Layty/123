using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public interface IGetRequest
    {

        /// <summary>
        /// 请求类型 ，分3种
        /// </summary>
        GetRequestType GetRequestType { get; }

        /// <summary>
        /// 优先级
        /// </summary>
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }

        /// <summary>
        /// 组包
        /// </summary>
        /// <returns></returns>
        string GetRequestToPduStringInHex();
    }
}