using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MyAppBar.Contracts;
using MyAppBar.Model;
using System.IO;

namespace MyAppBar.Data {
    public class DataService : DbContext, IDataService {
        private static readonly string ConnectionString;
        static DataService() {
            Database.SetInitializer(new DataServiceInitializer());
            var dbFilePath = Path.Combine(
                Properties.Settings.Default.DatabaseFolder,
                Properties.Settings.Default.DatabaseName + Properties.Settings.Default.DatabaseExtension
            );

            ConnectionString = String.Format(@"Data Source={0}", dbFilePath);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Note> Notes { get; set; }

        #region Database initialization
        public DataService() : base(ConnectionString) {
            Database.Initialize(false);
        }

        public DataService(string conString) : base(conString) {
            Database.Initialize(false);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            // Fluent configuration of relations
            base.OnModelCreating(modelBuilder);
        }
        #endregion


        public async Task<int> SaveAsync<T>(T entity) where T : EntityBase {
            if (entity.Id > 0) {// Id already assigned, need to update.
                Entry(entity).State = EntityState.Modified;
            } else {
                //This is a simple add since we don't have it in db.
                entity.Created = DateTime.Now;
                Entry(entity).State = EntityState.Added;
            }
            entity.Updated = DateTime.Now;

            try {
                return await SaveChangesAsync();
            } catch (DbEntityValidationException e) {
                foreach (var eve in e.EntityValidationErrors) {
                    Debug.WriteLine(
                        "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State
                    );
                    foreach (var ve in eve.ValidationErrors) {
                        Debug.WriteLine(
                            "- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage
                        );
                    }
                }
                throw;
            }
        }

        public async Task<int> SaveAsync<T>(IEnumerable<T> entities) where T : EntityBase {
            foreach (var entity in entities) {
                var task = SaveAsync(entity);
                task.Wait();
            }
            
            return await SaveChangesAsync();
        }

        public async Task<int> DeleteAsync<T>(T entity) where T : EntityBase {
            Entry(entity).State = EntityState.Deleted;
            return await SaveChangesAsync();
        }


        private async Task<List<Category>> getCategoriesAsync() {
            return await (Categories.Include(c => c.Links.Select(l => l.Tags)).ToListAsync());
        }
        public List<Category> GetCategoriesAsync(Action waitCallback = null) {
            var task = getCategoriesAsync();

            if (waitCallback != null) waitCallback();

            task.Wait();
            return task.Result;
        }

        private async Task<List<Tag>> getTagsAsync() {
            return await (Tags.ToListAsync());
        }
        public List<Tag> GetTagsAsync(Action waitCallback = null) {
            var task = getTagsAsync();

            if (waitCallback != null) waitCallback();

            task.Wait();
            return task.Result;
        }
    }
}