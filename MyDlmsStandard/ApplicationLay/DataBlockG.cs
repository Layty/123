using MyDlmsStandard.Axdr;
using System.Text;

namespace MyDlmsStandard.ApplicationLay
{
    public class DataBlockG : IToPduStringInHex, IPduStringInHexConstructor
    {
        /// <summary>
        ///LastBlock是bool类型，Value为 00代表false ,01代表true
        /// 当LastBlock=00时代表未传完，
        /// 当LastBlock=00时代表此帧为最后一块数据
        /// </summary>
        public AxdrIntegerBoolean LastBlock { get; set; }

        /// <summary>
        /// 当前的块数号
        /// </summary>
        public AxdrIntegerUnsigned32 BlockNumber { get; set; }

        /// <summary>
        /// 未加工的数据，最终完成所有块传输后，需要将所有RawData进行拼接后解析
        /// </summary>
        public AxdrOctetString RawData { get; set; }
        /// <summary>
        /// 数据访问结果，为00代表失败，01代表成功
        /// </summary>
        public AxdrIntegerUnsigned8 DataAccessResult { get; set; }


        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(LastBlock.ToPduStringInHex());
            stringBuilder.Append(BlockNumber.ToPduStringInHex());
            if (DataAccessResult != null)
            {
                stringBuilder.Append("01");
                stringBuilder.Append(DataAccessResult.ToPduStringInHex());
            }
            else if (RawData != null)
            {
                stringBuilder.Append("00");
                stringBuilder.Append(RawData.ToPduStringInHex());
            }

            return stringBuilder.ToString();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

            LastBlock = new AxdrIntegerBoolean();
            if (!LastBlock.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            BlockNumber = new AxdrIntegerUnsigned32();
            if (!BlockNumber.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            string a = pduStringInHex.Substring(0, 2);
            if (a == "00")
            {
                pduStringInHex = pduStringInHex.Substring(2);
                RawData = new AxdrOctetString();
                return RawData.PduStringInHexConstructor(ref pduStringInHex);
            }

            if (a == "01")
            {
                pduStringInHex = pduStringInHex.Substring(2);
                DataAccessResult = new AxdrIntegerUnsigned8();
                return DataAccessResult.PduStringInHexConstructor(ref pduStringInHex);
            }

            return false;
        }
    }
}