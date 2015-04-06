using MyAppBar.Model;

namespace MyAppBar.Migrations {
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Data.DataService> {
        public Configuration() {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Data.DataService context) {
            //  This method will be called after migrating to the latest version.

            //You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //to avoid creating duplicate seed data. E.g.

            //context.Categories.AddOrUpdate(
            //  c => c.Name,
            //  new Category() { Name = ".NET", Description = "Everything concerning .NET-Framework", Created = DateTime.Now, Updated = DateTime.Now },
            //  new Category() { Name = "Science", Description = "Science and science news websites", Created = DateTime.Now, Updated = DateTime.Now }
            //);
        }
    }
}
