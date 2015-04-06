using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Autofac;
using GalaSoft.MvvmLight.CommandWpf;
using LinkBar.Contracts;
using LinkBar.Model;
using Microsoft.Practices.ServiceLocation;
using UtilityLib.Mvvm;

namespace LinkBar.ViewModel {
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TagDialogViewModel : DialogViewModelBase {
        #region ObservableCollection<Tag> Tags Property
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

        #region Tag SelectedTag Property
        private Tag _SelectedTag = null;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Tag SelectedTag {
            get { return _SelectedTag; }
            set {
                _SelectedTag = value;
                CachedEntity = value != null
                    ? _SelectedTag.Clone()
                    : null;

                RaisePropertyChanged();
            }
        }
        #endregion

        

        /// <summary>
        /// Initializes a new instance of the TagDialogViewModel class.
        /// </summary>
        public TagDialogViewModel(ILifetimeScope autofacScope) : base(autofacScope) {
            DisableForm();

            MessengerInstance.Register<Messages.Response.TagListResponse>(this, msg => {
                Tags = msg.List;
                CreateNewCommand.Execute(null);
                
                SetProgress(0, "Tags loaded");
                ValidateForm();
            });

            MessengerInstance.Send(new Messages.Request.TagListRequest());
        }


        protected override void OnCreateNewCommand() {
            SelectedTag = Autofac.Resolve<Tag>();
        }

        protected override void UpdateUi(GuiAction action = GuiAction.Add) {
            switch (action) {
                case GuiAction.Add:
                    Tags.Add(SelectedTag);
                    break;
                case GuiAction.Remove:
                    Tags.Remove(SelectedTag);
                    CreateNewCommand.Execute(null);
                    break;
            }
        }
        
        protected override void ValidateForm() {
            var cachedTag = CachedEntity as Tag;

            if (SelectedTag == null) {
                CanSaveChanges = false;
                CanDelete = false;
                CanCreateNew = true;
                return;
            }

            if (!String.IsNullOrEmpty(SelectedTag.Value) && (cachedTag.Value != SelectedTag.Value)) {
                CanSaveChanges = true;
            } else {
                CanSaveChanges = false;
            }

            CanDelete = CanCreateNew = SelectedTag.Id >= 1;
        }

        protected override void OnWindowClosedCommand() {
            MessengerInstance.Send(new Messages.View.TagDialogClosed());
            Cleanup();
        }

        /// <summary>
        /// Cleans up the viewmodel.
        /// </summary>
        public override void Cleanup() {
            MessengerInstance.Unregister(this);
            base.Cleanup();
        }

    }
}