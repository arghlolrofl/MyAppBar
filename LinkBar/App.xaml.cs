using System.Windows;
using Autofac;
using GalaSoft.MvvmLight.Threading;
using LinkBar.ViewModel;
using UtilityLib.Contracts;

namespace LinkBar {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        static App() {
            DispatcherHelper.Initialize();
        }

        private void App_OnStartup(object sender, StartupEventArgs e) {
            ViewModelLocator.InitContainer()
                .Resolve<IDisplayManager>()
                .SetMainWindow<MainWindow>();
        }
    }
}
