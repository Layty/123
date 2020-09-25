using System.Collections.Generic;
using NLog;
using 三相智慧能源网关调试软件.ViewModel;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;


namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Association
{
    public class AssociationRequest : IToPduBytes
    {
        public ApplicationContextName ApplicationContextName { get; set; }
        public CallingAPTitle CallingAPTitle { get; set; }
        public SenderACSERequirements SenderACSERequirements { get; set; }
        public MechanismName MechanismName { get; set; }
        public CallingAuthenticationValue CallingAuthenticationValue { get; set; }

        public InitiateRequest InitiateRequest { get; set; }

        public AssociationRequest()
        {
        }

        public AssociationRequest(byte[] passWorld, ushort maxReceivePduSize, byte dlmsVersion,string systemTitle)
        {
            ApplicationContextName = new ApplicationContextName();
            CallingAPTitle = new CallingAPTitle(systemTitle);
            SenderACSERequirements = new SenderACSERequirements();
            MechanismName = new MechanismName();
            CallingAuthenticationValue = new CallingAuthenticationValue(passWorld);
            InitiateRequest = new InitiateRequest(maxReceivePduSize, dlmsVersion);
        }

        public AssociationRequest(DLMSSettingsViewModel dlmsSettingsViewModel)
        {
            ApplicationContextName = new ApplicationContextName();
            MechanismName = new MechanismName();
            SenderACSERequirements = new SenderACSERequirements();
            CallingAuthenticationValue = new CallingAuthenticationValue(dlmsSettingsViewModel);
            CallingAPTitle = new CallingAPTitle(dlmsSettingsViewModel.SystemTitle);
            InitiateRequest = new InitiateRequest(dlmsSettingsViewModel);
        }

        private Logger Logger = LogManager.GetLogger("XML");

        public byte[] ToPduBytes()
        {
            List<byte> appApduAssociationRequest = new List<byte>();
            if (ApplicationContextName != null)
                appApduAssociationRequest.AddRange(ApplicationContextName.ToPduBytes());
            if (CallingAPTitle != null)
            {
                appApduAssociationRequest.AddRange(CallingAPTitle.ToPduBytes());
            }

            if (SenderACSERequirements != null)
            {
                appApduAssociationRequest.AddRange(SenderACSERequirements.ToPduBytes());
            }

            if (MechanismName != null)
            {
                appApduAssociationRequest.AddRange(MechanismName.ToPduBytes());
            }

            if (CallingAuthenticationValue != null)
            {
                appApduAssociationRequest.AddRange(CallingAuthenticationValue.ToPduBytes());
            }

            if (InitiateRequest != null)
            {
                appApduAssociationRequest.AddRange(InitiateRequest.ToPduBytes());
            }

            return appApduAssociationRequest.ToArray();
        }
    }
}