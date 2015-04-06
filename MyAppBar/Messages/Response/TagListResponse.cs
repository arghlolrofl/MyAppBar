using System.Collections.ObjectModel;
using MyAppBar.Model;

namespace MyAppBar.Messages.Response {
    public class TagListResponse {
        public ObservableCollection<Tag> List { get; set; }
    }
}