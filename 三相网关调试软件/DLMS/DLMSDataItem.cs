using System.Collections.Generic;
using System.Text;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;

namespace 三相智慧能源网关调试软件.DLMS
{
    public class DLMSArray:IToPduBytes
    {
        public DLMSDataItem[] items { get; set; }

        public byte[] ToPduBytes()
        {
            //List<byte> array=new List<byte>();
            //array.Add((byte)DataType.Array);
            //array.Add((byte)items.Length);
           
            string str = "01";
            string str2 = (items.Length <= 127) ? items.Length.ToString("X2") : ((items.Length > 255) ? ("82" + items.Length.ToString("X4")) : ("81" + items.Length.ToString("X2")));
            StringBuilder stringBuilder = new StringBuilder();
            DLMSDataItem[] array = items;
            foreach (DLMSDataItem dlmsDataItem in array)
            {
                stringBuilder.Append(dlmsDataItem.ToPduBytes());
            }
            return (str + str2 + stringBuilder.ToString()).StringToByte();
        }
    }

    public class DLMSDataItem:IToPduBytes
    {
        public DataType DataType { get; set; }
        public byte[] ValueBytes { get; set; }

        public DLMSDataItem(DataType dataType, byte[] valueBytes)
        {
            DataType = dataType;
            ValueBytes = valueBytes;
        }
        public byte[] ToPduBytes()
        {
            var list = new List<byte> {(byte) DataType};
            list.AddRange(ValueBytes);
            return list.ToArray();
        }
    }
}