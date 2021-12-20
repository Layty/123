using MyDlmsStandard;


namespace JobMaster.Services
{

    /// <summary>
    /// 数据层
    /// </summary>
    public class SendData : IToPduStringInHex
    {
        public SendData(IToPduStringInHex handlerData)
        {
            HandlerHexData = handlerData;
        }

        private IToPduStringInHex HandlerHexData;

        public string ToPduStringInHex()
        {
            return HandlerHexData.ToPduStringInHex();
        }
    }
}