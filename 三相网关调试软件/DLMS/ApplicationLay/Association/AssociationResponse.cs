using System;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Association
{
    public class AssociationResponse : IPduBytesToConstructor
    {
        public ApplicationContextName ApplicationContextName { get; set; }
        public MechanismName MechanismName { get; set; }
        public CallingAuthenticationValue CallingAuthenticationValue { get; set; }
        public ResultSourceDiagnostic ResultSourceDiagnostic { get; set; }
        public AssociationResult AssociationResult { get; set; }
        public VaaName VaaName { get; set; }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes[0] != (byte) Command.Aare)
            {
                return false;
            }

            if (pduBytes[1] < 16 || pduBytes[1] + 1 > pduBytes.Length)
            {
                return false;
            }

            var data = pduBytes.Skip(2).ToArray();
            while (data.Length != 0)
            {
                var sw = data[0] & 0x1F;
                switch (sw)
                {
                    case 0:
                        ;
                        break;
                    case 1:
                        ApplicationContextName = new ApplicationContextName();
                        
                        if (!ApplicationContextName.PduBytesToConstructor(data.Skip(1).Take(10).ToArray()))
                        {
                            return false;
                        }

                        data = data.Skip(11).ToArray();
                        break;
                    case 2:
                        AssociationResult = new AssociationResult();
                        if (AssociationResult.PduBytesToConstructor(data.Take(4).ToArray()))
                        {

                        }

                        ;
                        break;
                }
            }


            return true;
        }
    }

    public class VaaName
    {
        public Int16 Value { get; set; }
    }
}