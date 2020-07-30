using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class DataViewModel : ViewModelBase
    {
        public DisplayFormatToShow DisplayFormat
        {
            get => _displayFormat;
            set
            {
                _displayFormat = value;


                RaisePropertyChanged();
            }
        }

        private DisplayFormatToShow _displayFormat = DisplayFormatToShow.Original;
        public Array DisplayArray { get; set; }

        public ObservableCollection<DLMSSelfDefineData> DataCollection
        {
            get => _dataCollection;
            set
            {
                _dataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<DLMSSelfDefineData> _dataCollection;

        public RelayCommand<DLMSSelfDefineData> GetLogicNameCommand
        {
            get => _getLogicNameCommand;
            set
            {
                _getLogicNameCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineData> _getLogicNameCommand;

        public RelayCommand<DLMSSelfDefineData> GetValueCommand
        {
            get => _getValueCommand;
            set
            {
                _getValueCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineData> _getValueCommand;

        public RelayCommand<DLMSSelfDefineData> SetValueCommand
        {
            get => _setValueCommand;
            set
            {
                _setValueCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineData> _setValueCommand;

        public DLMSClient Client { get; set; }

      

        public DataViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
            var dataTable = excel.GetExcelDataTable("Data$");
            DataCollection = new ObservableCollection<DLMSSelfDefineData>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataCollection.Add(new DLMSSelfDefineData(dataTable.Rows[i][0].ToString())
                    {DataName = dataTable.Rows[i][1].ToString()});
            }

            GetLogicNameCommand = new RelayCommand<DLMSSelfDefineData>(async t =>
            {
                t.Value = new DLMSDataItem();
                var dataResult = await Client.GetRequest(t.GetLogicName());
                GetResponse getResponse = new GetResponse();
                getResponse.GetResponseNormal.PduBytesToConstructor(dataResult);
                t.Value.ValueBytes = getResponse.GetResponseNormal.GetDataResult.Data.ValueBytes;
                DisplayFormat = DisplayFormatToShow.OBIS;
            });
            GetValueCommand = new RelayCommand<DLMSSelfDefineData>(async t =>
            {
                t.LastResult = new ErrorCode();
                t.Value = new DLMSDataItem();
                var dataResult = await Client.GetRequest(t.GetValue());

                GetResponse getResponse = new GetResponse();

                if (getResponse.PduBytesToConstructor(dataResult))
                {
                    t.Value.DataType = getResponse.GetResponseNormal.GetDataResult.Data.DataType;
                    t.LastResult = getResponse.GetResponseNormal.GetDataResult.DataAccessResult;
                    t.Value.ValueString = getResponse.GetResponseNormal.GetDataResult.Data.ValueString;
                    t.Value.ValueBytes = getResponse.GetResponseNormal.GetDataResult.Data.ValueBytes;
                    if (t.Value.DataType == DataType.OctetString)
                    {
                        t.Value.ValueString =
                            NormalDataParse.HowToDisplayOctetString(t.Value.ValueBytes, DisplayFormat);
                    }
                }
            });
            SetValueCommand = new RelayCommand<DLMSSelfDefineData>(async (t) =>
            {
                t.LastResult = new ErrorCode();
                var d = new DLMSDataItem(t.Value.DataType, t.Value.ValueString);
                await Client.SetRequest(t.SetValue(d));
            });

            DisplayArray = Enum.GetValues(typeof(DisplayFormatToShow));
        }
    }
}