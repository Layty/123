﻿using MyDlmsStandard.Common;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class AssociationResult
    {
        [XmlAttribute] public string Value { get; set; }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes[0] != 0xA2)
            {
                return false;
            }

            var pduStringInHex = pduBytes.Skip(1).ToArray().ByteToString();

            if (!pduStringInHex.StartsWith("0302"))
                return false;
            else
            {
                int num = Convert.ToInt32(pduStringInHex.Substring(0, 2), 16);
                if ((num + 1) * 2 > pduStringInHex.Length)
                {
                    return false;
                }

                Value = pduStringInHex.Substring(2, num * 2);
                pduStringInHex = pduStringInHex.Substring((num + 1) * 2);
                return true;
            }
        }
    }
}