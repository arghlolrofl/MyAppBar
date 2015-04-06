using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyAppBar.Model;

namespace MyAppBar.Contracts {
    public interface IDataService : IDisposable {
        DbSet<Category> Categories { get; set; }
        DbSet<Link> Links { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Note> Notes { get; set; }

        List<Category> GetCategoriesAsync(Action waitCallback = null);
        List<Tag> GetTagsAsync(Action waitCallback = null);

        Task<int> SaveAsync<T>(T entity) where T : EntityBase;
        Task<int> SaveAsync<T>(IEnumerable<T> entity) where T : EntityBase;
        Task<int> DeleteAsync<T>(T entity) where T : EntityBase;


    }
}
