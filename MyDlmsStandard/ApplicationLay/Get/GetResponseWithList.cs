using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetResponseWithList : IToPduStringInHex, IPduStringInHexConstructor,IGetResponse
    {
        [XmlIgnore] public GetResponseType GetResponseType { get; set; } = GetResponseType.WithList;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }

        public GetDataResult[] Result;

        public string ToPduStringInHex()
        {
			StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("03");
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            int num = Result.Length;
            if (num <= 127)
            {
                stringBuilder.Append(num.ToString("X2"));
            }
            else if (num <= 255)
            {
                stringBuilder.Append("81" + num.ToString("X2"));
            }
            else
            {
                stringBuilder.Append("82" + num.ToString("X4"));
            }
            GetDataResult[] array = Result;
            foreach (GetDataResult getDataResult in array)
            {
                stringBuilder.Append(getDataResult.ToPduStringInHex());
            }
            return stringBuilder.ToString();
		}

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }
         
            InvokeIdAndPriority = new AxdrIntegerUnsigned8();
            if (!InvokeIdAndPriority.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            int num = MyConvert.DecodeVarLength(ref pduStringInHex);
            Result = new GetDataResult[num];
            for (int i = 0; i < num; i++)
            {
                if (string.IsNullOrEmpty(pduStringInHex))
                {
                    return false;
                }
                Result[i] = new GetDataResult();
                if (!Result[i].PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }
            }
            return true;
        }
    }
}