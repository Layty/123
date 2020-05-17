using System;
using System.Collections.Generic;
using System.Linq;

namespace 三相智慧能源网关调试软件.Model.IIC
{
    public class IicCommonDataConvertor
    {
        public static string ValueConvertor(IEnumerable<byte> dataBytes, (string Unit, int Scalar) unitAndScalar,
            int length = 4, bool isUnsigned = true)
        {
            dynamic result = 0;
            switch (length)
            {
                case 4 when isUnsigned:
                    result = BitConverter.ToUInt32(dataBytes.Take(length).Reverse().ToArray(), 0);
                    break;
                case 4:
                    result = BitConverter.ToInt32(dataBytes.Take(length).Reverse().ToArray(), 0);
                    break;
                case 2 when isUnsigned:
                    result = BitConverter.ToUInt16(dataBytes.Take(length).Reverse().ToArray(), 0);
                    break;
                case 2:
                    result = BitConverter.ToInt16(dataBytes.Take(length).Reverse().ToArray(), 0);
                    break;
                case 1 when isUnsigned:
                    result = Convert.ToByte(dataBytes.Take(length).ToArray()[0]);
                    break;
                case 1:
                    result = Convert.ToSByte(dataBytes.Take(length).ToArray()[0]);
                    break;
            }

            string resultString = HandlerUnitScalar(result, unitAndScalar);

            return resultString;
        }

        private static string HandlerUnitScalar(dynamic result, (string Unit, int Scalar) unitAndScalar)
        {
            string resultString = result.ToString();
            switch (unitAndScalar.Scalar)
            {
                case 0:
                    resultString = (result).ToString() + unitAndScalar.Unit;
                    break;
                case 1:
                    resultString = (result * 0.1).ToString() + unitAndScalar.Unit;
                    break;
                case 2:
                    resultString = (result * 0.01).ToString() + unitAndScalar.Unit;
                    break;
                case 3:
                    resultString = (result * 0.001).ToString() + unitAndScalar.Unit;
                    break;
                case 4:
                    resultString = (result * 0.0001).ToString() + unitAndScalar.Unit;
                    break;
            }

            return resultString;
        }
    }
}