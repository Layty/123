using GalaSoft.MvvmLight;

namespace 三相智慧能源网关调试软件.Model
{
    public class InstantDataModel : ObservableObject
    {
        public string DateTime { get; set; }
        public double Ua { get; set; }
        public double Ub { get; set; }
        public double Uc { get; set; }
        public double Ia { get; set; }
        public double Ib { get; set; }
        public double Ic { get; set; }
        public double In { get; set; }

        public double P { get; set; }
        public double Pa { get; set; }
        public double Pb { get; set; }
        public double Pc { get; set; }

        public double Q { get; set; }
        public double Qa { get; set; }
        public double Qb { get; set; }
        public double Qc { get; set; }
        public double S { get; set; }
        public double Sa { get; set; }
        public double Sb { get; set; }
        public double Sc { get; set; }
        public double Pf { get; set; }
        public double Pfa { get; set; }
        public double Pfb { get; set; }
        public double Pfc { get; set; }
        public double angle_Ua { get; set; }
        public double angle_Ub { get; set; }
        public double angle_Uc { get; set; }
        public double angle_Ia { get; set; }
        public double angle_Ib { get; set; }
        public double angle_Ic { get; set; }
        public double RtcBattV { get; set; }
    }
}