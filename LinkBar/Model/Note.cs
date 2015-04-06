using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilityLib.Mvvm;

namespace LinkBar.Model {
    public class Note : ValidatableEntity {
        [StringLength(256)]
        public string Header { get; set; }
        [StringLength(1024)]
        public string Content { get; set; }

        public virtual List<Tag> Tags { get; set; }

        [ForeignKey("AssociatedLink")]
        public Int64? AssociatedLinkId { get; set; }
        public virtual Link AssociatedLink { get; set; }

        public Note() {
            Tags = new List<Tag>();
        }

    }
}