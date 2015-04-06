using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
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
    public class CategoryDialogViewModel : DialogViewModelBase {
        #region ObservableCollection<Category> Categories Property
        private ObservableCollection<Category> _categories = null;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Category> Categories {
            get {
                return _categories;
            }

            set {
                _categories = value;
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
                CachedEntity = value != null
                    ? _SelectedCategory.Clone()
                    : null;

                RaisePropertyChanged();
            }
        }
        #endregion


        /// <summary>
        /// Initializes a new instance of the DialogWindowViewModel class.
        /// </summary>
        public CategoryDialogViewModel(ILifetimeScope autofacScope)
            : base(autofacScope) {
            DisableForm();

            MessengerInstance.Register<Messages.Response.CategoryListResponse>(this, msg => {
                Categories = msg.List;
                CreateNewCommand.Execute(null);

                ValidateForm();
                SetProgress(0, "Categories loaded");
            });

            MessengerInstance.Send(new Messages.Request.CategoryListRequest());
        }


        protected override void OnCreateNewCommand() {
            SelectedCategory = Autofac.Resolve<Category>();
        }

        protected override void UpdateUi(GuiAction action = GuiAction.Add) {
            switch (action) {
                case GuiAction.Add:
                    Categories.Add(SelectedCategory);
                    break;
                case GuiAction.Remove:
                    Categories.Remove(SelectedCategory);
                    CreateNewCommand.Execute(null);
                    break;
            }
        }

        protected override void ValidateForm() {
            var cachedCategory = CachedEntity as Category;

            if (SelectedCategory == null) {
                CanSaveChanges = false;
                CanDelete = false;
                CanCreateNew = true;
                return;
            }

            if (
                !String.IsNullOrEmpty(SelectedCategory.Name)
                && (
                    cachedCategory.Name != SelectedCategory.Name
                    || cachedCategory.Description != SelectedCategory.Description
                )
            ) {
                CanSaveChanges = true;
            } else {
                CanSaveChanges = false;
            }

            CanDelete = CanCreateNew = SelectedCategory.Id >= 1;
        }

        protected override void OnWindowClosedCommand() {
            MessengerInstance.Send(new Messages.View.CategoryDialogClosed());
            Cleanup();
        }

        /// <summary>
        /// Cleans up the viewmodel.
        /// </summary>
        public override void Cleanup() {
            MessengerInstance.Unregister<Messages.Response.CategoryListResponse>(this);
            base.Cleanup();
        }
    }
}
