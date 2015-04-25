using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using LinkBar.Messages.View;
using LinkBar.ViewModel;
using MahApps.Metro.Controls;
using Microsoft.Practices.ServiceLocation;
using UtilityLib.Contracts;

namespace LinkBar {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        private readonly IMessenger _messenger;
#if !DEBUG
        private readonly DockingManager _dockManager = null;
#endif

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            Loaded += OnLoaded;
            Closing += OnClosing;

#if !DEBUG
            _dockManager = new DockingManager(this);
#endif
        }

        /// <summary>
        /// Undocks the main window if hard docking was enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cancelEventArgs"></param>
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs) {
#if !DEBUG
            _dockManager.Undock();
#endif
        }

        /// <summary>
        /// If in debug mode, only soft docking will be used. In release mode
        /// the main window hard docks to the desired screen edge.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
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

        //private void TreeViewItem_OnMouseDoubleClick(object sender, RoutedEventArgs e) {
        //    e.Handled = true;
        //}

        //private void TreeViewItem_OnMouseUp(object sender, MouseButtonEventArgs e) {
        //    if (!e.Handled && e.ChangedButton == MouseButton.Left) {
        //        var tvi = sender as TreeViewItem;

        //        if (tvi != null)
        //            tvi.IsExpanded = !tvi.IsExpanded;

        //        e.Handled = true;
        //    }
        //}
    }
}