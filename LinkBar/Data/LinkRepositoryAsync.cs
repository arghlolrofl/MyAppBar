using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkBar.Contracts;
using LinkBar.Model;
using UtilityLib.Mvvm;

namespace LinkBar.Data {
    public class LinkRepositoryAsync : ILinkRepositoryAsync {
        private readonly IDataServiceAsync _dataService;

        public LinkRepositoryAsync(IDataServiceAsync dataService) {
            _dataService = dataService;
        }

        public async Task<Category> FetchCategory(int primaryKey) {
            return await _dataService.Categories.FindAsync(primaryKey);
        }

        public async Task<Tag> FetchTag(int primaryKey) {
            return await _dataService.Tags.FindAsync(primaryKey);
        }

        public async Task<Link> FetchLink(int primaryKey) {
            return await _dataService.Links.FindAsync(primaryKey);
        }

        public async Task<List<Category>> FetchAllCategories() {
            var query = from cat in _dataService.Categories.Include(
                            c => c.Links.Select(l => l.Tags)
                        )
                        orderby cat.Name
                        select cat;

            return await query.ToListAsync();
        }

        public async Task<List<Tag>> FetchAllTags() {
            var query = from tag in _dataService.Tags
                        orderby tag.Value
                        select tag;

            return await query.ToListAsync();
        }

        public async Task<int> CreateAsync(ValidatableEntity entity) {
            _dataService.Entry(entity).State = EntityState.Added;

            try {
                return await _dataService.SaveChangesAsync();
            } catch (DbEntityValidationException e) {
                debugLogEntityValidationError(e);
                throw;
            }
        }

        public async Task<int> UpdateAsync(ValidatableEntity entity) {
            _dataService.Entry(entity).State = EntityState.Modified;

            try {
                return await _dataService.SaveChangesAsync();
            } catch (DbEntityValidationException e) {
                debugLogEntityValidationError(e);
                throw;
            }
        }

        public async Task<int> DeleteAsync(ValidatableEntity entity) {
            _dataService.Entry(entity).State = EntityState.Deleted;

            try {
                return await _dataService.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine("LinkRepositoryAsync Exception while deleting entity!");
                Debug.WriteLine("Exception: " + ex.GetType().Name);
                Debug.WriteLine("Message: " + ex.Message);
                throw;
            }
        }

        public Task<int> AddTagToLink(Link selectedLink, Tag selectedAvailableTag) {
            throw new NotImplementedException();
        }

        public Task<int> RemoveTagFromLink(Link SelectedLink, Tag SelectedUsedTag) {
            throw new NotImplementedException();
        }

        public IEnumerable<Tag> AddNewTagsToLink(Link SelectedLink, string[] newTags) {
            throw new NotImplementedException();
        }

        private void debugLogEntityValidationError(DbEntityValidationException e) {
            foreach (var eve in e.EntityValidationErrors) {
                Debug.WriteLine(
                    "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State
                );
                foreach (var ve in eve.ValidationErrors) {
                    Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                }
            }
        }

        public void Dispose() {
            _dataService.Dispose();
        }
    }
}