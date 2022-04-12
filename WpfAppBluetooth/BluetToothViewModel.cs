
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace WpfAppBluetooth
{
    public class BluetToothViewModel : ViewModelBase
    {
        #region Error Codes

        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);

        readonly int
            E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df); // HRESULT_FROM_WIN32(ERROR_DEVICE_NOT_AVAILABLE)

        #endregion

        public BleCore BleCore { get; set; }
        private GattCharacteristic selectedCharacteristic;

        // Only one registered characteristic at a time.
        private GattCharacteristic registeredCharacteristic;
        private GattPresentationFormat presentationFormat;

        public void InitBleCore()
        {
            BleCore = new BleCore();
            GattCharacteristics = new ObservableCollection<GattCharacteristic>();
            GattDeviceServices = new ObservableCollection<GattDeviceService>();
            BleCore.MessAgeChanged += BleCore_MessAgeChanged;
            BleCore.DeviceWatcherChanged += BleCore_DeviceWatcherChanged;
            BleCore.GattDeviceServiceAdded += BleCore_GattDeviceServiceAdded;
            BleCore.CharacteristicAdded += BleCore_CharacteristicAdded;
        }

        public ObservableCollection<BluetoothLEDevice> BluetoothLeDevices
        {
            get => _bluetoothLeDevices;
            set
            {
                _bluetoothLeDevices = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BluetoothLEDevice> _bluetoothLeDevices;

        public ObservableCollection<GattCharacteristic> GattCharacteristics
        {
            get => _gattCharacteristics;
            set
            {
                _gattCharacteristics = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<GattCharacteristic> _gattCharacteristics;

        public ObservableCollection<GattDeviceService> GattDeviceServices
        {
            get => _gattDGattDeviceServices;
            set
            {
                _gattDGattDeviceServices = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<GattDeviceService> _gattDGattDeviceServices;
        private BluetoothLEDevice bluetoothLeDevice;

        public BluetToothViewModel()
        {
            //            Process.Start("ms-settings:bluetooth");
            BluetoothLeDevices = new ObservableCollection<BluetoothLEDevice>();

            ConnectCommand = new RelayCommand<BluetoothLEDevice>(async (t) =>
            {
                if (t.ConnectionStatus == BluetoothConnectionStatus.Connected)
                {
                    return;
                }

                BleCore.CurrentDevice = t;
                BleCore.StartMatching(t);
                try
                {
                    bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(t.DeviceId);

                    if (bluetoothLeDevice == null)
                    {
                        Console.WriteLine("error Connect");
                    }
                }
                catch (Exception ex) when (ex.HResult == E_DEVICE_NOT_AVAILABLE)
                {
                    //  rootPage.NotifyUser("Bluetooth radio is not on.", NotifyType.ErrorMessage);
                }


                if (bluetoothLeDevice != null)
                {
                    DevicePairingResult e = await bluetoothLeDevice.DeviceInformation.Pairing.PairAsync();
                    var ttt = bluetoothLeDevice.DeviceInformation.Pairing.IsPaired;
                    var st = e.Status;
                    // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
                    // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
                    // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
                    GattDeviceServicesResult result =
                        await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);


                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        var services = result.Services;
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            foreach (var service in services)
                            {
                                GattDeviceServices.Add(service);
                            }
                        });


                        // ConnectButton.Visibility = Visibility.Collapsed;
                        //ServiceList.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        //rootPage.NotifyUser("Device unreachable", NotifyType.ErrorMessage);
                    }
                }
            });
            FindGattCharacteristicsCommand = new RelayCommand<GattDeviceService>(async (service) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    GattCharacteristics = new ObservableCollection<GattCharacteristic>();
                });
                RemoveValueChangedHandler();
                IReadOnlyList<GattCharacteristic> characteristics = null;
                try
                {
                    // Ensure we have access to the device.
                    var accessStatus = await service.RequestAccessAsync();
                    if (accessStatus == DeviceAccessStatus.Allowed)
                    {
                        // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                        // and the new Async functions to get the characteristics of unpaired devices as well. 
                        var result = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

                        if (result.Status == GattCommunicationStatus.Success)
                        {
                            characteristics = result.Characteristics;
                        }
                        else
                        {
                            //   rootPage.NotifyUser("Error accessing service.", NotifyType.ErrorMessage);

                            // On error, act as if there are no characteristics.
                            characteristics = new List<GattCharacteristic>();
                        }
                    }
                    else
                    {
                        // Not granted access
                        // rootPage.NotifyUser("Error accessing service.", NotifyType.ErrorMessage);

                        // On error, act as if there are no characteristics.
                        characteristics = new List<GattCharacteristic>();
                    }
                }
                catch (Exception ex)
                {
                    //rootPage.NotifyUser("Restricted service. Can't read characteristics: " + ex.Message,
                    //  NotifyType.ErrorMessage);
                    // On error, act as if there are no characteristics.
                    characteristics = new List<GattCharacteristic>();
                }


                foreach (GattCharacteristic c in characteristics)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { GattCharacteristics.Add(c); });
                }
            });

            DisConnectCommand = new RelayCommand<BluetoothLEDevice>((t) =>
            {
                BleCore.CurrentDevice = t;
                GattCharacteristics = new ObservableCollection<GattCharacteristic>();
                GattDeviceServices = new ObservableCollection<GattDeviceService>();
                BleCore.Dispose();
                foreach (GattCharacteristic gattCharacteristic in GattCharacteristics)
                {
                    gattCharacteristic.Service.Dispose();
                }

                foreach (var gattDeviceService in GattDeviceServices)
                {
                    gattDeviceService.Dispose();
                }

                bluetoothLeDevice.Dispose();
                bluetoothLeDevice = null;


                GC.Collect();
            });
        }

        private bool subscribedForNotifications = false;

        private void RemoveValueChangedHandler()
        {
            //ValueChangedSubscribeToggle.Content = "Subscribe to value changes";
            //            if (subscribedForNotifications)
            //            {
            //                registeredCharacteristic.ValueChanged -= Characteristic_ValueChanged;
            //                registeredCharacteristic = null;
            //                subscribedForNotifications = false;
            //            }
        }

        private async void CharacteristicWriteButtonInt_Click()
        {
            if (!String.IsNullOrEmpty("1"))
            {
                var isValidValue = Int32.TryParse("1", out int readValue);
                if (isValidValue)
                {
                    var writer = new DataWriter();
                    writer.ByteOrder = ByteOrder.LittleEndian;
                    writer.WriteInt32(readValue);

                    var writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writer.DetachBuffer());
                }
                else
                {
                    //    rootPage.NotifyUser("Data to write has to be an int32", NotifyType.ErrorMessage);
                }
            }
            else
            {
                //  rootPage.NotifyUser("No data to write to device", NotifyType.ErrorMessage);
            }
        }

        private async Task<bool> WriteBufferToSelectedCharacteristicAsync(IBuffer buffer)
        {
            try
            {
                // BT_Code: Writes the value from the buffer to the characteristic.
                var result = await selectedCharacteristic.WriteValueWithResultAsync(buffer);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    //      rootPage.NotifyUser("Successfully wrote value to device", NotifyType.StatusMessage);
                    return true;
                }
                else
                {
                    //    rootPage.NotifyUser($"Write failed: {result.Status}", NotifyType.ErrorMessage);
                    return false;
                }
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_INVALID_PDU)
            {
                //rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                return false;
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED ||
                                       ex.HResult == E_ACCESSDENIED)
            {
                // This usually happens when a device reports that it support writing, but it actually doesn't.
                //  rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                return false;
            }
        }

        private async void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // BT_Code: An Indicate or Notify reported that the value has changed.
            // Display the new value with a timestamp.
            //            var newValue = FormatValueByPresentation(args.CharacteristicValue, presentationFormat);
            //            var message = $"Value at {DateTime.Now:hh:mm:ss.FFF}: {newValue}";
            //            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            //                () => CharacteristicLatestValue.Text = message);
        }

        private void BleCore_CharacteristicAdded(
            GattCharacteristic gattCharacteristic)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { GattCharacteristics.Add(gattCharacteristic); });
        }

        private void BleCore_GattDeviceServiceAdded(
            GattDeviceService gattDeviceService)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { GattDeviceServices.Add(gattDeviceService); });
        }


        public string WatcherStatus
        {
            get => _watcherStatus;
            set
            {
                _watcherStatus = value;
                RaisePropertyChanged();
            }
        }

        private string _watcherStatus;

        private void BleCore_DeviceWatcherChanged(MsgType type, BluetoothLEDevice bluetoothLEDevice)
        {
            switch (type)
            {
                case MsgType.NotifyTxt: break;
                case MsgType.BleData: break;
                case MsgType.BleDevice:
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(new Action(() =>
                        {
                            BluetoothLeDevices.Add(bluetoothLEDevice);
                        }));
                    }
                    break;
            }
        }


        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }

        private string _message;

        private void BleCore_MessAgeChanged(MsgType type, string message, byte[] data = null)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(new Action(() => { Message += message + "\r\n"; }));
        }

        private RelayCommand _startCommand;

        public RelayCommand StartCommand =>
            _startCommand ?? (_startCommand = new RelayCommand(Start));

        private void Start()
        {
            BluetoothLeDevices.Clear();
            InitBleCore();
            BleCore.StartBleDeviceWatcher();
        }

        private RelayCommand _stopCommand;

        public RelayCommand StopCommand =>
            _stopCommand ?? (_stopCommand = new RelayCommand(Stop));

        private void Stop()
        {
            BleCore.StopBleDeviceWatcher();
        }

        public RelayCommand<BluetoothLEDevice> ConnectCommand { get; set; }
        public RelayCommand<BluetoothLEDevice> DisConnectCommand { get; set; }
        public RelayCommand<GattDeviceService> FindGattCharacteristicsCommand { get; }

        public BluetoothClient BluetoothClient = new BluetoothClient();

        public RelayCommand FindCommand => new RelayCommand(async () =>
        {
            await Task.Run(() =>
            {
                BluetoothDeviceInfos = new ObservableCollection<BluetoothDeviceInfo>();
                BluetoothClient = new BluetoothClient();
                BluetoothRadio bluetooth = BluetoothRadio.Default;
                bluetooth.Mode = RadioMode.Connectable;
                var countsCollection = BluetoothClient.DiscoverDevices();
                foreach (var bluetoothDeviceInfo in countsCollection)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { BluetoothDeviceInfos.Add(bluetoothDeviceInfo); });
                }
            });
        });

        public ObservableCollection<BluetoothDeviceInfo> BluetoothDeviceInfos
        {
            get => _BluetoothDeviceInfos;
            set
            {
                _BluetoothDeviceInfos = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BluetoothDeviceInfo> _BluetoothDeviceInfos;

        public RelayCommand<BluetoothDeviceInfo> connect => new RelayCommand<BluetoothDeviceInfo>((t) =>
        {
            BluetoothClient.Connect(t.DeviceAddress, BluetoothService.Handsfree);
        });
    }
}