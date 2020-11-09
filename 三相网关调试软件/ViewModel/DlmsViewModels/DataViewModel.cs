using System;
using System.Collections.ObjectModel;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class DataViewModel : ObservableObject
    {
        public Array OctetStringDisplayFormatArray { get; set; } = Enum.GetValues(typeof(OctetStringDisplayFormat));
        public Array UInt32ValueDisplayFormatArray { get; set; } = Enum.GetValues(typeof(UInt32ValueDisplayFormat));


        public ObservableCollection<CustomCosemDataModel> DataCollection
        {
            get => _dataCollection;
            set
            {
                _dataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CustomCosemDataModel> _dataCollection;

        public RelayCommand<CustomCosemDataModel> GetLogicNameCommand
        {
            get => _getLogicNameCommand;
            set
            {
                _getLogicNameCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemDataModel> _getLogicNameCommand;

        public RelayCommand<CustomCosemDataModel> GetValueCommand
        {
            get => _getValueCommand;
            set
            {
                _getValueCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemDataModel> _getValueCommand;

        public RelayCommand<CustomCosemDataModel> SetValueCommand
        {
            get => _setValueCommand;
            set
            {
                _setValueCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemDataModel> _setValueCommand;

        public DLMSClient Client { get; set; }

        public DataViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            ExcelHelper excel = new ExcelHelper(Properties.Settings.Default.ExcelFileName);
            var dataTable = excel.GetExcelDataTable(Properties.Settings.Default.DlmsDataSheetName);
            DataCollection = new ObservableCollection<CustomCosemDataModel>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataCollection.Add(new CustomCosemDataModel(dataTable.Rows[i][0].ToString(),
                        (ObjectType) (int.Parse(dataTable.Rows[i][2].ToString())),
                        new AxdrInteger8(sbyte.Parse(dataTable.Rows[i][3].ToString())))
                    {DataName = dataTable.Rows[i][1].ToString()});
            }

            GetLogicNameCommand = new RelayCommand<CustomCosemDataModel>(async t =>
            {
                t.Value = new DlmsDataItem();
                var getResponse = await Client.GetRequestAndWaitResponse(t.GetLogicNameAttributeDescriptor());
                if (getResponse != null)
                {
                    t.Value.OctetStringDisplayFormat = OctetStringDisplayFormat.Obis;
                    t.Value = getResponse.GetResponseNormal.Result.Data;
                }
            });
            GetValueCommand = new RelayCommand<CustomCosemDataModel>(async t =>
            {
                t.LastResult = new ErrorCode();
                if (t.Value==null)
                {
                    t.Value = new DlmsDataItem();
                }

                t.Value.Value = "";
                
                GetResponse requestAndWaitResponse =
                    await Client.GetRequestAndWaitResponse(t.GetCosemAttributeDescriptor(t.Attr));
                if (requestAndWaitResponse.GetResponseNormal != null)
                {
                    t.LastResult = (ErrorCode) requestAndWaitResponse.GetResponseNormal.Result.DataAccessResult
                        .GetEntityValue();
                   var tt= requestAndWaitResponse.GetResponseNormal.Result.Data.ToPduStringInHex();
                   t.Value.PduStringInHexConstructor(ref tt);
//                    t.Value.Value = requestAndWaitResponse.GetResponseNormal.Result.Data.Value;
                }
            });
            SetValueCommand = new RelayCommand<CustomCosemDataModel>(async (t) =>
            {
                t.Value.UpdateValueBytes();
                t.LastResult = new ErrorCode();
                var setResponse =
                    await Client.SetRequestAndWaitResponse(t.GetCosemAttributeDescriptor(t.Attr), t.Value);
                if (setResponse != null)
                {
                    t.LastResult = (ErrorCode) setResponse.SetResponseNormal.Result;
                }
            });
        }
    }
}