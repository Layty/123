using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class DataViewModel : ViewModelBase
    {
        public Array OctetStringDisplayFormatArray { get; set; } = Enum.GetValues(typeof(OctetStringDisplayFormat));
        public Array UInt32ValueDisplayFormatArray { get; set; } = Enum.GetValues(typeof(UInt32ValueDisplayFormat));

        public DLMSSelfDefineData DlmsSelfDefineData
        {
            get => _dlmsSelfDefineData;
            set
            {
                _dlmsSelfDefineData = value;
                RaisePropertyChanged();
            }
        }

        private DLMSSelfDefineData _dlmsSelfDefineData;

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
            ExcelHelper excel = new ExcelHelper(Properties.Settings.Default.ExcelFileName);
            var dataTable = excel.GetExcelDataTable(Properties.Settings.Default.DlmsDataSheetName);
            DataCollection = new ObservableCollection<DLMSSelfDefineData>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataCollection.Add(new DLMSSelfDefineData(dataTable.Rows[i][0].ToString(),
                        (ObjectType) int.Parse(dataTable.Rows[i][2].ToString()),
                        byte.Parse(dataTable.Rows[i][3].ToString()))
                    {DataName = dataTable.Rows[i][1].ToString()});
            }

            GetLogicNameCommand = new RelayCommand<DLMSSelfDefineData>(async t =>
            {
                t.Value = new DLMSDataItem();
                GetRequest getRequest = new GetRequest()
                    {GetRequestNormal = new GetRequestNormal(t.GetLogicNameAttributeDescriptor())};

                var dataResult = await Client.GetRequest(getRequest);

                GetResponse getResponse = new GetResponse();
                var data = dataResult.ByteToString("");
                if (getResponse.PduStringInHexConstructor(ref data))
                {
                    t.Value.ValueDisplay.OctetStringDisplayFormat = OctetStringDisplayFormat.Obis;
                    t.Value = getResponse.GetResponseNormal.Result.Data;
                }
            });
            GetValueCommand = new RelayCommand<DLMSSelfDefineData>(async (t) =>
            {
                DlmsSelfDefineData = t;
                t.LastResult = new ErrorCode();
                t.Value = new DLMSDataItem();
                GetRequest getRequest = new GetRequest
                {
                    GetRequestNormal = new GetRequestNormal(t.GetCosemAttributeDescriptor(t.Attr))
                };
                GetResponse requestAndWaitResponse = await Client.GetRequestAndWaitResponse(getRequest);
                if (requestAndWaitResponse != null)
                {
                    t.LastResult = (ErrorCode) requestAndWaitResponse.GetResponseNormal.Result.DataAccessResult
                        .GetEntityValue();
                    t.Value = requestAndWaitResponse.GetResponseNormal.Result.Data;
                }
            }, true);
            SetValueCommand = new RelayCommand<DLMSSelfDefineData>(async (t) =>
            {
                t.Value.UpdateValueBytes();
                t.LastResult = new ErrorCode();
                SetRequest setRequest = new SetRequest();
                setRequest.SetRequestNormal = new SetRequestNormal(t.GetCosemAttributeDescriptor(t.Attr), t.Value);
              var  dataResult =await Client.SetRequest(setRequest);
                var data = dataResult.ByteToString("");
                SetResponse setResponse=new SetResponse();
                if (setResponse.PduStringInHexConstructor(ref data))
                {
                    t.LastResult = (ErrorCode)setResponse.SetResponseNormal.Result
                        .GetEntityValue();
                }
            });
        }
    }
}