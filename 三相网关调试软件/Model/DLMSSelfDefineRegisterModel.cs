using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;

namespace 三相智慧能源网关调试软件.Model
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/06/05 9:45:26
        主要用途：
        更改记录：
    */
   public class DLMSSelfDefineRegisterModel:DLMSRegister
    {
        public string RegisterName { get; set; }
        public DLMSSelfDefineRegisterModel(string logicName) : base(logicName)
        {
        }
    }

   public class DLMSSelfDefineData : DLMSData
   {
       public string DataName { get; set; }
        public DLMSSelfDefineData(string logicalName) : base(logicalName)
       {
       }
   }
}
