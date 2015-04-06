using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.Utilities;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using LinkBar.Contracts;
using LinkBar.Model;
using LinkBar.Properties;
using UtilityLib.Mvvm;

namespace LinkBar.Data {
    public class DataService : DbContext, IDataServiceAsync {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Note> Notes { get; set; }

        public new DbEntityEntry Entry(object obj) {
            return base.Entry(obj);
        }

        private const string ConStringTemplate = "Data Source={0}";
        private static readonly string ConnectionString;
        static DataService() {
            Database.SetInitializer(new DataServiceInitializer());
            var dbFilePath = Path.Combine(
                Settings.Default.DatabaseFolder,
                Settings.Default.DatabaseName + Settings.Default.DatabaseExtension
            );

            ConnectionString = String.Format(ConStringTemplate, dbFilePath);
        }

        #region Database initialization
        public DataService()
            : base(ConnectionString) {
            Database.Initialize(false);
        }
        public DataService(FileInfo databaseFile)
            : base(String.Format(ConStringTemplate, databaseFile.FullName)) {
            Database.Initialize(false);
        }
        public DataService(string databaseFilePath)
            : base(String.Format(ConStringTemplate, databaseFilePath)) {
            Database.Initialize(false);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            // Fluent configuration of relations
            base.OnModelCreating(modelBuilder);
        }
        #endregion

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }
}