using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using LinkBar.Messages.Request;
using Microsoft.Practices.ServiceLocation;
using UtilityLib.Mvvm;

namespace LinkBar.Model {
    public class Tag : ValidatableEntity {
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

        public Tag Clone() {
            return new Tag {
                Id = Id, Value = Value
            };
        }

        public override bool Equals(object obj) {
            var tag = obj as Tag;
            if (tag == null) return false;

            return Id == tag.Id && Value == tag.Value;
        }
    }
}