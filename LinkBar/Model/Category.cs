using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.ServiceLocation;
using UtilityLib.Mvvm;

namespace LinkBar.Model {
    public class Category : ValidatableEntity {
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
            return new Category {
                Id = Id, Name = Name, Description = Description
            };
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