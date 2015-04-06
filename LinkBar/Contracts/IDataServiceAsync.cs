using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Threading.Tasks;
using LinkBar.Model;
using UtilityLib.Mvvm;

namespace LinkBar.Contracts {
    public interface IDataServiceAsync : IDisposable {
        DbSet<Category> Categories { get; set; }
        DbSet<Link> Links { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Note> Notes { get; set; }


        Task<int> SaveChangesAsync(); 

        DbEntityEntry Entry(object obj);
    }
}