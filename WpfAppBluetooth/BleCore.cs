using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Security.Cryptography;

namespace WpfAppBluetooth
{
    public class BleCore
    {
        public delegate void DeviceWatcherChangedEvent(MsgType type, BluetoothLEDevice bluetoothLEDevice);

        public delegate void GattDeviceServiceAddedEvent(GattDeviceService gattDeviceService);

        public delegate void CharacteristicAddedEvent(GattCharacteristic gattCharacteristic);

        public delegate void MessAgeChangedEvent(MsgType type, string message, byte[] data = null);

        private bool _asyncLock = false;

        public string MSState = string.Empty;

        private const GattClientCharacteristicConfigurationDescriptorValue CHARACTERISTIC_NOTIFICATION_TYPE =
            GattClientCharacteristicConfigurationDescriptorValue.Notify;

        private List<BluetoothLEDevice> _deviceList = new List<BluetoothLEDevice>();

        private DeviceWatcher deviceWatcher;

        public GattDeviceService CurrentService { get; set; }

        public BluetoothLEDevice CurrentDevice { get; set; }

        public GattCharacteristic CurrentWriteCharacteristic { get; set; }

        public GattCharacteristic CurrentWriteWithoutResponseCharacteristic { get; set; }

        public GattCharacteristic CurrentNotifyCharacteristic { get; set; }

        public string CurrentDeviceMAC { get; set; }

        public event DeviceWatcherChangedEvent DeviceWatcherChanged;

        public event GattDeviceServiceAddedEvent GattDeviceServiceAdded;

        public event CharacteristicAddedEvent CharacteristicAdded;

        public event MessAgeChangedEvent MessAgeChanged;

        public void StartBleDeviceWatcher()
        {
            string[] requestedProperties = {
                "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected",
                "System.Devices.Aep.Bluetooth.Le.IsConnectable"
            };
            string aqsAllBluetoothLEDevices =
                "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

            deviceWatcher =
                DeviceInformation.CreateWatcher(
                    aqsAllBluetoothLEDevices,
                    requestedProperties,
                    DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            deviceWatcher.Start();


//            _deviceWatcher.Start();
            string message = "自动发现设备中..";
            MessAgeChanged?.Invoke(MsgType.NotifyTxt, message);
        }


        private void DeviceWatcher_Received(BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementReceivedEventArgs args)
        {
            BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress).Completed = (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed && asyncInfo.GetResults() != null)
                {
                    BluetoothLEDevice currentDevice = asyncInfo.GetResults();
                    bool contain = false;
                    foreach (BluetoothLEDevice device in _deviceList)
                    {
                        if (device.DeviceId == currentDevice.DeviceId)
                        {
                            contain = true;
                        }
                    }

                    if (!contain && currentDevice.Name.Contains("SH"))
                    {
                        _deviceList.Add(currentDevice);

                        DeviceWatcherChanged?.Invoke(MsgType.BleDevice, currentDevice);
                    }
                }
            };
        }

