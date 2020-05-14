using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace 三相智慧能源网关调试软件.DLMS.HDLC
{
    public class HDLCInfo : ValidateModelBase
    {
        internal enum InfoTag : byte
        {
            MaxInfoTx = 0x05,
            MaxInfoRx = 0x06,
            WindowSizeTx = 0x07,
            WindowSizeRx = 0x08
        }
      
        public HDLCInfo()
        {
            _formatIdentifier = 0x81;
            _groupIdentifier = 0x80;
            TransmitMaxInfoValue = 1024;
            ReceiveMaxInfoValue = 1024;
            TransmitMaxWindowSize = 1u;
            ReceiveMaxWindowSize = 7u;
            _transmitMaxWindowFrameLength = 4;
            _receiveMaxWindowFrameLength = 4;
        }
      
        private readonly byte _formatIdentifier;

        /// <summary>
        /// 组标识符
        /// </summary>
        private readonly byte _groupIdentifier;

        private byte _groupLength;

        private ushort _transmitMaxInfoValue;

        public ushort TransmitMaxInfoValue
        {
            get => _transmitMaxInfoValue;
            set
            {
                _transmitMaxInfoValue = value;
                if (TransmitMaxInfoValue > 0 && TransmitMaxInfoValue <= 128)
                {
                    _transmitMaxInfoFrameLength = 1;
                }
                else if (TransmitMaxInfoValue > 128)
                {
                    _transmitMaxInfoFrameLength = 2;
                }

                RaisePropertyChanged();
            }
        }

        private ushort _transmitMaxInfoFrameLength;

        private ushort _receiveMaxInfoValue;

        public ushort ReceiveMaxInfoValue
        {
            get => _receiveMaxInfoValue;
            set
            {
                _receiveMaxInfoValue = value;
                if (ReceiveMaxInfoValue > 0 && ReceiveMaxInfoValue <= 128)
                {
                    _receiveMaxInfoFrameLength = 1;
                }
                else if (ReceiveMaxInfoValue > 128)
                {
                    _receiveMaxInfoFrameLength = 2;
                }

                RaisePropertyChanged();
            }
        }

        private ushort _receiveMaxInfoFrameLength;

        private uint _transmitMaxWindowSize;

        public uint TransmitMaxWindowSize
        {
            get => _transmitMaxWindowSize;
            set
            {
                bool flag = value <= 7u;
                if (flag)
                {
                    _transmitMaxWindowSize = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _transmitMaxWindowFrameLength;


        private uint _receiveMaxWindowSize;

        [Required]
        public uint ReceiveMaxWindowSize
        {
            get => _receiveMaxWindowSize;
            set
            {
                bool flag = value <= 7u;
                if (flag)
                {
                    _receiveMaxWindowSize = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _receiveMaxWindowFrameLength;

        public bool InformationEnable { get; set; }

        public byte[] GetSnrmInfo()
        {
            List<byte> info = new List<byte>
            {
                _formatIdentifier,
                _groupIdentifier
            };
            //发送最大信息域参数信息
            info.Add((byte) InfoTag.MaxInfoTx);
            info.Add((byte) _transmitMaxInfoFrameLength);
            info.AddRange(BitConverter.GetBytes(TransmitMaxInfoValue).Reverse());

            //接收最大信息域参数信息
            info.Add((byte) InfoTag.MaxInfoRx);
            info.Add((byte) _receiveMaxInfoFrameLength);
            info.AddRange(BitConverter.GetBytes(ReceiveMaxInfoValue).Reverse());
            //发送最大窗口参数信息
            info.Add((byte) InfoTag.WindowSizeTx);
            info.Add((byte) _transmitMaxWindowFrameLength);
            info.AddRange(BitConverter.GetBytes(TransmitMaxWindowSize).Reverse());
            //接收最大窗口参数信息
            info.Add((byte) InfoTag.WindowSizeRx);
            info.Add((byte) _receiveMaxWindowFrameLength);
            info.AddRange(BitConverter.GetBytes(ReceiveMaxWindowSize).Reverse());
            _groupLength = (byte) (info.Count - 2);
            info.Insert(2, _groupLength);
            return info.ToArray();
        }
    }
}