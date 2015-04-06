using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
    public class LinkDialogViewModel : DialogViewModelBase {
        private List<Tag> _tagList;

        #region Tag Handling
        #region bool CanAddTags Property
        private bool _CanModifyTags;

        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool CanModifyTags {
            get { return _CanModifyTags; }
            set {
                if (_CanModifyTags == value)
                    return;

                _CanModifyTags = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region RelayCommand AddSelectedTagCommand Property
        private RelayCommand _AddSelectedTagCommand;
        /// <summary>
        /// Gets the AddSelectedTagCommand.
        /// </summary>
        public RelayCommand AddSelectedTagCommand {
            get {
                return _AddSelectedTagCommand
                       ?? (_AddSelectedTagCommand = new RelayCommand(OnAddSelectedTagCommand));
            }
        }
        #endregion

        #region RelayCommand DeselectTagCommand Property
        private RelayCommand _DeselectTagCommand;

        /// <summary>
        /// Gets the DeselectTagCommand.
        /// </summary>
        public RelayCommand DeselectTagCommand {
            get {
                return _DeselectTagCommand
                       ?? (_DeselectTagCommand = new RelayCommand(OnDeselectTagCommand));
            }
        }
        #endregion

        #region RelayCommand RemoveSelectedTagCommand Property
        private RelayCommand _RemoveSelectedTagCommand;
        /// <summary>
        /// Gets the RemoveSelectedTagCommand.
        /// </summary>
        public RelayCommand RemoveSelectedTagCommand {
            get {
                return _RemoveSelectedTagCommand
                       ?? (_RemoveSelectedTagCommand = new RelayCommand(OnRemoveSelectedTagCommand));
            }
        }
        #endregion
        #endregion

        #region View Data

        #region ObservableCollection<Category> Categories Property
        private ObservableCollection<Category> _Categories = null;

        /// <summary>
        /// Sets and gets the Links property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Category> Categories {
            get { return _Categories; }
            set {
                _Categories = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Category SelectedCategory Property
        private Category _SelectedCategory = null;

        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Category SelectedCategory {
            get { return _SelectedCategory; }
            set {
                _SelectedCategory = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Link SelectedLink Property
        private Link _SelectedLink = null;

        /// <summary>
        /// Sets and gets the Links property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Link SelectedLink {
            get { return _SelectedLink; }
            set {
                _SelectedLink = value;
                RaisePropertyChanged();
                OnSelectedLinkChanged();
            }
        }
        #endregion


        #region Tag AvailableTags Property
        private ObservableCollection<Tag> _AvailableTags = null;

        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Tag> AvailableTags {
            get { return _AvailableTags; }
            set {
                if (_AvailableTags == value)
                    return;

                _AvailableTags = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Tag SelectedUsedTag Property
        private Tag _SelectedUsedTag = null;

        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Tag SelectedUsedTag {
            get { return _SelectedUsedTag; }
            set {
                if (_SelectedUsedTag == value)
                    return;

                _SelectedUsedTag = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Tag SelectedAvailableTag Property
        private Tag _SelectedAvailableTag = null;

        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Tag SelectedAvailableTag {
            get { return _SelectedAvailableTag; }
            set {
                if (_SelectedAvailableTag == value)
                    return;

                _SelectedAvailableTag = value;
                RaisePropertyChanged();
                ValidateForm();
            }
        }
        #endregion

        #region string AdditionalTagsString Property
        private string _AdditionalTagsString;

        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string AdditionalTagsString {
            get { return _AdditionalTagsString; }
            set {
                if (_AdditionalTagsString == value)
                    return;

                _AdditionalTagsString = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #endregion



        /// <summary>
        /// Initializes a new instance of the LinkDialogViewModel class.
        /// </summary>
        public LinkDialogViewModel(ILifetimeScope autofacScope) : base(autofacScope) {
            DisableForm();

            MessengerInstance.Register<Messages.Response.LinkContentResponse>(this, msg => {
                Categories = msg.Categories;
                _tagList = msg.Tags.Any() ? new List<Tag>(msg.Tags) : new List<Tag>();
                SelectedCategory = Categories.FirstOrDefault();

                CreateNewCommand.Execute(null);

                ValidateForm();
                SetProgress(0, "Links loaded");
            });

            MessengerInstance.Send(new Messages.Request.LinkContentRequest());
        }


        protected override void OnCreateNewCommand() {
            SelectedLink = Autofac.Resolve<Link>();
        }

        protected override void OnSaveChangesCommand(ValidatableEntity entity) {
            var link = entity as Link;
            if (link != null) {
                link.Category = SelectedCategory;
                base.OnSaveChangesCommand(entity);
            }
        }

        protected override void UpdateUi(GuiAction action = GuiAction.Add) {
            switch (action) {
                case GuiAction.Add:
                    SelectedCategory.Links.Add(SelectedLink);
                    break;
                case GuiAction.Remove:
                    SelectedCategory.Links.Remove(SelectedLink);
                    CreateNewCommand.Execute(null);
                    break;
            }
        }

        protected override void ValidateForm() {
            if (SelectedCategory == null) {
                CanSaveChanges = false;
                CanDelete = false;
                CanCreateNew = false;
                return;
            }

            if (SelectedLink == null) {
                CanSaveChanges = false;
                CanDelete = false;
                CanCreateNew = true;
                return;
            }

            var cachedLink = (Link) CachedEntity;

            if (
                (
                    !String.IsNullOrEmpty(SelectedLink.Name)
                    && !String.IsNullOrEmpty(SelectedLink.Url)
                ) && (
                    cachedLink.Name != SelectedLink.Name
                    || cachedLink.Description != SelectedLink.Description
                    || cachedLink.Url != SelectedLink.Url
                    || cachedLink.Tags.Count != SelectedLink.Tags.Count
                )
            ) {
                CanSaveChanges = true;
            } else {
                CanSaveChanges = false;
            }

            CanDelete = CanCreateNew = SelectedLink.Id >= 1;
            CanModifyTags = SelectedLink.Id > 0 && (_SelectedAvailableTag != null || !String.IsNullOrEmpty(AdditionalTagsString));
        }

        protected override void OnWindowClosedCommand() {
            MessengerInstance.Send(new Messages.View.LinkDialogClosed());
            Cleanup();
        }


        private void OnSelectedLinkChanged() {
            if (_SelectedLink == null) {
                CachedEntity = null;
                return;
            }
            CachedEntity = _SelectedLink.Clone();
            SetAvailableTags();
        }

        private void OnAddSelectedTagCommand() {
            using (var repo = Autofac.Resolve<ILinkRepositoryAsync>()) {
                if (SelectedAvailableTag != null) { // add tag from listbox
                    repo.AddTagToLink(SelectedLink, SelectedAvailableTag);

                    SelectedLink.Tags.Add(SelectedAvailableTag);
                    AvailableTags.Remove(SelectedAvailableTag);
                } else { // add new tags from textbox
                    var newTags = AdditionalTagsString.Split(',');
                    for (var i = 0; i < newTags.Length; i++) 
                        newTags[i] = newTags[i].Trim();
                    
                    var addedTags = repo.AddNewTagsToLink(SelectedLink, newTags);

                    foreach (var newTag in addedTags)
                        SelectedLink.Tags.Add(newTag);

                }
                AdditionalTagsString = String.Empty;
            }
        }

        private void OnRemoveSelectedTagCommand() {
            using (var repo = Autofac.Resolve<ILinkRepositoryAsync>()) {
                repo.RemoveTagFromLink(SelectedLink, SelectedUsedTag);
            }
        }

        private void OnDeselectTagCommand() {
            SelectedAvailableTag = null;
        }

        private void SetAvailableTags() {
            if (SelectedLink.Tags.Count == 0) {
                AvailableTags = new ObservableCollection<Tag>(_tagList);
            } else {
                AvailableTags = new ObservableCollection<Tag>();
                var aTags = _tagList.Where(
                    t => SelectedLink.Tags.All(tag => tag.Id != t.Id)
                );
                foreach (var tag in aTags)
                    AvailableTags.Add(tag.Clone());
            }
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
