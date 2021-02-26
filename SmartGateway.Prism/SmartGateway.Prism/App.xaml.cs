using Prism.Ioc;
using Prism.Modularity;
using SmartGateway.Prism.Modules.ModuleName;
using SmartGateway.Prism.Services;
using SmartGateway.Prism.Services.Interfaces;
using SmartGateway.Prism.Views;
using System.Windows;

namespace SmartGateway.Prism
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
        }
    }
}
