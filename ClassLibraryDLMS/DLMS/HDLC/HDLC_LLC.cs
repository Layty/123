namespace ClassLibraryDLMS.DLMS.HDLC
{
    internal class HdlcLlc
    {
        //LLC:逻辑链路控制(子层)(LogicalLinkControl(Sub-layer));
        //LLC 子层唯一的作用是保证一致的数据链路寻址
        //LSAP:LLC子层服务接入点(LLCsub-layerServiceAccessPoint);
        private const byte DestinationLsap = 0xe6;
        private const byte SourceLsapRequest = 0xe6;
        private const byte SourceLsapResponse = 0xe7;
        private const byte LlcQuality = 0x00;

        /// <summary>
        /// 发送
        /// </summary>
        internal static readonly byte[]
            LLCSendBytes = new byte[3] {DestinationLsap, SourceLsapRequest, LlcQuality}; //0xE6,0xE6,0x00
        /// <summary>
        /// 服务端应答返回时使用
        /// </summary>
        internal static readonly byte[]
            LLCReplyBytes = new byte[3] {DestinationLsap, SourceLsapResponse, LlcQuality}; //0xE6,0xE7,0x00


        public byte[] Information;//n*8bit 

   
    }
}