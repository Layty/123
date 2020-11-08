using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MySerialPortMaster;
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

        public RelayCommand<CustomCosemRegisterModel> GetValueCommand
        {
            get => _getValueCommand;
            set
            {
                _getValueCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemRegisterModel> _getValueCommand;

        public RelayCommand<CustomCosemRegisterModel> SetValueCommand
        {
            get => _setValueCommand;
            set
            {
                _setValueCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemRegisterModel> _setValueCommand;

        public RelayCommand<CosemRegister> GetLogicNameCommand
        {
            get => _getLogicNameCommand;
            set
            {
                _getLogicNameCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CosemRegister> _getLogicNameCommand;

        public DLMSClient Client { get; set; }


        public RegisterViewModel()
        {
            ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
            var dataTable = excel.GetExcelDataTable("Register$");
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            Registers = new ObservableCollection<CustomCosemRegisterModel>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Registers.Add(new CustomCosemRegisterModel(dataTable.Rows[i][0].ToString())
                    {RegisterName = dataTable.Rows[i][1].ToString()});
            }

            GetValueCommand = new RelayCommand<CustomCosemRegisterModel>(
                async t =>
                {
                    t.Value = new DlmsDataItem();
                    t.ScalarUnit = new ScalarUnit() ;
                    var getResponse = await Client.GetRequestAndWaitResponse(t.GetValueAttributeDescriptor());
                 
                    if (getResponse!=null)
                    {
                        t.LastResult =
                            (ErrorCode) getResponse.GetResponseNormal.Result.DataAccessResult.GetEntityValue();
                        t.Value = getResponse.GetResponseNormal.Result.Data;
                        if (t.LastResult != ErrorCode.Ok)
                        {
                            return;
                        }

                        var scalarUnit = await Client.GetRequest(t.GetScalar_UnitAttributeDescriptor());
                        var structData = NormalDataParse.ParsePduData(scalarUnit);
                        var unitByte = structData.StringToByte();
                        switch (unitByte.Take(1).ToArray()[0])
                        {
                            case (byte) DataType.Int8:
                                t.ScalarUnit.Scalar = (sbyte) unitByte.Skip(1).Take(1).ToArray()[0];
                                break;
                        }

                        switch (unitByte.Skip(2).Take(1).ToArray()[0])
                        {
                            case (byte) DataType.Enum:
                                t.ScalarUnit.Unit = (Unit) unitByte.Skip(3).Take(1).ToArray()[0];
                                break;
                        }
                    }
                });
            SetValueCommand = new RelayCommand<CustomCosemRegisterModel>(async (t) =>
            {
                t.Value.UpdateValueBytes();
                var dataResult = await Client.SetRequestAndWaitResponse(t.GetValueAttributeDescriptor(), t.Value);
            });
        }
    }
}