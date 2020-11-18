using System;
using System.Text;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard
{
    public class DLMSCommon
    {
        public static DataType GetDLMSDataType(Type type)
        {
            if (type == null)
            {
                return DataType.NullData;
            }
            if (type == typeof(int))
            {
                return DataType.Int32;
            }
            if (type == typeof(uint))
            {
                return DataType.UInt32;
            }
            if (type == typeof(string))
            {
                return DataType.VisibleString;
            }
            if (type == typeof(byte))
            {
                return DataType.UInt8;
            }
            if (type == typeof(sbyte))
            {
                return DataType.Int8;
            }
            if (type == typeof(short))
            {
                return DataType.Int16;
            }
            if (type == typeof(ushort))
            {
                return DataType.UInt16;
            }
            if (type == typeof(long))
            {
                return DataType.Int64;
            }
            if (type == typeof(ulong))
            {
                return DataType.UInt64;
            }
            if (type == typeof(float))
            {
                return DataType.Float32;
            }
            if (type == typeof(double))
            {
                return DataType.Float64;
            }
            //if (type == typeof(DateTime) || type == typeof(GXDateTime))
            //{
            //    return DataType.DateTime;
            //}
            //if (type == typeof(GXDate))
            //{
            //    return DataType.Date;
            //}
            //if (type == typeof(GXTime))
            //{
            //    return DataType.Time;
            //}
            if (type == typeof(bool))
            {
                return DataType.Boolean;
            }
            if (type == typeof(byte[]))
            {
                return DataType.OctetString;
            }
            //if (type == typeof(GXStructure))
            //{
            //    return DataType.Structure;
            //}
            //if (type == typeof(GXArray) || type == typeof(object[]))
            //{
            //    return DataType.Array;
            //}
            //if (type == typeof(GXEnum) || type.IsEnum)
            //{
            //    return DataType.Enum;
            //}
            //if (type == typeof(GXBitString))
            //{
            //    return DataType.BitString;
            //}
            //if (type == typeof(GXByteBuffer))
            //{
            //    return DataType.OctetString;
            //}
            throw new Exception("Failed to convert data type to DLMS data type. Unknown data type.");
        }

        internal static void ToBitString(StringBuilder sb, byte value, int count)
        {
            if (count <= 0)
            {
                return;
            }
            if (count > 8)
            {
                count = 8;
            }
            for (int num = 7; num != 8 - count - 1; num--)
            {
                if ((value & (1 << num)) != 0)
                {
                    sb.Append('1');
                }
                else
                {
                    sb.Append('0');
                }
            }
        }

    }
}