using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class ExceptionResponse : IPduStringInHexConstructor, IDlmsCommand
    {
        [XmlIgnore] public Command Command => Command.ExceptionResponse;

        public StateError StateError { get; set; }
        public ServiceError ServiceError { get; set; }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

            if (pduStringInHex.Substring(0, 2) != "D8")
            {
                return false;
            }

            pduStringInHex = pduStringInHex.Substring(2);
            StateError = new StateError();
            if (!StateError.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            ServiceError = new ServiceError();
            if (!ServiceError.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }

    public class StateError
    {
        public const int ServiceNotAllowed = 1;

        public const int ServiceUnknown = 2;
        [XmlAttribute] public string Value { get; set; }

        public bool PduStringInHexConstructor(ref string Data)
        {
            switch (Data.Substring(0, 2))
            {
                case "01":
                    Value = "ServiceNotAllowed";
                    break;
                case "02":
                    Value = "ServiceUnknown";
                    break;
                default:
                    return false;
            }

            Data = Data.Substring(2);
            return true;
        }
    }

    public class ServiceError
    {
        public const int OperationNotPossible = 1;

        public const int ServiceNotSupported = 2;

        public const int OtherReason = 3;

        public const int PduTooLong = 4;

        public const int DecipheringError = 5;

        public const int InvocationCounterError = 6;
        [XmlAttribute] public string Value { get; set; }

        public bool PduStringInHexConstructor(ref string Data)
        {
            switch (Data.Substring(0, 2))
            {
                case "01":
                    Value = "OperationNotPossible";
                    break;
                case "02":
                    Value = "ServiceNotSupported";
                    break;
                case "03":
                    Value = "OtherReason";
                    break;
                case "04":
                    Value = "PduTooLong";
                    break;
                case "05":
                    Value = "DecipheringError";
                    break;
                case "06":
                    Value = "InvocationCounterError";
                    break;
                default:
                    return false;
            }

            Data = Data.Substring(2);
            return true;
        }
    }
}