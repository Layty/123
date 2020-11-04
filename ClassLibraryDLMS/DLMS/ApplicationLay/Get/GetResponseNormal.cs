﻿using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;
using ClassLibraryDLMS.DLMS.Axdr;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Get
{
    public class GetResponseNormal : IToPduStringInHex,IPduStringInHexConstructor
    {
        [XmlIgnore] public GetResponseType GetResponseType { get; set; } = GetResponseType.Normal;
        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }

        public GetDataResult Result { get; set; }

        public string ToPduStringInHex()
        {
            return InvokeIdAndPriority.ToPduStringInHex() +Result.ToPduStringInHex();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
          
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

            InvokeIdAndPriority = new AxdrUnsigned8();
            if (!InvokeIdAndPriority.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            Result = new GetDataResult();
            if (!Result.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            return true;
        }
    }
}