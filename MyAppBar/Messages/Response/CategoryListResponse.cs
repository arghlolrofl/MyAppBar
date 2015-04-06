using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyAppBar.Model;

namespace MyAppBar.Messages.Response {
    public class CategoryListResponse {
        public ObservableCollection<Category> List { get; set; }
    }
}