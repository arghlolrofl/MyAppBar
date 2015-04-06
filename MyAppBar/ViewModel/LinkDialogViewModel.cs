using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using MyAppBar.Contracts;
using MyAppBar.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace MyAppBar.ViewModel {
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LinkDialogViewModel : DialogViewModelBase {
        private List<Tag> _tagList;

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
                        MessengerInstance.Send(new Messages.View.LinkDialogClosed());
                    }));
            }
        }
        #endregion

        #region New Link command
        private RelayCommand _createNewCommand;
        /// <summary>
        /// Gets the CreateNewCommand.
        /// </summary>
        public override RelayCommand CreateNewCommand {
            get {
                return _createNewCommand
                    ?? (_createNewCommand = new RelayCommand(
                    () => {
                        SelectedLink = ServiceLocator.Current.GetInstance<Link>();
                    }));
            }
        }
        #endregion


        private RelayCommand _deleteSelectedTagCommand;
        /// <summary>
        /// Gets the DeleteSelectedTagCommand.
        /// </summary>
        public RelayCommand DeleteSelectedTagCommand {
            get {
                return _deleteSelectedTagCommand
                    ?? (_deleteSelectedTagCommand = new RelayCommand(OnDeleteSelectedTagCommand));
            }
        }
        private RelayCommand _addSelectedTagCommand;
        /// <summary>
        /// Gets the AddSelectedTagCommand.
        /// </summary>
        public RelayCommand AddSelectedTagCommand {
            get {
                return _addSelectedTagCommand
                    ?? (_addSelectedTagCommand = new RelayCommand(OnAddSelectedTagCommand));
            }
        }
        private RelayCommand _deselectTagCommand;
        /// <summary>
        /// Gets the DeselectTagCommand.
        /// </summary>
        public RelayCommand DeselectTagCommand {
            get {
                return _deselectTagCommand
                    ?? (_deselectTagCommand = new RelayCommand(
                    () => {
                        SelectedAvailableTag = null;
                    }));
            }
        }
        


        #region Category list Binding
        private ObservableCollection<Category> _Categories = null;
        /// <summary>
        /// Sets and gets the Links property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Category> Categories {
            get {
                return _Categories;
            }

            set {
                _Categories = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedCategory Binding
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

        #region SelectedLink Binding
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

                if (_SelectedLink == null) {
                    _cachedEntity = null;
                    return;
                }
                _cachedEntity = _SelectedLink.Clone();
                

                if (_SelectedLink.Tags.Count == 0) {
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
        }
        #endregion

        #region AvailableTags Binding
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

        #region SelectedUsedTag Binding
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

        #region SelectedAvailableTag Binding
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

        #region AdditionalTagsString Binding
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


        #region CanAddTags Binding
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

        /// <summary>
        /// Initializes a new instance of the LinkDialogViewModel class.
        /// </summary>
        public LinkDialogViewModel() {
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

        protected override void AddToUiCollection() {
            SelectedCategory.Links.Add(SelectedLink);
        }

        protected override void RemoveFromUiCollection() {
            SelectedCategory.Links.Remove(SelectedLink);
            CreateNewCommand.Execute(null);
        }

        private void OnDeleteSelectedTagCommand() {
            using (var ctx = ServiceLocator.Current.GetInstance<IDataService>()) {
                var link = ctx.Links.Find(SelectedLink.Id);
                var removedTag = ctx.Tags.Find(SelectedUsedTag.Id);

                link.Tags.Remove(removedTag);
                var task = ctx.SaveAsync(link);
                if (task.Result > 0) {
                    AvailableTags.Add(SelectedUsedTag);
                    SelectedLink.Tags.Remove(SelectedUsedTag);
                }
            }
        }

        private void OnAddSelectedTagCommand() {
            using (var ctx = ServiceLocator.Current.GetInstance<IDataService>()) {
                var link = ctx.Links.Find(SelectedLink.Id);

                // add tag from listbox
                if (SelectedAvailableTag != null) {
                    var addedTag = ctx.Tags.Find(SelectedAvailableTag.Id);

                    link.Tags.Add(addedTag);
                    var task = ctx.SaveAsync(link);
                    if (task.Result == 0) 
                        throw new Exception("DB Error while adding tag!");
                    
                    SelectedLink.Tags.Add(SelectedAvailableTag);
                    AvailableTags.Remove(SelectedAvailableTag);
                } else { // add new tags from textbox
                    var newTagIds = AdditionalTagsString.Split(',');
                    List<Tag> newTags = new List<Tag>();

                    foreach (var tagValue in newTagIds) {
                        var tag = ServiceLocator.Current.GetInstance<Tag>();
                        tag.Value = tagValue.Trim();
                        newTags.Add(tag);
                    }
                    ctx.SaveAsync<Tag>(newTags);

                    foreach (var newTag in newTags) 
                        link.Tags.Add(newTag);

                    var task = ctx.SaveAsync(link);
                    if (task.Result > 0) {
                        foreach (var newTag in newTags) 
                            SelectedLink.Tags.Add(newTag);
                        
                    }
                    AdditionalTagsString = String.Empty;
                }
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

            var cachedLink = (Link) _cachedEntity;

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

        public override void Cleanup() {
            MessengerInstance.Unregister<Messages.Response.CategoryListResponse>(this);

            base.Cleanup();
        }
    }
}
