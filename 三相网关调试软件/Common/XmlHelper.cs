using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.Common
{
    public static class XmlHelper
    {
        public static XMLLogViewModel Logger = App.Current.Services.GetService<XMLLogViewModel>();

        public static void XmlCommon<T>(T t)
        {
            if (t == null)
            {
                return;
            }

            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", ""); //去掉Namespaces
                //OmitXmlDeclaration = true 省略XML声明
                XmlWriterSettings settings = new XmlWriterSettings
                { OmitXmlDeclaration = true, Indent = true, Encoding = Encoding.UTF8 };
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    xmlSerializer.Serialize(xmlWriter, t, ns);
                    Logger.XmlLog = stringWriter + Environment.NewLine + "-----萌萌哒分割线-----\r\n";
                }
            }
        }
    }
}