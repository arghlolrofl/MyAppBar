using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;
using MyAppBar.Contracts;
using MyAppBar.Model;

namespace MyAppBar.ViewModel {
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TagDialogViewModel : DialogViewModelBase {
        #region WindowClosedCommand
        private RelayCommand _windowClosedCommand;
        /// <summary>
        /// Gets the WindowClosedCommand.
        /// </summary>
        public override RelayCommand WindowClosedCommand {
            get {
                return _windowClosedCommand
                    ?? (_windowClosedCommand = new RelayCommand(() => {
                        Cleanup();
                        MessengerInstance.Send(new Messages.View.TagDialogClosed());
                    }));
            }
        }
        #endregion

        #region New category command
        private RelayCommand _createNewCommand;
        /// <summary>
        /// Gets the CreateNewCommand.
        /// </summary>
        public override RelayCommand CreateNewCommand {
            get {
                return _createNewCommand
                    ?? (_createNewCommand = new RelayCommand(
                    () => {
                        SelectedTag = ServiceLocator.Current.GetInstance<Tag>();
                    }));
            }
        }
        #endregion
        
        #region Tag list Binding
        private ObservableCollection<Tag> _tags = null;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Tag> Tags {
            get {
                return _tags;
            }

            set {
                _tags = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedTag Binding
        private Tag _SelectedTag = null;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Tag SelectedTag {
            get { return _SelectedTag; }
            set {
                _SelectedTag = value;
                _cachedEntity = value != null
                    ? _SelectedTag.Clone()
                    : null;

                RaisePropertyChanged();
            }
        }
        #endregion

        

        /// <summary>
        /// Initializes a new instance of the TagDialogViewModel class.
        /// </summary>
        public TagDialogViewModel() {
            DisableForm();

            MessengerInstance.Register<Messages.Response.TagListResponse>(this, msg => {
                Tags = msg.List;
                CreateNewCommand.Execute(null);
                
                SetProgress(0, "Tags loaded");
                ValidateForm();
            });

            MessengerInstance.Send(new Messages.Request.TagListRequest());
        }

        protected override void AddToUiCollection() {
            Tags.Add(SelectedTag);
        }

        protected override void RemoveFromUiCollection() {
            Tags.Remove(SelectedTag);
            CreateNewCommand.Execute(null);
        }
        
        protected override void ValidateForm() {
            if (SelectedTag == null) {
                CanSaveChanges = false;
                CanDelete = false;
                CanCreateNew = true;
                return;
            }

            var cachedTag = (Tag) _cachedEntity;

            if (!String.IsNullOrEmpty(SelectedTag.Value) && (cachedTag.Value != SelectedTag.Value)) {
                CanSaveChanges = true;
            } else {
                CanSaveChanges = false;
            }

            CanDelete = CanCreateNew = SelectedTag.Id >= 1;
        }

        public override void Cleanup() {
            MessengerInstance.Unregister<Messages.Response.TagListResponse>(this);

            base.Cleanup();
        }

    }
}