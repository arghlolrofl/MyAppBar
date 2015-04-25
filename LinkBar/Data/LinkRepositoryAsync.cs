using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Documents.DocumentStructures;
using Autofac;
using LinkBar.Contracts;
using LinkBar.Model;
using UtilityLib.Mvvm;

namespace LinkBar.Data {
    public class LinkRepositoryAsync : ILinkRepositoryAsync {
        private readonly IDataServiceAsync _dataService;
        private readonly ILifetimeScope _autofac;

        public LinkRepositoryAsync(IDataServiceAsync dataService, ILifetimeScope lifetimeScope) {
            _dataService = dataService;
            _autofac = lifetimeScope;
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
            try {
                var link = entity as Link;
                if (link != null) {
                    _dataService.Links.Attach(link);
                }
                _dataService.Entry(entity).State = EntityState.Added;
                entity.Created = entity.Updated = DateTime.Now;


                return await _dataService.SaveChangesAsync();
            } catch (DbEntityValidationException e) {
                debugLogEntityValidationError(e);
                throw;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<int> UpdateAsync(ValidatableEntity entity) {
            try {
                _dataService.Entry(entity).State = EntityState.Modified;
                entity.Updated = DateTime.Now;

                return await _dataService.SaveChangesAsync();
            } catch (DbEntityValidationException e) {
                debugLogEntityValidationError(e);
                throw;
            } catch (Exception ex) {
                throw ex;
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

        public async Task<int> AddTagToLink(Link selectedLink, Tag selectedAvailableTag) {
            try {
                _dataService.Links.Attach(selectedLink);
                _dataService.Tags.Attach(selectedAvailableTag);

                selectedLink.Tags.Add(selectedAvailableTag);
                return await _dataService.SaveChangesAsync();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<int> RemoveTagFromLink(Link SelectedLink, Tag SelectedUsedTag) {
            try {
                _dataService.Links.Attach(SelectedLink);
                _dataService.Tags.Attach(SelectedUsedTag);

                SelectedLink.Tags.Remove(SelectedUsedTag);
                return await _dataService.SaveChangesAsync();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<int> AddNewTagsToLink(Link SelectedLink, string[] newTags) {
            try {
                List<Tag> newTagsList = new List<Tag>();

                foreach (var newTagValue in newTags) {
                    var newTag = _autofac.Resolve<Tag>();
                    newTag.Value = newTagValue;

                    var result = CreateAsync(newTag);
                    Debug.WriteLine("Tag created: " + newTag.Value + "        Result: " + result);

                    newTagsList.Add(newTag);
                }

                _dataService.Links.Attach(SelectedLink);

                foreach (var tag in newTagsList)
                    SelectedLink.Tags.Add(tag);

                return await _dataService.SaveChangesAsync();
            } catch (Exception ex) {
                throw ex;
            }
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