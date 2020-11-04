using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.Model;
using Common = 三相智慧能源网关调试软件.Commom.Common;
using ObservableObject = Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class ProfileGenericViewModel : ObservableObject
    {
       
        public ObservableCollection<DlmsSelfDefineCosemProfileGeneric> ProfileGenericCollection
        {
            get => _profileGenericCollection;
            set
            {
                _profileGenericCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DlmsSelfDefineCosemProfileGeneric> _profileGenericCollection;
//        public DlmsSelfDefineCosemProfileGeneric Generic
//        {
//            get => _generic;
//            set
//            {
//                _generic = value;
//                OnPropertyChanged();
//          
//            }
//        }
//
//        private DlmsSelfDefineCosemProfileGeneric _generic;


        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetCaptureObjectsCommand
        {
            get => _GetCaptureObjectsCommand;
            set
            {
                _GetCaptureObjectsCommand = value;
             OnPropertyChanged();
            }
        }

        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _GetCaptureObjectsCommand;

        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetCapturePeriodCommand
        {
            get => _getCapturePeriodCommand;
            set
            {
                _getCapturePeriodCommand = value;
              OnPropertyChanged();
            }
        }

        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _getCapturePeriodCommand;

        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> SetCapturePeriodCommand
        {
            get => _setCapturePeriodCommand;
            set
            {
                _setCapturePeriodCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _setCapturePeriodCommand;

     

        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetEntriesInUseCommand
        {
            get => _GetEntriesInUseCommand;
            set { _GetEntriesInUseCommand = value; OnPropertyChanged(); }
        }
        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _GetEntriesInUseCommand;

        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetProfileEntriesCommand
        {
            get => _GetProfileEntriesCommand;
            set { _GetProfileEntriesCommand = value; OnPropertyChanged(); }
        }
        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _GetProfileEntriesCommand;


      


        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetSortMethodCommand
        {
            get => _GetSortMethodCommand;
            set { _GetSortMethodCommand = value; OnPropertyChanged(); }
        }
        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _GetSortMethodCommand;

    

        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetBufferCommand
        {
            get => _GetBufferCommand;
            set { _GetBufferCommand = value; OnPropertyChanged(); }
        }
        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _GetBufferCommand;


        public DLMSClient Client { get; set; }


        public ObservableCollection<byte[]> ListObservableCollection
        {
            get => _listObservableCollection;
            set
            {
                _listObservableCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<byte[]> _listObservableCollection;

        

        public ProfileGenericViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            ExcelHelper excel = new ExcelHelper(Properties.Settings.Default.ExcelFileName);
            var dataTable = excel.GetExcelDataTable(Properties.Settings.Default.DlmsProfileGenericSheetName);

            ProfileGenericCollection = new ObservableCollection<DlmsSelfDefineCosemProfileGeneric>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                ProfileGenericCollection.Add(new DlmsSelfDefineCosemProfileGeneric(dataTable.Rows[i][0].ToString())
                    { DataName = dataTable.Rows[i][1].ToString() });
            }

            GetCaptureObjectsCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async (t) =>
            {
                var responese = await Client.GetRequestAndWaitResponse(t.GetCaptureObjectsAttributeDescriptor()
                );
                if (responese != null)
                {
                    if (responese.GetResponseNormal.Result.Data.DataType == DataType.Array)
                    {
                        var Array = new DLMSArray();
                        var ar = responese.GetResponseNormal.Result.Data.ToPduStringInHex();
                        ar = ar.Substring(2);
                        if (Array.PduStringInHexConstructor(ref ar))
                        {
                            t.CaptureObjects.Clear();
                            for (int i = 0; i < Array.Items.Length; i++)
                            {
                                var captureObjectDefinition =
                                    CaptureObjectDefinition.CreateFromDlmsData(Array.Items[i]);
                                if (captureObjectDefinition != null)
                                {
                                    t.CaptureObjects.Add(captureObjectDefinition);
                                }
                            }
                        }
                    }
                }
            });

            GetCapturePeriodCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.GetCosemAttributeDescriptor(4));
                if (response != null)
                {
                    response.GetResponseNormal.Result.Data.ValueDisplay.UInt32ValueDisplayFormat =
                        UInt32ValueDisplayFormat.IntValue;
                    t.CapturePeriod1.Value = response.GetResponseNormal.Result.Data.ValueDisplay.ValueString;
                }
            });
            SetCapturePeriodCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async (t) =>
            {
                await Client.SetRequestAndWaitResponse(t.GetCapturePeriodAttributeDescriptor(),
                    new DLMSDataItem(DataType.UInt32, t.CapturePeriod1.Value));
            });
            GetEntriesInUseCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.GetEntriesInUseAttributeDescriptor());
                if (response!=null)
                {
                    response.GetResponseNormal.Result.Data.ValueDisplay.UInt32ValueDisplayFormat =
                        UInt32ValueDisplayFormat.IntValue;
                    t.EntriesInUse.Value = response.GetResponseNormal.Result.Data.ValueDisplay.ValueString;
                }
             
            });
            GetProfileEntriesCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async
                (t) =>
            {
                var resultBytes = await Client.GetRequest(t.GetProfileEntriesAttributeDescriptor());
                t.ProfileEntries = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetSortMethodCommand =  new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async
                (t) =>
            {
                var resultBytes = await Client.GetRequest(t.GetSortMethodAttributeDescriptor());
                t.SortMethod = (SortMethod) ushort.Parse(NormalDataParse.ParsePduData(resultBytes));
            });

            GetBufferCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async
                (t) =>
            {
                var resultBytes = await Client.GetRequest(t.GetBufferAttributeDescriptor());
                var stringTo = NormalDataParse.ParsePduData(resultBytes).StringToByte();
                var splitCountLength = (stringTo.Length - 1) / stringTo[0];
                var Index = 1;
                ListObservableCollection = new ObservableCollection<byte[]>();
                for (int i = 0; i < stringTo[0]; i++)
                {
                    Index = 1 + (i * splitCountLength);
                    ListObservableCollection.Add(stringTo.Skip(Index).Take(splitCountLength).ToArray());
                }
            });
        }
    }
}