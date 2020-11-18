using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/06/04 16:06:26
        主要用途：
        更改记录：
    */
    public class RegisterViewModel : ObservableObject
    {
        public ObservableCollection<CustomCosemRegisterModel> Registers
        {
            get => _registers;
            set
            {
                _registers = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CustomCosemRegisterModel> _registers;

        public RelayCommand<CustomCosemRegisterModel> GetValueAndScalarUnitCommand { get; set; }

        public RelayCommand<CustomCosemRegisterModel> GetValueCommand { get; set; }
        public RelayCommand<CustomCosemRegisterModel> GetScalarUnitCommand { get; set; }

        public RelayCommand<CustomCosemRegisterModel> SetValueCommand { get; set; }


        public RelayCommand<CosemRegister> GetLogicNameCommand { get; set; }

        public DlmsClient Client { get; set; }


        public RegisterViewModel()
        {
            ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
            var dataTable = excel.GetExcelDataTable("Register$");
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DlmsClient>();
            Registers = new ObservableCollection<CustomCosemRegisterModel>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Registers.Add(new CustomCosemRegisterModel(dataTable.Rows[i][0].ToString())
                    {RegisterName = dataTable.Rows[i][1].ToString()});
            }

            GetValueCommand = new RelayCommand<CustomCosemRegisterModel>(async (t) =>
            {
                t.CompleteData = "";
                t.Value = new DlmsDataItem();
                var getResponse = await Client.GetRequestAndWaitResponse(t.GetValueAttributeDescriptor());
                if (getResponse != null)
                {
                    t.LastResult =
                        (ErrorCode) getResponse.GetResponseNormal.Result.DataAccessResult.GetEntityValue();
                    t.Value = getResponse.GetResponseNormal.Result.Data;
                    t.CompleteData = t.ParserData();
                }
            });
            GetScalarUnitCommand = new RelayCommand<CustomCosemRegisterModel>(async (t) =>
            {
                t.CompleteData = "";
                t.ScalarUnit = new ScalarUnit();
                var scalarUnitResponse =
                    await Client.GetRequestAndWaitResponse(t.GetScalar_UnitAttributeDescriptor());
                if (scalarUnitResponse != null)
                {
                    t.LastResult =
                        (ErrorCode) scalarUnitResponse.GetResponseNormal.Result.DataAccessResult.GetEntityValue();
                    var su = scalarUnitResponse.GetResponseNormal.Result.Data.ToPduStringInHex();
                    if (t.ScalarUnit.PduStringInHexConstructor(ref su))
                    {
                        t.CompleteData = t.ParserData();
                    }
                }
            });
            GetValueAndScalarUnitCommand = new RelayCommand<CustomCosemRegisterModel>(
                async t =>
                {
                    t.CompleteData = "";
                    t.Value = new DlmsDataItem();
                    t.ScalarUnit = new ScalarUnit();
                    var getResponse = await Client.GetRequestAndWaitResponse(t.GetValueAttributeDescriptor());

                    if (getResponse != null)
                    {
                        t.LastResult =
                            (ErrorCode) getResponse.GetResponseNormal.Result.DataAccessResult.GetEntityValue();
                        t.Value = getResponse.GetResponseNormal.Result.Data;
                        if (t.LastResult != ErrorCode.Ok)
                        {
                            return;
                        }

                        var scalarUnitResponse =
                            await Client.GetRequestAndWaitResponse(t.GetScalar_UnitAttributeDescriptor());
                        if (scalarUnitResponse != null)
                        {
                            t.LastResult =
                                (ErrorCode) scalarUnitResponse.GetResponseNormal.Result.DataAccessResult
                                    .GetEntityValue();
                            if (t.LastResult != ErrorCode.Ok)
                            {
                                return;
                            }

                            var su = scalarUnitResponse.GetResponseNormal.Result.Data.ToPduStringInHex();
                            if (t.ScalarUnit.PduStringInHexConstructor(ref su))
                            {
                                t.CompleteData = t.ParserData();
                            }
                        }
                    }
                });
            SetValueCommand = new RelayCommand<CustomCosemRegisterModel>(async (t) =>
            {
                t.Value.UpdateValue();
                var setResponse = await Client.SetRequestAndWaitResponse(t.GetValueAttributeDescriptor(), t.Value);
                if (setResponse != null)
                {
                    t.LastResult =
                        (ErrorCode) setResponse.SetResponseNormal.Result;
                }
            });
        }
    }
}