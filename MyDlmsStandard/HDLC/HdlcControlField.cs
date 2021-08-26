namespace MyDlmsStandard.HDLC
{
    /// <summary>
    /// 该区域长度为1byte，用于指示命令和回应的类型，同时还包含了数据帧的帧序号
    /// </summary>
    public class HdlcControlField
    {
        /// <summary>
        /// 当前接收帧序号
        /// </summary>
        public int CurrentReceiveSequenceNumber
        {
            get => _currentReceiveSequenceNumber;
            set
            {
                bool flag = value >= 8;
                _currentReceiveSequenceNumber = flag ? 0 : value;
            }
        }
        /// <summary>
        /// 当前发送帧序号
        /// </summary>
        public int CurrentSendSequenceNumber
        {
            get => _currentSendSequenceNumber;
            set
            {
                bool flag = value >= 8;
                _currentSendSequenceNumber = flag ? 0 : value;
            }
        }

        private int _currentReceiveSequenceNumber;

        private int _currentSendSequenceNumber;
    }
}