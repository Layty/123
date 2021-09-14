using System.Text;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.ApplicationLay.Set;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class Request : IToPduStringInHex
    {
        public AssociationRequest AssociationRequest { get; set; }
        public GetRequest GetRequest { get; set; }
        public SetRequest SetRequest { get; set; }
        public ActionRequest ActionRequest { get; set; }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (AssociationRequest!=null)
            {
                stringBuilder.Append(AssociationRequest.ToPduStringInHex());
            }
            if (GetRequest != null)
            {
                stringBuilder.Append(GetRequest.ToPduStringInHex());
            }

            if (SetRequest != null)
            {
                stringBuilder.Append(SetRequest.ToPduStringInHex());
            }

            if (ActionRequest != null)
            {
                stringBuilder.Append(ActionRequest.ToPduStringInHex());
            }

            return stringBuilder.ToString();
        }
    }
}