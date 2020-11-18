using System;
using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.ApplicationLay
{
    public class InvokeIdAndPriority
    {
        //Invoke-Id-And-Priority::= BIT STRING(SIZE(8))
        //--{
        //    --  invoke_id(0..3),
        //    --  reserved(4..5),
        //    --  service_class(6),     0 = Unconfirmed, 1 = Confirmed
        //        --  priority(7)      0 = normal; 1 = high
        //        --}

        private byte _invokeId;

        [XmlAttribute]
        public byte InvokeId
        {
            get => _invokeId;
            set
            {
                if (value > 15)
                {
                    throw new ArgumentException("Invalid InvokeID");
                }

                _invokeId = value;
            }
        }

        [XmlAttribute] public byte Reserved { get; set; } //(4..5)
        [XmlAttribute] public ServiceClass ServiceClass { get; set; } //0 = Unconfirmed, 1 = Confirmed
        [XmlAttribute] public Priority Priority { get; set; } //0=normal,1=high

     

        [XmlAttribute]
        public string OriginalHexValue { get; set; }
        // public byte Value { get; set; } = 0xC1;
        [XmlAttribute]
        public byte Value
        {
            get => _value;
            set
            {
                _value = value;
                OriginalHexValue = value.ToString("X2");
            }
        }

        private byte _value = 0xC1;
        /// <summary>
        ///  BIT STRING (SIZE(8))
        /// </summary>
        /// <param name="invokeId">(0..3)</param>
        /// <param name="serviceClass">0 = Unconfirmed, 1 = Confirmed</param>
        /// <param name="priority">优先级 0=normal,1=high</param>
        /// <param name="reserved"></param>
        public InvokeIdAndPriority(byte invokeId, ServiceClass serviceClass, Priority priority, byte reserved = 0)
        {
            InvokeId = invokeId;
            this.Reserved = reserved;
            ServiceClass = serviceClass;
            Priority = priority;
            Value = GetInvoke_Id_And_Priority();
        }

        public InvokeIdAndPriority()
        {
        }
        public InvokeIdAndPriority(byte value)
        {
            Value = value;
            UpdateInvokeIdAndPriority(value);
        }
        public void UpdateInvokeIdAndPriority(byte value)
        {
            if ((value & 0x80) != 0)
            {
                Priority = Priority.High;
            }
            else
            {
                Priority = Priority.Normal;
            }

            if ((value & 0x40) != 0)
            {
                ServiceClass = ServiceClass.Confirmed;
            }
            else
            {
                ServiceClass = ServiceClass.UnConfirmed;
            }

            InvokeId = (byte) (value & 0xF);
            Value = GetInvoke_Id_And_Priority();
        }

        public byte GetInvoke_Id_And_Priority()
        {
            byte count = 0;
            if (Priority == Priority.High)
            {
                count += 0x80;
            }

            if (ServiceClass == ServiceClass.Confirmed)
            {
                count += 0x40;
            }

            count += InvokeId;

            return count;
        }
    }
}