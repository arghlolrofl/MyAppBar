using System;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using MyAppBar.Model;
using UtilityLib.Contracts;
using GalaSoft.MvvmLight.CommandWpf;
using MyAppBar.Contracts;
using MyAppBar.View;

namespace MyAppBar.ViewModel {
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase {
        #region Window bindings
        #region TitleBarText Binding
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

        #region IsWindowEnabled Binding
        private bool _IsWindowEnabled = true;
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

        #region SearchBoxVisibility Binding
        private Visibility _SearchBoxVisibility = Visibility.Collapsed;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility SearchBoxVisibility {
            get { return _SearchBoxVisibility; }
            set {
                if (_SearchBoxVisibility == value)
                    return;

                _SearchBoxVisibility = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        #endregion

        #region View commands
        #region Open category dialog command
        private RelayCommand _openCategoryDialogCommand;
        /// <summary>
        /// Gets the CreateNewCategoryCommand.
        /// </summary>
        public RelayCommand OpenCategoryDialogCommand {
            get {
                return _openCategoryDialogCommand
                    ?? (_openCategoryDialogCommand = new RelayCommand(
                    () => {
                        IsWindowEnabled = false;

                        MessengerInstance.Register<Messages.View.CategoryDialogClosed>(
                            this, msg => { IsWindowEnabled = true; }
                        );

                        ServiceLocator.Current.GetInstance<IDisplayManager>().ShowDialogWindow<CategoryDialogWindow>();
                    }));
            }
        }
        #endregion

        #region Open link dialog command
        private RelayCommand _openLinkDialogCommand;
        /// <summary>
        /// Gets the OpenLinkDialogCommand.
        /// </summary>
        public RelayCommand OpenLinkDialogCommand {
            get {
                return _openLinkDialogCommand
                    ?? (_openLinkDialogCommand = new RelayCommand(
                    () => {
                        if (Categories.Count == 0) {
                            MessageBox.Show("Please add at least one category first!", "No categories found");
                            return;
                        }

                        IsWindowEnabled = false;

                        MessengerInstance.Register<Messages.View.LinkDialogClosed>(
                            this, msg => { IsWindowEnabled = true; }
                        );

                        ServiceLocator.Current.GetInstance<IDisplayManager>().ShowDialogWindow<LinkDialogWindow>();
                    }));
            }
        }
        #endregion

        #region Open tag dialog command
        private RelayCommand _openTagDialogCommand;
        /// <summary>
        /// Gets the OpenTagDialogCommand.
        /// </summary>
        public RelayCommand OpenTagDialogCommand {
            get {
                return _openTagDialogCommand
                    ?? (_openTagDialogCommand = new RelayCommand(
                    () => {
                        IsWindowEnabled = false;

                        MessengerInstance.Register<Messages.View.TagDialogClosed>(
                            this, msg => { IsWindowEnabled = true; }
                        );

                        ServiceLocator.Current.GetInstance<IDisplayManager>().ShowDialogWindow<TagDialogWindow>();
                    }));
            }
        }
        #endregion

        #region Open note dialog command
        private RelayCommand _openNoteDialogCommand;
        /// <summary>
        /// Gets the OpenNoteDialogCommand.
        /// </summary>
        public RelayCommand OpenNoteDialogCommand {
            get {
                return _openNoteDialogCommand
                    ?? (_openNoteDialogCommand = new RelayCommand(
                    () => {
                        //IsWindowEnabled = false;

                        //MessengerInstance.Register<Messages.View.NoteDialogClosed>(
                        //    this, (msg) => { IsWindowEnabled = true; }
                        //);

                        //ServiceLocator.Current.GetInstance<IDisplayManager>().ShowDialogWindow<NoteDialogWindow>();
                    }));
            }
        }
        #endregion

        #region Enable search mode command
        private RelayCommand _enableSearchModeCommand;
        /// <summary>
        /// Gets the EnableSearchModeCommand.
        /// </summary>
        public RelayCommand EnableSearchModeCommand {
            get {
                return _enableSearchModeCommand
                    ?? (_enableSearchModeCommand = new RelayCommand(
                    () => {
                        SearchBoxVisibility = Visibility.Visible;
                    }));
            }
        }
        #endregion

        #region Disable search mode command
        private RelayCommand _disableSearchModeCommand;
        /// <summary>
        /// Gets the DisableSearchModeCommand.
        /// </summary>
        public RelayCommand DisableSearchModeCommand {
            get {
                return _disableSearchModeCommand
                    ?? (_disableSearchModeCommand = new RelayCommand(
                    () => {
                        SearchBoxVisibility = Visibility.Collapsed;
                    }));
            }
        }
        #endregion

        #region ViewClosedCommand
        private RelayCommand _viewClosedCommand;
        /// <summary>
        /// Gets the WindowClosedCommand.
        /// </summary>
        public RelayCommand ViewClosedCommand {
            get {
                return _viewClosedCommand
                    ?? (_viewClosedCommand = new RelayCommand(() => {
                        MessengerInstance.Send(new Messages.View.CategoryDialogClosed());
                        Cleanup();
                    }));
            }
        }
        #endregion
        #endregion
        
        #region View data
        #region List of categories binding
        private ObservableCollection<Category> _categories = null;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Category> Categories {
            get { return _categories; }
            set {
                _categories = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region List of tags binding
        private ObservableCollection<Tag> _Tags = null;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Tag> Tags {
            get { return _Tags; }
            set {
                _Tags = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SearchString Binding
        private string _SearchString = String.Empty;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SearchString {
            get { return _SearchString; }
            set {
                if (_SearchString == value)
                    return;

                _SearchString = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        #endregion
        

        #region IsBusy Binding
        private bool _IsBusy = false;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsBusy {
            get { return _IsBusy; }
            set {
                if (_IsBusy == value)
                    return;

                _IsBusy = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel() {
            enableBusyState();

            using (var ctx = ServiceLocator.Current.GetInstance<IDataService>()) {
                Categories = new ObservableCollection<Category>(ctx.GetCategoriesAsync());
                Tags = new ObservableCollection<Tag>(ctx.GetTagsAsync());
            }

            MessengerInstance.Register<Messages.Request.CategoryListRequest>(this, msg => {
                MessengerInstance.Send(new Messages.Response.CategoryListResponse() { List = Categories });
            });

            MessengerInstance.Register<Messages.Request.TagListRequest>(this, msg => {
                MessengerInstance.Send(new Messages.Response.TagListResponse() { List = Tags });
            });

            MessengerInstance.Register<Messages.Request.LinkContentRequest>(this, msg => {
                MessengerInstance.Send(new Messages.Response.LinkContentResponse() { Tags = Tags, Categories = Categories});
            });

            disableBusyState();
        }

        private void enableBusyState() {
            IsBusy = true;
        }

        private void disableBusyState() {
            IsBusy = false;
        }

        public override void Cleanup() {
            MessengerInstance.Unregister(this);

            base.Cleanup();
        }
    }
}