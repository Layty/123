using System;

namespace 三相智慧能源网关调试软件.Model
{
    public class EmptyResponsedException:Exception
    {
        public EmptyResponsedException(string message):base(message)
        {
            
        }
    }

}
