using System.Collections.ObjectModel;
using LinkBar.Model;

namespace LinkBar.Messages.Response {
    public class TagListResponse {
        public ObservableCollection<Tag> List { get; set; }
    }
}