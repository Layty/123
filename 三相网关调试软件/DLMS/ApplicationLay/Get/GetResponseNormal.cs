using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
{
    public class ExceptionResponse : IPduBytesToConstructor
    {
        [XmlIgnore] public Command Command { get; set; } = Command.ExceptionResponse;

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes[0] != (byte) Command)
            {
                return false;
            }

            return
                true;
            
        }
    }

    public class Response : IPduBytesToConstructor
    {
        public Command Command { get; set; }
        public GetResponse GetResponse { get; set; }
        public ExceptionResponse ExceptionResponse { get; set; }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes.Length == 0)
            {
                return false;
            }

            if (pduBytes[0] == (byte) Command.ExceptionResponse)
            {
                ExceptionResponse = new ExceptionResponse();
                return ExceptionResponse.PduBytesToConstructor(pduBytes);
            }

            if (pduBytes[0] == (byte) Command.GetResponse)
            {
                GetResponse = new GetResponse();
                return GetResponse.PduBytesToConstructor(pduBytes);
            }

            return true;
        }
    }


    public class GetResponseNormal : IPduBytesToConstructor
    {
        [XmlIgnore] public GetResponseType GetResponseType { get; set; } = GetResponseType.Normal;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }

        public GetDataResult Result { get; set; }

        public bool PduBytesToConstructor(byte[] GetResponseNormalByte)
        {
            if (GetResponseNormalByte[0] != (byte) GetResponseType)
            {
                return false;
            }

            InvokeIdAndPriority = new InvokeIdAndPriority(GetResponseNormalByte[1]);
            
            Result = new GetDataResult();
            Result.Data = new DLMSDataItem();
            if (GetResponseNormalByte[2] != 0)
            {
                Result.DataAccessResult = (ErrorCode) GetResponseNormalByte[3];

                return true;
            }
            else if (GetResponseNormalByte[2] == 0)
            {
                Result.DataAccessResult = (ErrorCode) GetResponseNormalByte[2];
                var dataTypeBytes = GetResponseNormalByte.Skip(2).Take(2).Reverse().ToArray();
                var dt = BitConverter.ToInt16(dataTypeBytes, 0);
                var result = "";
                switch (dt)
                {
                    case (byte) DataType.UInt8:
                        Result.Data =
                            new DLMSDataItem(DataType.UInt8,
                                GetResponseNormalByte.Skip(4).Take(1).ToArray()[0].ToString());
                        break;
                    case (byte) DataType.UInt16:
                        result = BitConverter.ToUInt16(GetResponseNormalByte.Skip(4).Take(2).Reverse().ToArray(), 0)
                            .ToString();
                        Result.Data = new DLMSDataItem(DataType.UInt16, result);
                        break;
                    case (byte) DataType.Int16:
                        result = BitConverter.ToInt16(GetResponseNormalByte.Skip(4).Take(2).Reverse().ToArray(), 0)
                            .ToString();
                        Result.Data = new DLMSDataItem(DataType.Int16, result);
                        break;
                    case (byte) DataType.Int32:

                        result = BitConverter.ToInt32(GetResponseNormalByte.Skip(4).Take(4).Reverse().ToArray(), 0)
                            .ToString();
                        Result.Data = new DLMSDataItem(DataType.Int32, result);
                        break;
                    case (byte) DataType.UInt32:
                        result = BitConverter.ToUInt32(GetResponseNormalByte.Skip(4).Take(4).Reverse().ToArray(), 0)
                            .ToString();
                        Result.Data = new DLMSDataItem(DataType.UInt32, result);
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
                            Result.Data = new DLMSDataItem(DataType.OctetString, result);
                        }
                        else
                        {
                            result = GetResponseNormalByte.Skip(5).Take(GetResponseNormalByte[4]).ToArray()
                                .ByteToString();
//                    
                            Result.Data = new DLMSDataItem(DataType.OctetString, result);
                        }

                        break;
                    case (byte) DataType.Structure:
                        var rangeStruct = GetResponseNormalByte.Skip(4).Take(1).ToArray()[0];
                        result = GetResponseNormalByte.Skip(5).ToArray().ByteToString(); //返回结构体
                        Result.Data = new DLMSDataItem(DataType.Structure, result);
                        break;
                    case (byte) DataType.Enum:
                        result = GetResponseNormalByte.Skip(4).Take(1).ToArray()[0].ToString();
                        Result.Data = new DLMSDataItem(DataType.Enum, result);
                        break;
                    case (byte) DataType.Array:
                        result = GetResponseNormalByte.Skip(4).ToArray().ByteToString();
                        Result.Data = new DLMSDataItem(DataType.Array, result);
                        break;
                    case (byte) DataType.BitString:
                        result = GetResponseNormalByte.Skip(4).ToArray().ByteToString();
                        Result.Data = new DLMSDataItem(DataType.BitString, result);
                        break;
                    case (byte) DataType.String:
                        result = GetResponseNormalByte.Skip(5).Take(GetResponseNormalByte[4]).ToArray().ByteToString();
                        Result.Data = new DLMSDataItem(DataType.String, result);
                        break;
                    case (byte) DataType.Boolean:
                        result = GetResponseNormalByte.Skip(4).Take(1).ToArray()[0].ToString();
                        Result.Data = new DLMSDataItem(DataType.Boolean, result);
                        break;
                }
            }


            return true;
        }
    }
}