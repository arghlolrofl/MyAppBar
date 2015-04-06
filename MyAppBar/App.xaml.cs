using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using MyAppBar.ViewModel;
using UtilityLib.Contracts;

namespace MyAppBar {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        static App() {
            DispatcherHelper.Initialize();
        }

        private void App_OnStartup(object sender, StartupEventArgs e) {
            ViewModelLocator.Init();
            ServiceLocator.Current.GetInstance<IDisplayManager>().SetMainWindow<MainWindow>();
        }
    }
}
