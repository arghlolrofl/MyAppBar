using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls;
using MyAppBar.Model;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;

namespace MyAppBar.Contracts {
    public abstract class DialogViewModelBase : ViewModelBase {
        protected EntityBase _cachedEntity = null;

        #region ProgressValue Binding
        private int _ProgressValue = 0;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int ProgressValue {
            get { return _ProgressValue; }
            set {
                if (_ProgressValue == value)
                    return;

                _ProgressValue = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region DialogStatusText Binding
        private string _DialogStatusText = "Loading";
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DialogStatusText {
            get { return _DialogStatusText; }
            set {
                if (_DialogStatusText == value)
                    return;

                _DialogStatusText = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region CanCreateNew Binding
        private bool _CanCreateNew = true;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool CanCreateNew {
            get { return _CanCreateNew; }
            set {
                if (_CanCreateNew == value)
                    return;

                _CanCreateNew = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CanDelete Binding
        private bool _CanDelete = false;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool CanDelete {
            get { return _CanDelete; }
            set {
                if (_CanDelete == value)
                    return;

                _CanDelete = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CanSaveChanges Binding
        private bool _CanSaveChanges = false;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool CanSaveChanges {
            get { return _CanSaveChanges; }
            set {
                if (_CanSaveChanges == value)
                    return;

                _CanSaveChanges = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Form is dirty command
        private RelayCommand _isDirtyCommand;
        /// <summary>
        /// Gets the IsDirtyCommand.
        /// </summary>
        public RelayCommand IsDirtyCommand {
            get {
                return _isDirtyCommand ?? (_isDirtyCommand = new RelayCommand(ValidateForm));
            }
        }
        #endregion

        #region Save changes command
        private RelayCommand<EntityBase> _saveChangesCommand;
        /// <summary>
        /// Gets the SaveChangesCommand.
        /// </summary>
        public RelayCommand<EntityBase> SaveChangesCommand {
            get {
                return _saveChangesCommand
                    ?? (_saveChangesCommand = new RelayCommand<EntityBase>(
                    e => {
                        SetProgress(0, "Saving ...");
                        DisableForm();
                        if (UpdateModel(e))
                            AddToUiCollection();
                        ValidateForm();
                        SetProgress(100, "Update successful");
                    }));
            }
        }
        #endregion

        #region Delete command
        private RelayCommand<EntityBase> _deleteCommand;
        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand<EntityBase> DeleteCommand {
            get {
                return _deleteCommand
                    ?? (_deleteCommand = new RelayCommand<EntityBase>(
                    e => {
                        SetProgress(0, "Deleting entity");
                        DisableForm();
                        Delete(e);
                        RemoveFromUiCollection();
                        ValidateForm();
                        SetProgress(100, "Entity successfully deleted");
                    }));
            }
        }
        #endregion

        public abstract RelayCommand WindowClosedCommand { get; }
        public abstract RelayCommand CreateNewCommand { get; }

        protected abstract void ValidateForm();
        protected abstract void AddToUiCollection();
        protected abstract void RemoveFromUiCollection();

        protected bool UpdateModel<T>(T entity) where T : EntityBase {
            if (entity == null)
                throw new NullReferenceException("Entity given is null!");

            var isNew = entity.Id == 0;

            using (var ctx = ServiceLocator.Current.GetInstance<IDataService>()) {
                SetProgress(33);
                var task = ctx.SaveAsync(entity);

                // @TODO: do something here?

                task.Wait();

                SetProgress(66);
                if (task.Result == 0)
                    throw new Exception("Error deleting entity: " + entity.Id);
            }
            return isNew;
        }

        protected void Delete<T>(T entity) where T : EntityBase {
            if (entity == null)
                throw new Exception("Update model called without an entity in cache!");

            using (var ctx = ServiceLocator.Current.GetInstance<IDataService>()) {
                SetProgress(33);
                var task = ctx.DeleteAsync(entity);

                task.Wait();
                SetProgress(66);
                if (task.Result == 0)
                    throw new Exception("Error deleting entity: " + entity.Id);
            }
        }

        protected void DisableForm() {
            CanDelete = false;
            CanSaveChanges = false;
            CanCreateNew = false;
        }

        protected void SetProgress(int value, string msg = null) {
            ProgressValue = value;
            if (!String.IsNullOrEmpty(msg)) {
                DialogStatusText = msg;
            }
        }
    }
}
