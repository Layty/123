using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Documents;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.MyControl
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/06/04 16:06:26
        主要用途：
        更改记录：
    */
    public class RegisterViewModel : ViewModelBase
    {
        public ObservableCollection<DLMSSelfDefineRegisterModel> Registers
        {
            get => _registers;
            set
            {
                _registers = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<DLMSSelfDefineRegisterModel> _registers;

        public RelayCommand<DLMSRegister> GetValueCommand
        {
            get => _getValueCommand;
            set
            {
                _getValueCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSRegister> _getValueCommand;

        public RelayCommand<DLMSRegister> GetLogicNameCommand
        {
            get => _GetLogicNameCommand;
            set
            {
                _GetLogicNameCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSRegister> _GetLogicNameCommand;

        public DLMSClient Client { get; set; }


        public RegisterViewModel()
        {
            
            ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
            //  var sheetName = excel.GetDataFromExcelWithAppointSheetNames();
            //  var table=  ExcelHelper.ExcelToDataTable("DLMS设备信息.xls", true);
            var DataTable = excel.GetExcelDataTable("Register$");
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();

            Registers = new ObservableCollection<DLMSSelfDefineRegisterModel>();
            for (int i = 0; i < DataTable.Rows.Count; i++)
            {
                Registers.Add(new DLMSSelfDefineRegisterModel(DataTable.Rows[i][0].ToString())
                    {RegisterName = DataTable.Rows[i][1].ToString()});
            }

            GetValueCommand = new RelayCommand<DLMSRegister>(
                async t =>
                {
                    t.Value = "";
                    t.Scalar = 1;
                    t.Unit = Unit.None;
                    var dataResult = await Client.GetRequest(t.GetAttributeData(2));
                    t.Value = NormalDataParse.ParsePduData(dataResult);
                    var scalarUnit = await Client.GetRequest(t.GetAttributeData(3));
                    var structData = NormalDataParse.ParsePduData(scalarUnit);
                    var unitbyte = structData.StringToByte();
                    switch (unitbyte.Take(1).ToArray()[0])
                    {
                        case (byte) DataType.Int8:
                            t.Scalar = (sbyte) unitbyte.Skip(1).Take(1).ToArray()[0];
                            break;
                    }

                    switch (unitbyte.Skip(2).Take(1).ToArray()[0])
                    {
                        case (byte) DataType.Enum:
                            t.Unit = (Unit) unitbyte.Skip(3).Take(1).ToArray()[0];
                            break;
                    }
                });
        }
    }
}