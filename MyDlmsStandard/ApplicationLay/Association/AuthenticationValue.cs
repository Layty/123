using System;
using System.Collections.Generic;
using MyDlmsStandard.Ber;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class AuthenticationValue : IToPduStringInHex
    {
        public string Value { get; set; }
        public BerBitString BitString { get; set; }
        public BerGraphicString CharString { get; set; }

        private byte[] passwordHex;

        public AuthenticationValue()
        {
        }

        public AuthenticationValue(byte[] passwordHex)
        {
            this.passwordHex = passwordHex;
        }


        public string ToPduStringInHex()
        {
            string text = "";
            if (CharString != null)
            {
                text = "80" + CharString.ToPduStringInHex();
            }
            else if (BitString != null)
            {
                text = "81" + BitString.ToPduStringInHex();
            }

            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            return (text.Length / 2).ToString("X2") + text;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            int num = Convert.ToInt32(pduStringInHex.Substring(0, 2), 16);
            if (num * 2 + 2 > pduStringInHex.Length)
            {
                return false;
            }

            pduStringInHex = pduStringInHex.Substring(2);
            if (pduStringInHex.StartsWith("80"))
            {
                pduStringInHex = pduStringInHex.Substring(2);
                CharString = new BerGraphicString();
                return CharString.PduStringInHexConstructor(ref pduStringInHex);
            }

            if (pduStringInHex.StartsWith("81"))
            {
                pduStringInHex = pduStringInHex.Substring(2);
                BitString = new BerBitString();
                return BitString.PduStringInHexConstructor(ref pduStringInHex);
            }

            return false;
        }
    }
}