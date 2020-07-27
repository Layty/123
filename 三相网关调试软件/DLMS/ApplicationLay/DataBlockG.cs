using System.Linq;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class DataBlockG:IPduBytesToConstructor
    {
        public bool IsLastBlock { get; set; }
        public uint BlockNumber { get; set; }
        public string RawData { get; set; }
        public byte DataAccessResult { get; set; }


        public bool PduBytesToConstructor(byte[] pduDataBlockGBytes)
        {
            if (pduDataBlockGBytes[0]==0x00)
            {
                IsLastBlock = false;
            }
            else if (pduDataBlockGBytes[0] == 0x01)
            {
                IsLastBlock = true;
            }
//            BlockNumber= pduDataBlockGBytes.Skip(1).Take(4)


            return true;
        }
    }
}