using GalaSoft.MvvmLight;

namespace 三相智慧能源网关调试软件.Model
{
   public class SenderModel:ObservableObject
   {
       private string _sendText;
       public string SendText
       {
           get => _sendText;
           set { _sendText = value; RaisePropertyChanged();}
       }

   }
}
