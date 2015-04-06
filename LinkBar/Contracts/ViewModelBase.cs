using GalaSoft.MvvmLight.CommandWpf;
namespace LinkBar.Contracts {
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase {
        #region RelayCommand WindowClosedCommand Property
        private RelayCommand _WindowClosedCommand;
        /// <summary>
        /// Gets the WindowClosedCommand. Binded to the window
        /// closed event.
        /// </summary>
        public RelayCommand WindowClosedCommand {
            get {
                return _WindowClosedCommand ?? (_WindowClosedCommand = new RelayCommand(OnWindowClosedCommand));
            }
        }
        #endregion

        #region String TitleBarText Property
        private string _TitleBarText = "@TODO";
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string TitleBarText {
            get { return _TitleBarText; }
            set {
                if (_TitleBarText == value)
                    return;

                _TitleBarText = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Bool IsWindowEnabled Property
        private bool _IsWindowEnabled = false;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWindowEnabled {
            get { return _IsWindowEnabled; }
            set {
                if (_IsWindowEnabled == value)
                    return;

                _IsWindowEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// Executed through the WindowClosedCommand.
        /// </summary>
        protected abstract void OnWindowClosedCommand();
    }
}