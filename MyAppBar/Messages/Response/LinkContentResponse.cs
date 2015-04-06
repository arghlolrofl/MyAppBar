using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyAppBar.Model;

namespace MyAppBar.Messages.Response {
    public class LinkContentResponse {
        public IEnumerable<Tag> Tags { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
    }
}
