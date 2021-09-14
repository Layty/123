using MyDlmsStandard.ApplicationLay.Release;

namespace MyDlmsStandard.ApplicationLay.Get
{

    public class Response : IPduStringInHexConstructor
    {
        public ReleaseResponse ReleaseResponse { get; set; }
        public GetResponse GetResponse { get; set; }
        public ExceptionResponse ExceptionResponse { get; set; }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            string a = pduStringInHex.Substring(0, 2);
            if (a == "C4")
            {
                GetResponse = new GetResponse();
                return GetResponse.PduStringInHexConstructor(ref pduStringInHex);
            }

            if (a=="63")
            {
                ReleaseResponse=new ReleaseResponse();
                ReleaseResponse.PduStringInHexConstructor(ref pduStringInHex)
            }

            if (a == "D8")
            {
                ExceptionResponse = new ExceptionResponse();
                return ExceptionResponse.PduStringInHexConstructor(ref pduStringInHex);
            }

            return false;
        }
    }
}