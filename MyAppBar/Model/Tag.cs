using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.ServiceLocation;

namespace MyAppBar.Model {
    public class Tag : EntityBase {
        #region Value Binding
        private string _Value;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Tag value must not be empty!")]
        [StringLength(36, ErrorMessage = "Maximum allowed characters: 32")]
        public string Value {
            get { return _Value; }
            set {
                if (_Value == value)
                    return;

                _Value = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public virtual List<Link> LinkList { get; set; }
        public virtual List<Note> NoteList { get; set; }

        public Tag() {
            LinkList = new List<Link>();
            NoteList = new List<Note>();
        }

        internal Tag Clone() {
            var tag = ServiceLocator.Current.GetInstance<Tag>();
            tag.Id = Id;
            tag.Value = Value;

            return tag;
        }
    }
}