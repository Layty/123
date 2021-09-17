using System;
using System.Collections.ObjectModel;
using 三相智慧能源网关调试软件.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.Axdr;


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

        public RelayCommand<CustomCosemDataModel> GetLogicNameCommand { get; set; }


        public RelayCommand<CustomCosemDataModel> GetValueCommand { get; set; }


        public RelayCommand<CustomCosemDataModel> SetValueCommand { get; set; }


        public DlmsClient Client { get; set; }

        public DataViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DlmsClient>();
            ExcelHelper excel = new ExcelHelper(Properties.Settings.Default.ExcelFileName);
            var dataTable = excel.GetExcelDataTable(Properties.Settings.Default.DlmsDataSheetName);
            DataCollection = new ObservableCollection<CustomCosemDataModel>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataCollection.Add(new CustomCosemDataModel(dataTable.Rows[i][0].ToString(),
                        (ObjectType) int.Parse(dataTable.Rows[i][2].ToString()),
                        new AxdrInteger8(sbyte.Parse(dataTable.Rows[i][3].ToString())))
                    {DataName = dataTable.Rows[i][1].ToString()});
            }

            GetLogicNameCommand = new RelayCommand<CustomCosemDataModel>(async t =>
            {
                t.Value = new DlmsDataItem();
                var getResponse = await Client.GetRequestAndWaitResponse(t.LogicNameAttributeDescriptor);
                if (getResponse != null)
                {
                    t.Value.OctetStringDisplayFormat = OctetStringDisplayFormat.Obis;
                    t.Value = getResponse.GetResponseNormal.Result.Data;
                }
            });
            GetValueCommand = new RelayCommand<CustomCosemDataModel>(async t =>
            {
                t.LastResult = new ErrorCode();
                if (t.Value == null)
                {
                    t.Value = new DlmsDataItem();
                }

                t.Value.Value = "";

                GetResponse requestAndWaitResponse =
                    await Client.GetRequestAndWaitResponse(t.GetCosemAttributeDescriptor(t.Attr));
                if (requestAndWaitResponse?.GetResponseNormal != null)
                {
                    t.LastResult = (ErrorCode) requestAndWaitResponse.GetResponseNormal.Result.DataAccessResult
                        .GetEntityValue();
                    if (t.LastResult != ErrorCode.Ok)
                    {
                        return;
                    }

                    var tt = requestAndWaitResponse.GetResponseNormal.Result.Data.ToPduStringInHex();
                    t.Value.PduStringInHexConstructor(ref tt);
                }
            });
            SetValueCommand = new RelayCommand<CustomCosemDataModel>(async t =>
            {
                t.Value.UpdateValue();
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