using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyAppBar.Model {
    public abstract class EntityBase : INotifyPropertyChanged, INotifyDataErrorInfo {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
            ValidateAsync();
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        protected void RaiseErrorsChanged(string propertyName) {
            var handler = ErrorsChanged;
            if (handler != null)
                handler(this, new DataErrorsChangedEventArgs(propertyName));
        }   
        
        private object _lock = new object();
        private ConcurrentDictionary<string, List<string>> _errors = new ConcurrentDictionary<string, List<string>>();

        #region Id Binding
        private Int64 _Id = 0;
        /// <summary>
        /// Sets and gets the Categories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Int64 Id {
            get { return _Id; }
            set {
                if (_Id == value)
                    return;

                _Id = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        [NotMapped]
        public bool HasErrors {
            get {
                var hasErrors = _errors.Any(
                    kv => kv.Value != null && kv.Value.Count > 0
                );

                return hasErrors;
            }
        }

        public IEnumerable GetErrors(string propertyName) {
            List<string> errorsForName;
            _errors.TryGetValue(propertyName, out errorsForName);
            return errorsForName;
        }

        protected Task ValidateAsync() {
            return Task.Run(() => Validate());
        }

        protected void Validate() {
            lock (_lock) {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                foreach (var kv in _errors) {
                    if (validationResults.All(r => r.MemberNames.All(m => m != kv.Key))) {
                        List<string> outLi;
                        if (_errors.TryRemove(kv.Key, out outLi))
                            RaiseErrorsChanged(kv.Key);
                    }
                }

                var q = from r in validationResults
                        from m in r.MemberNames
                        group r by m into g
                        select g;

                foreach (var prop in q) {
                    var messages = prop.Select(r => r.ErrorMessage).ToList();

                    if (_errors.ContainsKey(prop.Key)) {
                        List<string> outLi;
                        _errors.TryRemove(prop.Key, out outLi);
                    }
                    if (_errors.TryAdd(prop.Key, messages))
                        RaiseErrorsChanged(prop.Key);
                }
            }
        }
    }
}