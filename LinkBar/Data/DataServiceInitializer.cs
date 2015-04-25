using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using LinkBar.Model;

namespace LinkBar.Data {
    public class DataServiceInitializer : DropCreateDatabaseIfModelChanges<DataService> {
        protected override void Seed(DataService context) {
            context.Categories.AddOrUpdate(
              c => c.Name,
              new Category() { Name = "CodeProject", Description = "Interesting CodeProject articles.", Created = DateTime.Now, Updated = DateTime.Now },
              new Category() { Name = "Projects", Description = "Coding projects", Created = DateTime.Now, Updated = DateTime.Now },
              new Category() { Name = "Articles", Description = "Blog entries, articles and more.", Created = DateTime.Now, Updated = DateTime.Now },
              new Category() { Name = "Science", Description = "Science and science news websites", Created = DateTime.Now, Updated = DateTime.Now },
              new Category() { Name = "News", Description = "News websites", Created = DateTime.Now, Updated = DateTime.Now },
              new Category() { Name = "Media", Description = "Interesting videos or animations", Created = DateTime.Now, Updated = DateTime.Now },
              new Category() { Name = "Games", Description = "Walkthroughs, Guides, Forums, Portals, etc", Created = DateTime.Now, Updated = DateTime.Now }
            );

            context.Tags.AddOrUpdate(
                t => t.Value,
                new Tag() { Value = "C#", Created = DateTime.Now, Updated = DateTime.Now },
                new Tag() { Value = "Design", Created = DateTime.Now, Updated = DateTime.Now },
                new Tag() { Value = ".NET", Created = DateTime.Now, Updated = DateTime.Now },
                new Tag() { Value = "WPF", Created = DateTime.Now, Updated = DateTime.Now },
                new Tag() { Value = "Physics", Created = DateTime.Now, Updated = DateTime.Now },
                new Tag() { Value = "Astronomy", Created = DateTime.Now, Updated = DateTime.Now },
                new Tag() { Value = "IT", Created = DateTime.Now, Updated = DateTime.Now },
                new Tag() { Value = "News", Created = DateTime.Now, Updated = DateTime.Now },
                new Tag() { Value = "Entity Framework", Created = DateTime.Now, Updated = DateTime.Now },
                new Tag() { Value = "Science", Created = DateTime.Now, Updated = DateTime.Now }
            );

            base.Seed(context);
        }
    }
}
