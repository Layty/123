using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MySerialPortMaster
{
    /// <summary>
    /// 串口配置文件管理者，
    /// 负责产生配置文件给串口，负责备份串口参数,从Json文件中导入参数，将参数写入Json文件
    /// 采用备忘录模式进行设计，将串口状态参数保存至外部
    /// </summary>
    public class SerialPortConfigCaretaker
    {
        private IDictionary<string, SerialPortConfig> _jsonMap = new Dictionary<string, SerialPortConfig>();
        public IDictionary<string, SerialPortConfig> Dictionary = new Dictionary<string, SerialPortConfig>();

        /// <summary>
        /// 串口配置文件路径
        /// </summary>
        public string SerialPortConfigFilePath { get; set; }

        public SerialPortConfig DefaultConfig { get; set; } =
            new SerialPortConfig()
            {
                BaudRate = 9600,
                DataBits = 8,
                DelayTimeOut = 2,
                IsAutoDataReceived = false,
                IsOwnThisSerialPort = false,
                Parity = "None",
                PortName = "COM1",
                StopBits = "One"
            };

        public SerialPortConfigCaretaker()
        {
        }

        public SerialPortConfigCaretaker(string serialPortConfigFilePath)
        {
            SerialPortConfigFilePath = serialPortConfigFilePath;
        }

        /// <summary>
        /// 向内存中添加或修改配置参数列表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="config"></param>
        public void AddSerialPortConfigToList(string key, SerialPortConfig config)
        {
            Dictionary[key] = config;
        }
        /// <summary>
        /// 从内存中移除相应键值的数据
        /// </summary>
        /// <param name="key"></param>
        public void RemoveSerialPortConfigFromList(string key)
        {
            if (Dictionary.ContainsKey(key))
            {
                Dictionary.Remove(key);
            }
            else
            {
                throw new Exception($"不存在key为{key}的配置");
            }
        }

        /// <summary>
        /// 将SerialPortConfig保存至Json文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="key"></param>
        /// <exception cref="SerialPortConfigFilePath">SerialPortConfigFilePath不能为空</exception>
        public void SaveSerialPortConfigDataToJsonFile(SerialPortConfig config, string key = "1")
        {
            if (string.IsNullOrEmpty(SerialPortConfigFilePath))
            {
                throw new Exception("SerialPortConfigFilePath不能为空");
            }

            FileStream stream = new FileStream(SerialPortConfigFilePath, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(stream))
            {
                _jsonMap[key] = config;
                if (_jsonMap?[key].PortName != null)
                {
                    var jsonText = JsonConvert.SerializeObject(_jsonMap, Formatting.Indented); //格式化输出json字符串
                    sw.Write(jsonText);
                }
            }
        }

        /// <summary>
        /// 从json文件中加载串口参数
        /// </summary>
        /// <param name="key">代表要加载的键</param>
        /// <returns>返回key对应的SerialPortConfig</returns>
        public SerialPortConfig LoadSerialPortParamsByReadSerialPortConfigFile(string key = "1")
        {
            SerialPortConfig mySerialPortConfig = new SerialPortConfig();
            //判断是否存在配置文件，不存在直接创建并写入默认参数，
            if (!File.Exists(SerialPortConfigFilePath))
            {
                return DefaultConfig;
            }
            else
            {
                //如存在读取文件内容进行赋值
                FileStream fileStream = new FileStream(SerialPortConfigFilePath, FileMode.Open);
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string jsonText = sr.ReadToEnd();
                    _jsonMap = JsonConvert.DeserializeObject<IDictionary<string, SerialPortConfig>>(jsonText);
                    if (_jsonMap != null)
                    {
                        mySerialPortConfig = _jsonMap[key];
                        return mySerialPortConfig;
                    }
                }
            }

            return mySerialPortConfig;
        }
    }
}