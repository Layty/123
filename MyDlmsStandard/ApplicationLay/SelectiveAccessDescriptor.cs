using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay
{
    public class SelectiveAccessDescriptor : IToPduStringInHex, IPduStringInHexConstructor
    {
        public AxdrIntegerUnsigned8 AccessSelector { get; set; }
        public AccessParameters AccessParameters { get; set; }

        public SelectiveAccessDescriptor()
        {
        }

        public SelectiveAccessDescriptor(AxdrIntegerUnsigned8 accessSelector, DlmsDataItem dlmsDataItem)
        {
            AccessSelector = accessSelector;
            AccessParameters = new AccessParameters() { Data = dlmsDataItem };
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            AccessSelector = new AxdrIntegerUnsigned8();
            if (!AccessSelector.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            AccessParameters = new AccessParameters();
            if (!AccessParameters.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }

        public string ToPduStringInHex()
        {
            return AccessSelector.ToPduStringInHex() + AccessParameters.ToPduStringInHex();
        }
    }
}