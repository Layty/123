using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Association;

namespace 三相智慧能源网关调试软件.DLMS.Wrapper
{
    public class NetFrameMaker
    {
        private MyDLMSSettings MyDlmsSettings { get; set; }

        private NetFrame _netFrame;

        public NetFrameMaker(MyDLMSSettings settings)
        {
            MyDlmsSettings = settings;
        }


        public byte[] AarqRequest()
        {
            List<byte> aarq = new List<byte>();
            aarq.AddRange(new AssociationRequest(MyDlmsSettings).ToPduBytes());
            aarq.InsertRange(0, new byte[] {(byte) Command.Aarq, (byte) aarq.Count});
            _netFrame = new NetFrame(MyDlmsSettings, aarq.ToArray());
            return _netFrame.ToPduBytes();
        }


        public byte[] BuildPduRequestBytes(byte[] pduBytes)
        {
            _netFrame = new NetFrame(MyDlmsSettings, pduBytes);
            return _netFrame.ToPduBytes();
        }


        public byte[] ReleaseRequest()
        {
            List<byte> alrq = new List<byte>();
            alrq.InsertRange(0, new byte[] {(byte) Command.ReleaseRequest, (byte) alrq.Count});
            _netFrame = new NetFrame(MyDlmsSettings, alrq.ToArray());
            return _netFrame.ToPduBytes();
        }
    }
}