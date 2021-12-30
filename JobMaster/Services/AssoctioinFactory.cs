using JobMaster.ViewModels;
using MyDlmsStandard.ApplicationLay.Association;

namespace JobMaster.Services
{
    public static class AssoctioinFactory
    {
        public static AssociationRequest CreateAssociationRequest(DlmsSettingsViewModel DlmsSettingsViewModel)
        {
            var AssociationRequest = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
            return AssociationRequest;
        }
        public static AssociationResponse CreateAssociationResponse(DlmsSettingsViewModel DlmsSettingsViewModel)
        {
            var AssociationResponse = new AssociationResponse();
            return AssociationResponse;
        }
    }
}