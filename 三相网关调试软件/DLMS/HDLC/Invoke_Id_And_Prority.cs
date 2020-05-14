using System;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.HDLC
{
    public class Invoke_Id_And_Priority
    {
        //Invoke-Id-And-Priority::= BIT STRING(SIZE(8))
        //--{
        //    --  invoke_id(0..3),
        //    --  reserved(4..5),
        //    --  service_class(6),     0 = Unconfirmed, 1 = Confirmed
        //        --  priority(7)      0 = normal; 1 = high
        //        --}

        private byte invokeID;

        public byte InvokeID
        {
            get => invokeID;
            set
            {
                if (value > 15)
                {
                    throw new ArgumentException("Invalid InvokeID");
                }

                invokeID = value;
            }
        }

        public byte reserved { get; set; } //(4..5)
        public ServiceClass ServiceClass { get; set; } //0 = Unconfirmed, 1 = Confirmed
        public Priority Priority { get; set; } //0=normal,1=high
        public byte InvokeIdAndPriority { get; set; } = 0xC1;

        /// <summary>
        ///  BIT STRING (SIZE(8))
        /// </summary>
        /// <param name="ivoke_id">(0..3)</param>
        /// <param name="serviceClass">0 = Unconfirmed, 1 = Confirmed</param>
        /// <param name="priority">优先级 0=normal,1=high</param>
        /// <param name="reserved"></param>
        public Invoke_Id_And_Priority(byte ivoke_id, ServiceClass serviceClass, Priority priority, byte reserved = 0)
        {
            InvokeID = ivoke_id;
            this.reserved = reserved;
            ServiceClass = serviceClass;
            Priority = priority;
            InvokeIdAndPriority = GetInvoke_Id_And_Priority();
        }

        internal void UpdateInvokeId(byte value)
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

            InvokeID = (byte) (value & 0xF);
        }

        private byte GetInvoke_Id_And_Priority()
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

            count += InvokeID;

            return count;
        }
    }
}