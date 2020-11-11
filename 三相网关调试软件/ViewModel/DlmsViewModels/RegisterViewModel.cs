using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage;
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
                t.Value = new DlmsDataItem();
                var getResponse = await Client.GetRequestAndWaitResponse(t.GetValueAttributeDescriptor());
                if (getResponse != null)
                {
                    t.LastResult =
                        (ErrorCode) getResponse.GetResponseNormal.Result.DataAccessResult.GetEntityValue();
                    t.Value = getResponse.GetResponseNormal.Result.Data;
                }
            });
            GetScalarUnitCommand = new RelayCommand<CustomCosemRegisterModel>(async (t) =>
            {
                t.ScalarUnit = new ScalarUnit();
                var scalarUnitResponse =
                    await Client.GetRequestAndWaitResponse(t.GetScalar_UnitAttributeDescriptor());
                if (scalarUnitResponse != null)
                {
                    var structure = (DlmsStructure) scalarUnitResponse.GetResponseNormal.Result.Data.Value;
                    t.ScalarUnit.Scalar = (sbyte) Convert.ToSByte(structure.Items[0].Value.ToString(), 16);
                    t.ScalarUnit.Unit = (Unit) byte.Parse(structure.Items[1].ValueString);
                }
            });
            GetValueAndScalarUnitCommand = new RelayCommand<CustomCosemRegisterModel>(
                async t =>
                {
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
                            var strun = (DlmsStructure) scalarUnitResponse.GetResponseNormal.Result.Data.Value;
                            t.ScalarUnit.Scalar = (sbyte) Convert.ToSByte(strun.Items[0].Value.ToString(), 16);
                            t.ScalarUnit.Unit = (Unit) byte.Parse(strun.Items[1].ValueString);
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