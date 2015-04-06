using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.Practices.ServiceLocation;
using MyAppBar.ViewModel;
using UtilityLib.Contracts;

namespace MyAppBar {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
#if !DEBUG
        private readonly DockingManager _dockManager = null;
#endif

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow() {
            InitializeComponent();

            Closing += OnClosing;
            Loaded += OnLoaded;
            GotKeyboardFocus += (sender, args) => {
                ServiceLocator.Current.GetInstance<IDisplayManager>().FocusCurrentDialog();
            };

#if !DEBUG
            _dockManager = new DockingManager(this);
#endif
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs) {
            ViewModelLocator.Cleanup();

#if !DEBUG
            _dockManager.Undock();
#endif
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs) {
#if !DEBUG
            if (!_dockManager.IsDocked)
                _dockManager.Dock(DockAlignment.RightStretch);
#else
            Left = SystemParameters.WorkArea.Width - Width;
            Top = 0;
            Height = SystemParameters.WorkArea.Height;
#endif
        }

        private void TreeViewItem_OnMouseDoubleClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }

        private void TreeViewItem_OnMouseUp(object sender, MouseButtonEventArgs e) {
            if (!e.Handled && e.ChangedButton == MouseButton.Left) {
                var tvi = sender as TreeViewItem;

                if (tvi != null)
                    tvi.IsExpanded = !tvi.IsExpanded;

                e.Handled = true;
            }
        }
    }
}