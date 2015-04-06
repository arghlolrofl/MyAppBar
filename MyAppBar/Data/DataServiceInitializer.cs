using MyAppBar.Model;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace MyAppBar.Data {
    public class DataServiceInitializer : DropCreateDatabaseIfModelChanges<DataService> {
        protected override void Seed(DataService context) {
            //context.Categories.AddOrUpdate(
            //  c => c.Name,
            //  new Category() { Name = ".NET", Description = "Everything concerning .NET-Framework", Created = DateTime.Now, Updated = DateTime.Now },
            //  new Category() { Name = "Science", Description = "Science and science news websites", Created = DateTime.Now, Updated = DateTime.Now },
            //  new Category() { Name = "News", Description = "News websites", Created = DateTime.Now, Updated = DateTime.Now },
            //  new Category() { Name = "Media", Description = "Interesting videos or animations", Created = DateTime.Now, Updated = DateTime.Now },
            //  new Category() { Name = "Games", Description = "Walkthroughs, Guides, Forums, Portals, etc", Created = DateTime.Now, Updated = DateTime.Now }
            //);

            //context.Tags.AddOrUpdate(
            //    t => t.Value,
            //    new Tag() { Value = "C#", Created = DateTime.Now, Updated = DateTime.Now },
            //    new Tag() { Value = "WPF", Created = DateTime.Now, Updated = DateTime.Now },
            //    new Tag() { Value = "Physics", Created = DateTime.Now, Updated = DateTime.Now },
            //    new Tag() { Value = "Astronomy", Created = DateTime.Now, Updated = DateTime.Now },
            //    new Tag() { Value = "IT", Created = DateTime.Now, Updated = DateTime.Now },
            //    new Tag() { Value = "News", Created = DateTime.Now, Updated = DateTime.Now },
            //    new Tag() { Value = "Entity Framework", Created = DateTime.Now, Updated = DateTime.Now },
            //    new Tag() { Value = "Science", Created = DateTime.Now, Updated = DateTime.Now }
            //);

            base.Seed(context);
        }
    }
}
