

using Microsoft.Extensions.DependencyInjection;

namespace DataNotification.ViewModel
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
            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            //SimpleIoc.Default.Register<TcpServerViewModel>();
            //SimpleIoc.Default.Register<NetLogViewModel>();
            //SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main => App.Current.Services.GetService<MainViewModel>();

        public TcpServerViewModel TcpServerViewModel => App.Current.Services.GetService<TcpServerViewModel>();
        public NetLogViewModel NetLogViewModel => App.Current.Services.GetService<NetLogViewModel>();
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}