        public void StopBleDeviceWatcher()
        {
            if (deviceWatcher != null)
            {
                // Unregister the event handlers.
                deviceWatcher.Added -= DeviceWatcher_Added;
//                deviceWatcher.Updated -= DeviceWatcher_Updated;
//                deviceWatcher.Removed -= DeviceWatcher_Removed;
//                deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                deviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                deviceWatcher.Stop();
                deviceWatcher = null;
            }
        }


        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            lock (this)
            {
                //Debug.WriteLine(String.Format("Added {0}{1}", deviceInfo.Id, deviceInfo.Name));

                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    if (args.Name.StartsWith("SH"))
                    {
                        this.MessAgeChanged(MsgType.NotifyTxt, "发现设备:" + args.Name);
                        Matching(args.Id);
                    }

                    // Make sure device isn't already present in the list.
                    //                        if (FindBluetoothLEDeviceDisplay(deviceInfo.Id) == null)
                    //                        {
                    //                            if (deviceInfo.Name != string.Empty)
                    //                            {
                    //                                // If device has a friendly name display it immediately.
                    //                                KnownDevices.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                    //                            }
                    //                            else
                    //                            {
                    //                                // Add it to a list in case the name gets updated later. 
                    //                                UnknownDevices.Add(deviceInfo);
                    //                            }
                    //                        }
                }
            }
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            string message = "自动发现设备停止";
            this.MessAgeChanged(MsgType.NotifyTxt, message);
        }

        public void StartMatching(BluetoothLEDevice Device)
        {
            CurrentDevice = Device;
        }

        public async Task FindService()
        {
            try
            {
                IReadOnlyList<GattDeviceService> GattServices = CurrentDevice.GattServices;

                foreach (var ser in GattServices)
                {
                    this.GattDeviceServiceAdded?.Invoke(ser);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public async Task FindCharacteristic(GattDeviceService gattDeviceService)
        {
            try
            {
                CurrentService = gattDeviceService;
                foreach (GattCharacteristic ser in gattDeviceService.GetAllCharacteristics())
                {
                    CharacteristicAdded?.Invoke(ser);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public async Task SetOpteron(GattCharacteristic gattCharacteristic)
        {
            if (gattCharacteristic.CharacteristicProperties == GattCharacteristicProperties.Write)
            {
                CurrentWriteCharacteristic = gattCharacteristic;
            }

            if (gattCharacteristic.CharacteristicProperties == GattCharacteristicProperties.Notify)
            {
                CurrentNotifyCharacteristic = gattCharacteristic;
                CurrentNotifyCharacteristic.ProtectionLevel = GattProtectionLevel.Plain;
                //                GattCharacteristic currentNotifyCharacteristic = CurrentNotifyCharacteristic;
                CurrentNotifyCharacteristic.ValueChanged += Characteristic_ValueChanged;

                await EnableNotifications(CurrentNotifyCharacteristic);
            }

            if (gattCharacteristic.CharacteristicProperties == GattCharacteristicProperties.WriteWithoutResponse)
            {
                CurrentWriteWithoutResponseCharacteristic = gattCharacteristic;
            }

            if (gattCharacteristic.CharacteristicProperties ==
                (GattCharacteristicProperties.Read | GattCharacteristicProperties.Write |
                 GattCharacteristicProperties.Notify))
            {
            }

            if (gattCharacteristic.CharacteristicProperties ==
                (GattCharacteristicProperties.Read | GattCharacteristicProperties.Write |
                 GattCharacteristicProperties.Notify))
            {
                CurrentWriteCharacteristic = gattCharacteristic;
                CurrentNotifyCharacteristic = gattCharacteristic;
                CurrentNotifyCharacteristic.ProtectionLevel = GattProtectionLevel.Plain;
                //                GattCharacteristic currentNotifyCharacteristic = CurrentNotifyCharacteristic;
                CurrentNotifyCharacteristic.ValueChanged += Characteristic_ValueChanged;

                await EnableNotifications(CurrentNotifyCharacteristic);
            }

            Connect();
        }

        private void Connect()
        {
            byte[] bytes = BitConverter.GetBytes(CurrentDevice.BluetoothAddress);
            Array.Reverse(bytes);
            CurrentDeviceMAC = BitConverter.ToString(bytes, 2, 6).Replace('-', ':').ToLower();
            string msg = "正在连接设备<" + CurrentDeviceMAC + ">..";
            MessAgeChanged?.Invoke(MsgType.NotifyTxt, msg);

            CurrentDevice.ConnectionStatusChanged += CurrentDevice_ConnectionStatusChanged;
        }


        private async Task Matching(string id)
        {
            try
            {
                BluetoothLEDevice.FromIdAsync(id).Completed =
                    (asyncInfo, asyncStatus) =>
                    {
                        if (asyncStatus == AsyncStatus.Completed)
                        {
                            BluetoothLEDevice bleDevice = asyncInfo.GetResults();
                            CurrentDevice = bleDevice;
                            _deviceList.Add(bleDevice);
                            this.DeviceWatcherChanged?.Invoke(MsgType.BleDevice, bleDevice);
                        }
                    };
            }
            catch (Exception ex)
            {
                Exception e = ex;
                string msg = "没有发现设备" + e.ToString();
                this.MessAgeChanged(MsgType.NotifyTxt, msg);
                StartBleDeviceWatcher();
            }
        }

        public void Dispose()
        {
            CurrentDeviceMAC = null;
            CurrentService?.Dispose();
            CurrentDevice?.Dispose();
            CurrentDevice = null;
            CurrentService = null;
            CurrentWriteCharacteristic = null;
            CurrentNotifyCharacteristic = null;
            this.MessAgeChanged?.Invoke(MsgType.NotifyTxt, "主动断开连接");
        }

        public void DisposeBle()
        {
            try
            {
                Thread.Sleep(100);


                Thread.Sleep(100);
                _deviceList = new List<BluetoothLEDevice>();
                CurrentDeviceMAC = null;
                CurrentService?.Dispose();
                Thread.Sleep(500);
                CurrentDevice?.Dispose();
                Thread.Sleep(500);
                CurrentDevice = null;
                CurrentService = null;

                CurrentWriteCharacteristic = null;
                CurrentWriteWithoutResponseCharacteristic = null;
                CurrentNotifyCharacteristic = null;

                this.MessAgeChanged?.Invoke(MsgType.NotifyTxt, "主动断开连接");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void CurrentDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            if (CurrentDeviceMAC == null)
            {
                MSState = string.Empty;
                string message = "设备已断开";
                this.MessAgeChanged(MsgType.NotifyTxt, message);
            }
            else if (sender.ConnectionStatus != 0 || CurrentDeviceMAC == null)
            {
                string message2 = "设备已连接";
                this.MessAgeChanged(MsgType.NotifyTxt, message2);
            }
        }

        public async Task SelectDeviceFromIdAsync(string MAC)
        {
            CurrentDeviceMAC = MAC;
            CurrentDevice = null;
            BluetoothAdapter.GetDefaultAsync().Completed = async delegate(IAsyncOperation<BluetoothAdapter> asyncInfo,
                AsyncStatus asyncStatus)
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    BluetoothAdapter mBluetoothAdapter = asyncInfo.GetResults();
                    byte[] _Bytes = BitConverter.GetBytes(mBluetoothAdapter.BluetoothAddress);
                    Array.Reverse(_Bytes);
                    string macAddress = BitConverter.ToString(_Bytes, 2, 6).Replace('-', ':').ToLower();
                    string Id = "BluetoothLE#BluetoothLE" + macAddress + "-" + MAC;
                    await Matching(Id);
                }
            };
        }

        public async Task EnableNotifications(GattCharacteristic characteristic)
        {
            string msg = "收通知对象=" + CurrentDevice.ConnectionStatus;
            this.MessAgeChanged(MsgType.NotifyTxt, msg);
            characteristic
                    .WriteClientCharacteristicConfigurationDescriptorAsync(
                        GattClientCharacteristicConfigurationDescriptorValue.Notify).Completed =
                async delegate(IAsyncOperation<GattCommunicationStatus> asyncInfo, AsyncStatus asyncStatus)
                {
                    if (asyncStatus == AsyncStatus.Completed)
                    {
                        GattCommunicationStatus status = asyncInfo.GetResults();
                        if (status == GattCommunicationStatus.Unreachable)
                        {
                            MSState = string.Empty;
                            msg = "设备不可用";
                            this.MessAgeChanged(MsgType.NotifyTxt, msg);
                            if (CurrentNotifyCharacteristic != null && !_asyncLock)
                            {
                                await EnableNotifications(CurrentNotifyCharacteristic);
                            }
                        }

                        _asyncLock = false;
                        MSState = status.ToString();
                        msg = "设备连接状态" + status;
                        this.MessAgeChanged(MsgType.NotifyTxt, msg);
                    }
                };
        }

        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out byte[] value);
            string message = "收到数据：" + BitConverter.ToString(value);
            this.MessAgeChanged(MsgType.BleData, message, value);
        }

        public async Task Write(byte[] data)
        {
            if (CurrentWriteCharacteristic != null)
            {
                await CurrentWriteCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(data),
                    GattWriteOption.WriteWithResponse);
                string str2 = "发送数据：" + BitConverter.ToString(data);
                this.MessAgeChanged(MsgType.BleData, str2, data);
            }

            if (CurrentWriteWithoutResponseCharacteristic != null)
            {
                await CurrentWriteWithoutResponseCharacteristic.WriteValueAsync(
                    CryptographicBuffer.CreateFromByteArray(data),
                    GattWriteOption.WriteWithoutResponse);
                string str = "发送数据：" + BitConverter.ToString(data);
                this.MessAgeChanged(MsgType.BleData, str, data);
            }
        }
    }
}