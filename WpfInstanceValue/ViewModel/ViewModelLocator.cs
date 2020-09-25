/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:WpfInstanceValue"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using 三相智慧能源网关调试软件.DLMS;

namespace WpfInstanceValue.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            SimpleIoc.Default.Register<SerialPortViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MyDLMSSettings>();
            SimpleIoc.Default.Register<DLMSClient>();
          
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public SerialPortViewModel SerialPortViewModel => ServiceLocator.Current.GetInstance<SerialPortViewModel>();
        public MyDLMSSettings MyDlmsSettings => ServiceLocator.Current.GetInstance<MyDLMSSettings>();
        public DLMSClient Client => ServiceLocator.Current.GetInstance<DLMSClient>();
     
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}