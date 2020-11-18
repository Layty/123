using System.Text;

namespace MyDlmsStandard.ApplicationLay
{
    public class DLMSDate
    {
        public DLMSDate()
        {
            
        }
    }
    public class DLMSTime
    {
        public DLMSTime()
        {

        }
    }
    public class DLMSBitString
    {
        public string Value { get; set; }

        public DLMSBitString()
        {
            
        }

        public DLMSBitString(string value)
        {
            Value = value;
        }

        public DLMSBitString(byte[] value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte value2 in value)
            {
                DLMSCommon.ToBitString(stringBuilder, value2, 8);
            }
            Value = stringBuilder.ToString();
        }
        public DLMSBitString(byte[] value, int index, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte value2 in value)
            {
                if (index != 0)
                {
                    index--;
                    continue;
                }
                if (count < 1)
                {
                    break;
                }
                DLMSCommon.ToBitString(stringBuilder, value2, count);
                count -= 8;
            }
            Value = stringBuilder.ToString();
        }
        public DLMSBitString(byte value, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            DLMSCommon.ToBitString(stringBuilder, value, 8);
            if (count != 8)
            {
                stringBuilder.Remove(count, 8 - count);
            }
            Value = stringBuilder.ToString();
        }
    }
}