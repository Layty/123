using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;

namespace MySerialPortMaster
{
    /// <summary>
    /// 根据业务类型，为串口配置定义了基本的默认配置
    /// </summary>
    public enum ConfigType
    {
        Default,
        DLT645,
        Modbus,
        DLMS
    }
    /// <summary>
    /// 串口配置文件管理者
    /// 负责产生配置文件给串口，负责备份串口参数,从Json文件中导入参数，将参数写入Json文件
    /// 采用备忘录模式进行设计，将串口状态参数保存至外部
    /// 可以进行备份，撤销操作
    /// </summary>
    public sealed class SerialPortConfigCaretaker
    {
        private IDictionary<string, SerialPortConfig> _jsonMap = new Dictionary<string, SerialPortConfig>();

        private readonly IList<SerialPortConfig> _serialPortConfigs = new List<SerialPortConfig>();

        /// <summary>
        /// 串口配置文件路径
        /// </summary>
        public string SerialPortConfigFilePath { get; set; }

        public static SerialPortConfig DefaultConfig { get; set; } =
            new SerialPortConfig("COM1", 9600, StopBits.One, Parity.None, 8, 2, true, false, 2);


        /// <summary>
        /// 无参构造其，默认调用.\\Config\\SerialPortConfig.json的配置参数
        /// </summary>
        public SerialPortConfigCaretaker() : this(".\\Config\\SerialPortConfig.json")
        {
           
        }

        public SerialPortConfigCaretaker(string serialPortConfigFilePath)
        {
            if (string.IsNullOrEmpty(serialPortConfigFilePath))
                throw new ArgumentException("Value cannot be null or empty.", nameof(serialPortConfigFilePath));
            SerialPortConfigFilePath = serialPortConfigFilePath;
        }

        public void BackUp(SerialPortConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _serialPortConfigs.Add(config);
        }

        public SerialPortConfig Undo()
        {
            if (_serialPortConfigs.Count == 0)
            {
                return null;
            }
            var memento = this._serialPortConfigs.Last();
            _serialPortConfigs.Remove(memento);
            return memento;
        }

        public SerialPortConfig LoadSerialPortConfigFromJsonFile(ConfigType type = ConfigType.Default)
        {

            //判断是否存在配置文件，不存在直接创建并写入默认参数，
            if (!File.Exists(SerialPortConfigFilePath))
            {
                return DefaultConfig;
            }

            //如存在读取文件内容进行赋值
            FileStream fileStream = new FileStream(SerialPortConfigFilePath, FileMode.Open);
            using (StreamReader sr = new StreamReader(fileStream))
            {
                string jsonText = sr.ReadToEnd();
                _jsonMap = JsonConvert.DeserializeObject<IDictionary<string, SerialPortConfig>>(jsonText);
                if (_jsonMap != null)
                {
                    SerialPortConfig config = _jsonMap[type.ToString()];
                    return config;
                }

            }

            return null;
        }
        public void SaveSerialPortConfigToJsonFile(SerialPortConfig config, ConfigType type = ConfigType.Default)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
           
            if (string.IsNullOrEmpty(SerialPortConfigFilePath))
            {
                throw new Exception($"{nameof(SerialPortConfigFilePath)}不能为空");
            }

            FileStream stream = new FileStream(SerialPortConfigFilePath, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(stream))
            {
                _jsonMap[type.ToString()] = config;
                if (_jsonMap?[type.ToString()].PortName != null)
                {
                    var jsonText = JsonConvert.SerializeObject(_jsonMap, Formatting.Indented); //格式化输出json字符串
                    sw.Write(jsonText);
                }
            }
        }
    }
}