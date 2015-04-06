using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
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
    public class CategoryDialogViewModel : DialogViewModelBase {
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
                        MessengerInstance.Send(new Messages.View.CategoryDialogClosed());
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
                        SelectedCategory = ServiceLocator.Current.GetInstance<Category>();
                    }));
            }
        }
        #endregion

        #region Category list Binding
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
                _cachedEntity = value != null
                    ? _SelectedCategory.Clone()
                    : null;

                RaisePropertyChanged();
            }
        }
        #endregion



        /// <summary>
        /// Initializes a new instance of the DialogWindowViewModel class.
        /// </summary>
        public CategoryDialogViewModel() {
            DisableForm();

            MessengerInstance.Register<Messages.Response.CategoryListResponse>(this, msg => {
                Categories = msg.List;
                CreateNewCommand.Execute(null);

                ValidateForm();
                SetProgress(0, "Categories loaded");
            });

            MessengerInstance.Send(new Messages.Request.CategoryListRequest());
        }

        protected override void AddToUiCollection() {
            Categories.Add(SelectedCategory);
        }

        protected override void RemoveFromUiCollection() {
            Categories.Remove(SelectedCategory);
            CreateNewCommand.Execute(null);
        }

        protected override void ValidateForm() {
            if (SelectedCategory == null) {
                CanSaveChanges = false;
                CanDelete = false;
                CanCreateNew = true;
                return;
            }

            var cachedCategory = (Category) _cachedEntity;

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

        public override void Cleanup() {
            MessengerInstance.Unregister<Messages.Response.CategoryListResponse>(this);

            base.Cleanup();
        }
    }
}
