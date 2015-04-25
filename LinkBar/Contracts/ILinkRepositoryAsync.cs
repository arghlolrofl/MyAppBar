using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using LinkBar.Model;
using UtilityLib.Mvvm;

namespace LinkBar.Contracts {
    public interface ILinkRepositoryAsync : IDisposable {
        Task<Category> FetchCategory(int primaryKey);
        Task<Tag> FetchTag(int primaryKey);
        Task<Link> FetchLink(int primaryKey);

        Task<List<Category>>  FetchAllCategories();
        Task<List<Tag>> FetchAllTags();

        Task<int> UpdateAsync(ValidatableEntity entity);
        Task<int> CreateAsync(ValidatableEntity entity);
        Task<int> DeleteAsync(ValidatableEntity entity);

        Task<int> AddTagToLink(Link selectedLink, Tag selectedAvailableTag);
        Task<int> RemoveTagFromLink(Link selectedLink, Tag selectedUsedTag);

        Task<int> AddNewTagsToLink(Link selectedLink, string[] newTags);

    }
}