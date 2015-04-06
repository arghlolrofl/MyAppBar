using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using GalaSoft.MvvmLight.CommandWpf;
using UtilityLib.Mvvm;

namespace LinkBar.Contracts {

    public abstract class DialogViewModelBase : ViewModelBase {
        protected enum GuiAction { Add, Remove }
        protected readonly ILifetimeScope Autofac;
        protected ValidatableEntity CachedEntity = null;

        #region bool CanCreateNew Property
        private bool _CanCreateNew = false;
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

        #region bool CanSaveChanges Property
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

        #region bool CanDelete Property
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

        #region bool FormIsDirty Property
        private bool _FormIsDirty = false;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool FormIsDirty {
            get { return _FormIsDirty; }
            set {
                if (_FormIsDirty == value)
                    return;

                _FormIsDirty = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region int ProgressValue Property
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

        #region string StatusText Property
        private string _StatusText = "Loading ...";
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string StatusText {
            get { return _StatusText; }
            set {
                if (_StatusText == value)
                    return;

                _StatusText = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region RelayCommand IsDirtyCommand Property
        private RelayCommand _IsDirtyCommand;
        /// <summary>
        /// Gets the IsDirtyCommand.
        /// </summary>
        public RelayCommand IsDirtyCommand {
            get {
                return _IsDirtyCommand
                       ?? (_IsDirtyCommand = new RelayCommand(OnIsDirtyCommand));
            }
        }
        #endregion

        #region RelayCommand CreateNewCommand Property
        private RelayCommand _CreateNewCommand;
        /// <summary>
        /// Gets the CreateNewCommand.
        /// </summary>
        public RelayCommand CreateNewCommand {
            get {
                return _CreateNewCommand
                       ?? (_CreateNewCommand = new RelayCommand(OnCreateNewCommand));
            }
        }
        #endregion

        #region RelayCommand SaveChangesCommand Property
        private RelayCommand<ValidatableEntity> _SaveChangesCommand;
        /// <summary>
        /// Gets the SaveChangesCommand.
        /// </summary>
        public RelayCommand<ValidatableEntity> SaveChangesCommand {
            get {
                return _SaveChangesCommand
                       ?? (_SaveChangesCommand = new RelayCommand<ValidatableEntity>(OnSaveChangesCommand));
            }
        }
        #endregion

        #region RelayCommand DeleteCommand Property
        private RelayCommand<ValidatableEntity> _DeleteCommand;
        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand<ValidatableEntity> DeleteCommand {
            get {
                return _DeleteCommand
                       ?? (_DeleteCommand = new RelayCommand<ValidatableEntity>(OnDeleteCommand));
            }
        }
        #endregion

        protected DialogViewModelBase(ILifetimeScope lifetimeScope) {
            Autofac = lifetimeScope;
        }
        protected virtual async void OnSaveChangesCommand(ValidatableEntity entity) {
            DisableForm();
            Debug.WriteLine(" === DialogViewModelBase_OnSaveChangesCommand");

            int taskResult;
            entity.Updated = DateTime.Now;

            using (var repo = Autofac.Resolve<ILinkRepositoryAsync>()) {
                if (entity.Id > 0) {
                    // Id already assigned, need to update.
                    Debug.WriteLine("    Updating entry ...");
                    taskResult = await repo.UpdateAsync(entity);
                } else {
                    //This is a simple add since we don't have it in db.
                    Debug.WriteLine("    Creating new entry ...");
                    entity.Created = DateTime.Now;
                    taskResult = await repo.CreateAsync(entity);
                    UpdateUi(GuiAction.Add);
                }
            }

            Debug.WriteLine("    Task result: " + taskResult);
            ValidateForm();
        }

        protected async void OnDeleteCommand(ValidatableEntity entity) {
            DisableForm();
            Debug.WriteLine(" === DialogViewModelBase_OnDeleteCommand");

            int taskResult;
            using (var repo = Autofac.Resolve<ILinkRepositoryAsync>()) {
                //This is a simple add since we don't have it in db.
                Debug.WriteLine("    Deleting entry {0} ...", entity.Id);
                taskResult = await repo.DeleteAsync(entity);

                if (taskResult > 0)
                    UpdateUi(GuiAction.Remove);
            }

            Debug.WriteLine("    Task result: " + taskResult);
            ValidateForm();
        }

        protected void SetProgress(int value, string msg = null) {
            ProgressValue = value;
            if (!String.IsNullOrEmpty(msg)) StatusText = msg;
        }

        protected void DisableForm() {
            CanDelete = false;
            CanSaveChanges = false;
            CanCreateNew = false;
        }

        protected void OnIsDirtyCommand() {
            ValidateForm();
        }


        protected abstract void ValidateForm();
        protected abstract void OnCreateNewCommand();
        protected abstract void UpdateUi(GuiAction action = GuiAction.Add);

    }
}