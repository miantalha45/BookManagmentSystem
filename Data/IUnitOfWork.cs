using BookManagmentSystem.Models;
using System;
using System.Threading.Tasks;

namespace BookManagmentSystem.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Books> Books { get; }
        Task<int> SaveChangesAsync();
    }
}