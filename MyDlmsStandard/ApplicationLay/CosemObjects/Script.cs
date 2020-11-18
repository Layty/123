using System.Collections.Generic;

namespace MyDlmsStandard.ApplicationLay.CosemObjects
{
    public class Script
    {
     //   脚本标识符0(script_identifier0)保留。若规定一个执行方法的标 识符(script_identifier)为0,将产生一个空脚本(无任何可执行的操 作)。
        public ushort ScriptIdentifier { get; set; }
        public List<ScriptAction> Actions { get; set; }

        public Script()
        {
            Actions = new List<ScriptAction>();
        }
    }
}