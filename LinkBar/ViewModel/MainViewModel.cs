using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Autofac;
using LinkBar.Contracts;
using Microsoft.Practices.ServiceLocation;
using UtilityLib.Contracts;
using GalaSoft.MvvmLight.CommandWpf;
using LinkBar.Model;
using LinkBar.View;

namespace LinkBar.ViewModel {
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase {
        private readonly ILifetimeScope _autofac;

        #region Window bindings
        #region bool IsWindowEnabled Property
        private bool _IsWindowEnabled = true;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWindowEnabled {
            get { return _IsWindowEnabled; }
            set {
                if (_IsWindowEnabled == value) return;

                _IsWindowEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region string TitleBarText Property
        private string _TitleBarText = "@TODO";
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string TitleBarText {
            get { return _TitleBarText; }
            set {
                if (_TitleBarText == value) return;

                _TitleBarText = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        
        #region Visibility SearchBoxVisibility Property
        private Visibility _SearchBoxVisibility = Visibility.Collapsed;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility SearchBoxVisibility {
            get { return _SearchBoxVisibility; }
            set {
                if (_SearchBoxVisibility == value) return;

                _SearchBoxVisibility = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        #endregion

        #region View commands
        #region RelayCommand OpenCategoryDialogCommand Property
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

                        _autofac.Resolve<IDisplayManager>().ShowDialogWindow<CategoryDialogWindow>();
                    }));
            }
        }
        #endregion

        #region RelayCommand OpenLinkDialogCommand Property
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

                        _autofac.Resolve<IDisplayManager>().ShowDialogWindow<LinkDialogWindow>();
                    }));
            }
        }
        #endregion

        #region RelayCommandOpenTagDialogCommand Property
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

                        _autofac.Resolve<IDisplayManager>().ShowDialogWindow<TagDialogWindow>();
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

        #region RelayCommand ToggleSearchModeCommand Property
        private RelayCommand _ToggleSearchModeCommand;
        /// <summary>
        /// Gets the ToggleSearchModeCommand.
        /// </summary>
        public RelayCommand ToggleSearchModeCommand {
            get {
                return _ToggleSearchModeCommand
                       ?? (_ToggleSearchModeCommand = new RelayCommand(OnToggleSearchModeCommand));
            }
        }
        #endregion
        #endregion


        #region View data
        #region ObservableCollection<Category> Categories Property
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

        #region ObservableCollection<Tag> Tags Property
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

        #region string SearchString Property
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


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <param name="lifetimeScope"></param>
        public MainViewModel(ILifetimeScope lifetimeScope) {
            IsWindowEnabled = false;

            _autofac = lifetimeScope;

            using (var repo = _autofac.Resolve<ILinkRepositoryAsync>()) {
                var categories = repo.FetchAllCategories();
                var tags = repo.FetchAllTags();

                Categories = new ObservableCollection<Category>(categories.Result);
                Tags = new ObservableCollection<Tag>(tags.Result);
            }

            MessengerInstance.Register<Messages.Request.CategoryListRequest>(this, msg => {
                MessengerInstance.Send(new Messages.Response.CategoryListResponse() { List = Categories });
            });

            MessengerInstance.Register<Messages.Request.TagListRequest>(this, msg => {
                MessengerInstance.Send(new Messages.Response.TagListResponse() { List = Tags });
            });

            MessengerInstance.Register<Messages.Request.LinkContentRequest>(this, msg => {
                MessengerInstance.Send(new Messages.Response.LinkContentResponse() { Tags = Tags, Categories = Categories });
            });

            IsWindowEnabled = true;
        }

        private void OnToggleSearchModeCommand() {
            SearchBoxVisibility = SearchBoxVisibility == Visibility.Collapsed 
                ? Visibility.Visible 
                : Visibility.Collapsed;


        }

        protected override void OnWindowClosedCommand() {
            Cleanup();
        }

        /// <summary>
        /// Executed when the view closed command is being fired.
        /// </summary>
        public override void Cleanup() {
            // Clean up if needed
            MessengerInstance.Unregister(this);
            _autofac.Dispose();
            base.Cleanup();
            ViewModelLocator.Cleanup();
        }
    }
}