//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace 三相智慧能源网关调试软件.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=.\\UserInfo.mdb;")]
        public string AccessConnectionStr {
            get {
                return ((string)(this["AccessConnectionStr"]));
            }
            set {
                this["AccessConnectionStr"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Admin")]
        public string CurrentUser {
            get {
                return ((string)(this["CurrentUser"]));
            }
            set {
                this["CurrentUser"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string CurrentPassword {
            get {
                return ((string)(this["CurrentPassword"]));
            }
            set {
                this["CurrentPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool PasswordSave {
            get {
                return ((bool)(this["PasswordSave"]));
            }
            set {
                this["PasswordSave"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.1.100")]
        public string GatewayIpAddress {
            get {
                return ((string)(this["GatewayIpAddress"]));
            }
            set {
                this["GatewayIpAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{\"cmd_type\":13, \"data\":\"0\"}")]
        public string CmdStr1 {
            get {
                return ((string)(this["CmdStr1"]));
            }
            set {
                this["CmdStr1"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int InstantDataRefreshInterval {
            get {
                return ((int)(this["InstantDataRefreshInterval"]));
            }
            set {
                this["InstantDataRefreshInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\Config\\SerialPortViewModelConfig.json")]
        public string SerialPortViewModelConfigFilePath {
            get {
                return ((string)(this["SerialPortViewModelConfigFilePath"]));
            }
            set {
                this["SerialPortViewModelConfigFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BaseMeterUpGradeFile {
            get {
                return ((string)(this["BaseMeterUpGradeFile"]));
            }
            set {
                this["BaseMeterUpGradeFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF3F51B5")]
        public string PrimarySkin {
            get {
                return ((string)(this["PrimarySkin"]));
            }
            set {
                this["PrimarySkin"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1234")]
        public int GatewayPort {
            get {
                return ((int)(this["GatewayPort"]));
            }
            set {
                this["GatewayPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1500")]
        public int GatewayConnectTimeout {
            get {
                return ((int)(this["GatewayConnectTimeout"]));
            }
            set {
                this["GatewayConnectTimeout"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsDarkTheme {
            get {
                return ((bool)(this["IsDarkTheme"]));
            }
            set {
                this["IsDarkTheme"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF3F51B5")]
        public string AccentSkin {
            get {
                return ((string)(this["AccentSkin"]));
            }
            set {
                this["AccentSkin"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\TFTPServerFiles")]
        public string TftpServerDirectory {
            get {
                return ((string)(this["TftpServerDirectory"]));
            }
            set {
                this["TftpServerDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("TO DO")]
        public string Title {
            get {
                return ((string)(this["Title"]));
            }
            set {
                this["Title"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Data$")]
        public string DlmsDataSheetName {
            get {
                return ((string)(this["DlmsDataSheetName"]));
            }
            set {
                this["DlmsDataSheetName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("欢迎使用我的软件！")]
        public string OpenSound {
            get {
                return ((string)(this["OpenSound"]));
            }
            set {
                this["OpenSound"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ProfileGeneric$")]
        public string DlmsProfileGenericSheetName {
            get {
                return ((string)(this["DlmsProfileGenericSheetName"]));
            }
            set {
                this["DlmsProfileGenericSheetName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\TFTPClientFiles")]
        public string TftpClientDirectory {
            get {
                return ((string)(this["TftpClientDirectory"]));
            }
            set {
                this["TftpClientDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:5000/api")]
        public string WebApiUrl {
            get {
                return ((string)(this["WebApiUrl"]));
            }
            set {
                this["WebApiUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DLMS设备信息.xls")]
        public string ExcelFileName {
            get {
                return ((string)(this["ExcelFileName"]));
            }
            set {
                this["ExcelFileName"] = value;
            }
        }
    }
}
