using System;
using System.Collections.ObjectModel;

using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.Model;
using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class DataViewModel :ObservableObject
    {
        public Array OctetStringDisplayFormatArray { get; set; } = Enum.GetValues(typeof(OctetStringDisplayFormat));
        public Array UInt32ValueDisplayFormatArray { get; set; } = Enum.GetValues(typeof(UInt32ValueDisplayFormat));


        public ObservableCollection<CosemSelfDefineData> DataCollection
        {
            get => _dataCollection;
            set
            {
                _dataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CosemSelfDefineData> _dataCollection;

        public RelayCommand<CosemSelfDefineData> GetLogicNameCommand
        {
            get => _getLogicNameCommand;
            set
            {
                _getLogicNameCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CosemSelfDefineData> _getLogicNameCommand;

        public RelayCommand<CosemSelfDefineData> GetValueCommand
        {
            get => _getValueCommand;
            set
            {
                _getValueCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CosemSelfDefineData> _getValueCommand;

        public RelayCommand<CosemSelfDefineData> SetValueCommand
        {
            get => _setValueCommand;
            set
            {
                _setValueCommand = value;
             OnPropertyChanged();
            }
        }

        private RelayCommand<CosemSelfDefineData> _setValueCommand;

        public DLMSClient Client { get; set; }

        public DataViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            ExcelHelper excel = new ExcelHelper(Properties.Settings.Default.ExcelFileName);
            var dataTable = excel.GetExcelDataTable(Properties.Settings.Default.DlmsDataSheetName);
            DataCollection = new ObservableCollection<CosemSelfDefineData>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataCollection.Add(new CosemSelfDefineData(dataTable.Rows[i][0].ToString(),
                        (ObjectType) (int.Parse(dataTable.Rows[i][2].ToString())),
                        new AxdrInteger8(sbyte.Parse(dataTable.Rows[i][3].ToString())))
                    {DataName = dataTable.Rows[i][1].ToString()});
            }

            GetLogicNameCommand = new RelayCommand<CosemSelfDefineData>(async t =>
            {
                t.Value = new DLMSDataItem();
                var getResponse = await Client.GetRequestAndWaitResponse(t.GetLogicNameAttributeDescriptor());
                if (getResponse != null)
                {
                    t.Value.ValueDisplay.OctetStringDisplayFormat = OctetStringDisplayFormat.Obis;
                    t.Value = getResponse.GetResponseNormal.Result.Data;
                }
            });
            GetValueCommand = new RelayCommand<CosemSelfDefineData>(async t =>
            {
                t.LastResult = new ErrorCode();
                t.Value = new DLMSDataItem();
                GetResponse requestAndWaitResponse =
                    await Client.GetRequestAndWaitResponse(t.GetCosemAttributeDescriptor(t.Attr));
                if (requestAndWaitResponse != null)
                {
                    t.LastResult = (ErrorCode) requestAndWaitResponse.GetResponseNormal.Result.DataAccessResult
                        .GetEntityValue();
                    t.Value = requestAndWaitResponse.GetResponseNormal.Result.Data;
                }
            });
            SetValueCommand = new RelayCommand<CosemSelfDefineData>(async (t) =>
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