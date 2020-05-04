using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class EnergyDataViewModel : ViewModelBase
    {
        public EnergyDataViewModel()
        {
            EnergyDataCollection = new ObservableCollection<EnergyDataModel>
            {
                new EnergyDataModel
                {
                    PositiveEnergy = 1000
                }
            };
        }

        private ObservableCollection<EnergyDataModel> _energyDataCollection;

        public ObservableCollection<EnergyDataModel> EnergyDataCollection
        {
            get { return _energyDataCollection; }
            set
            {
                _energyDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private EnergyDataModel _energyData;

        public EnergyDataModel EnergyData
        {
            get { return _energyData; }
            set
            {
                _energyData = value;
                RaisePropertyChanged();
            }
        }
    }
}