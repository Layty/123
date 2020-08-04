using System;
using System.Linq;
using System.Text;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class GetResponse : IPduBytesToConstructor, IToPduBytes
    {
        public Command Command { get; set; } = Command.GetResponse;
        public GetResponseNormal GetResponseNormal { get; set; }
        public GetResponseWithDataBlock GetResponseWithDataBlock { get; set; }
        public GetResponseWithList GetResponseWithList { get; set; }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes.Length == 0)
            {
                return false;
            }

            if (pduBytes[0] == (byte) Command)
            {
                if (pduBytes[1] == (byte) GetResponseType.Normal)
                {
                    GetResponseNormal = new GetResponseNormal();
                    return GetResponseNormal.PduBytesToConstructor(pduBytes.Skip(1).ToArray());
                }

                if (pduBytes[1] == (byte) GetResponseType.WithDataBlock)
                {
                    GetResponseWithDataBlock = new GetResponseWithDataBlock();
                    return GetResponseWithDataBlock.PduBytesToConstructor(pduBytes);
                }

                if (pduBytes[1] == (byte) GetResponseType.WithList)
                {
                }
            }

            return false;
        }

        public byte[] ToPduBytes()
        {
            throw new NotImplementedException();
        }
    }

    public class GetResponseWithDataBlock : IPduBytesToConstructor
    {
        public GetResponseType GetResponseType { get; set; } = GetResponseType.WithDataBlock;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
        public DataBlockG DataBlockG { get; set; }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes[0] != (byte) GetResponseType)
            {
                return false;
            }

            InvokeIdAndPriority = new Invoke_Id_And_Priority();
            InvokeIdAndPriority.UpdateInvokeIdAndPriority(pduBytes[1]);
            InvokeIdAndPriority.InvokeIdAndPriority = InvokeIdAndPriority.GetInvoke_Id_And_Priority();

            DataBlockG = new DataBlockG();

            return true;
        }
    }

    public class GetResponseWithList : IPduBytesToConstructor
    {
        public Command Command { get; set; } = Command.GetResponse;
        public GetResponseType GetResponseType { get; set; } = GetResponseType.WithList;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }

        public GetDataResult[] Result;

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes[0] != (byte) Command)
            {
                return false;
            }

            if (pduBytes[1] != (byte) GetResponseType)
            {
                return false;
            }

            InvokeIdAndPriority = new Invoke_Id_And_Priority();
            InvokeIdAndPriority.UpdateInvokeIdAndPriority(pduBytes[2]);
            InvokeIdAndPriority.InvokeIdAndPriority = InvokeIdAndPriority.GetInvoke_Id_And_Priority();

            Result = new GetDataResult[] { };
            return true;
        }
    }

    public class GetResponseNormal : IPduBytesToConstructor
    {
        public GetResponseType GetResponseType { get; set; } = GetResponseType.Normal;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }

        public GetDataResult GetDataResult { get; set; }

        public bool PduBytesToConstructor(byte[] GetResponseNormalByte)
        {
            if (GetResponseNormalByte[0] != (byte) GetResponseType)
            {
                return false;
            }

            InvokeIdAndPriority = new Invoke_Id_And_Priority();
            InvokeIdAndPriority.UpdateInvokeIdAndPriority(GetResponseNormalByte[1]);
            InvokeIdAndPriority.InvokeIdAndPriority = InvokeIdAndPriority.GetInvoke_Id_And_Priority();


            GetDataResult = new GetDataResult();
            GetDataResult.Data = new DLMSDataItem();
            if (GetResponseNormalByte[2] != 0)
            {
                GetDataResult.DataAccessResult = (ErrorCode) GetResponseNormalByte[3];

                return true;
            }
            else if (GetResponseNormalByte[2] == 0)
            {
                GetDataResult.DataAccessResult = (ErrorCode) GetResponseNormalByte[2];
                var dataTypeBytes = GetResponseNormalByte.Skip(2).Take(2).Reverse().ToArray();
                var dt = BitConverter.ToInt16(dataTypeBytes, 0);
                var result = "";
                switch (dt)
                {
                    case (byte) DataType.UInt8:
                        GetDataResult.Data =
                            new DLMSDataItem(DataType.UInt8,
                                GetResponseNormalByte.Skip(4).Take(1).ToArray()[0].ToString());
                        break;
                    case (byte) DataType.UInt16:
                        result = BitConverter.ToUInt16(GetResponseNormalByte.Skip(4).Take(2).Reverse().ToArray(), 0)
                            .ToString();
                        GetDataResult.Data = new DLMSDataItem(DataType.UInt16, result);
                        break;
                    case (byte) DataType.Int16:
                        result = BitConverter.ToInt16(GetResponseNormalByte.Skip(4).Take(2).Reverse().ToArray(), 0)
                            .ToString();
                        GetDataResult.Data = new DLMSDataItem(DataType.Int16, result);
                        break;
                    case (byte) DataType.Int32:

                        result = BitConverter.ToInt32(GetResponseNormalByte.Skip(4).Take(4).Reverse().ToArray(), 0)
                            .ToString();
                        GetDataResult.Data = new DLMSDataItem(DataType.Int32, result);
                        break;
                    case (byte) DataType.UInt32:
                        result = BitConverter.ToUInt32(GetResponseNormalByte.Skip(4).Take(4).Reverse().ToArray(), 0)
                            .ToString();
                        GetDataResult.Data = new DLMSDataItem(DataType.UInt32, result);
                        break;
                    case (byte) DataType.OctetString:
                        if ((GetResponseNormalByte[4] & 0x80) == 0x80)
                        {
                            var index = GetResponseNormalByte[4] - 0x80;
                            var range = BitConverter.ToInt16(
                                GetResponseNormalByte.Skip(5).Take(index).Reverse().ToArray(),
                                0);
                            var resultBytes = GetResponseNormalByte.Skip(5 + index).Take(range).ToArray();
                            result = Encoding.Default.GetString(resultBytes);
                            GetDataResult.Data = new DLMSDataItem(DataType.OctetString, result);
                        }
                        else
                        {
                            result = GetResponseNormalByte.Skip(5).Take(GetResponseNormalByte[4]).ToArray()
                                .ByteToString();
//                            result =Encoding.Default.GetString(GetResponseNormalByte.Skip(5).Take(GetResponseNormalByte[4]).ToArray()) ;
                            GetDataResult.Data = new DLMSDataItem(DataType.OctetString, result);
                        }

                        break;
                    case (byte) DataType.Structure:
                        var rangeStruct = GetResponseNormalByte.Skip(4).Take(1).ToArray()[0];
                        result = GetResponseNormalByte.Skip(5).ToArray().ByteToString(); //返回结构体
                        GetDataResult.Data = new DLMSDataItem(DataType.Structure, result);
                        break;
                    case (byte) DataType.Enum:
                        result = GetResponseNormalByte.Skip(4).Take(1).ToArray()[0].ToString();
                        GetDataResult.Data = new DLMSDataItem(DataType.Enum, result);
                        break;
                    case (byte) DataType.Array:
                        result = GetResponseNormalByte.Skip(4).ToArray().ByteToString();
                        GetDataResult.Data = new DLMSDataItem(DataType.Array, result);
                        break;
                    case (byte) DataType.BitString:
                        result = GetResponseNormalByte.Skip(4).Take(2).ToArray().ByteToString();
                        GetDataResult.Data = new DLMSDataItem(DataType.BitString, result);
                        break;
                    case (byte)DataType.String:
                        result = GetResponseNormalByte.Skip(5).Take(GetResponseNormalByte[4]).ToArray().ByteToString();
                        GetDataResult.Data=new DLMSDataItem(DataType.String, result);
                        break;
                }
            }


            return true;
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