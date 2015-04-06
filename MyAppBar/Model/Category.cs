using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Documents;
using System.Data.Entity;
using Microsoft.Practices.ServiceLocation;

namespace MyAppBar.Model {
    public class Category : EntityBase {
        private string _Name = null;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name must not be empty!")]
        [StringLength(256, ErrorMessage = "Maximum allowed characters: 256")]
        public string Name {
            get { return _Name; }
            set {
                if (_Name == value)
                    return;

                _Name = value;
                RaisePropertyChanged();
            }
        }
        
        #region Description Binding
        private string _Description;
        [StringLength(512, ErrorMessage = "Maximum allowed characters: 512")]
        public string Description {
            get { return _Description; }
            set {
                if (_Description == value)
                    return;

                _Description = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public virtual ObservableCollection<Link> Links { get; set; }

        public Category() {
            Links = new ObservableCollection<Link>();
        }

        public Category Clone() {
            var c = ServiceLocator.Current.GetInstance<Category>();

            c.Id = Id;
            c.Name = Name;
            c.Description = Description;

            return c;
        }

        public override bool Equals(object obj) {
            var c = obj as Category;
            if (c == null)
                return false;

            if (c.Name != Name)
                return false;

            return c.Description == Description;
        }

    }
}