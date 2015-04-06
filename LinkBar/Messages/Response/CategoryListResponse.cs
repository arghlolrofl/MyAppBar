using System.Collections.ObjectModel;
using LinkBar.Model;

namespace LinkBar.Messages.Response {
    public class CategoryListResponse {
        public ObservableCollection<Category> List { get; set; }
    }
}