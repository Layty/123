﻿using System;
using System.ComponentModel;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;

namespace 三相智慧能源网关调试软件.Model
{
    public class DLMSSelfDefineProfileGeneric : ProfileGeneric
    {
        public DLMSSelfDefineProfileGeneric(string logicalName) : base(logicalName)
        {
        }
    }

    public class DLMSSelfDefineRegisterModel : DLMSRegister
    {
        public string RegisterName { get; set; }

        public DLMSSelfDefineRegisterModel(string logicName) : base(logicName)
        {
        }
    }

    public class DLMSSelfDefineData : DLMSData
    {
        public string DataName { get; set; }
 
        public ErrorCode LastResult
        {
            get => _lastResult;
            set
            {
                _lastResult = value;
                OnPropertyChanged();
            }
        }

        private ErrorCode _lastResult;

        public DLMSSelfDefineData(string logicalName) : base(logicalName)
        {
        }
    }
}