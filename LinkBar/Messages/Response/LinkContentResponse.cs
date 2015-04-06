using System.Collections.Generic;
using System.Collections.ObjectModel;
using LinkBar.Model;

namespace LinkBar.Messages.Response {
    public class LinkContentResponse {
        public IEnumerable<Tag> Tags { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
    }
}
