using BookManagmentSystem.Data;
using BookManagmentSystem.Data.Repositories;
using BookManagmentSystem.Models;
using System.Threading.Tasks;

namespace BookManagmentSystem.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookDbContext _context;
        private IGenericRepository<Books>? _books;

        public UnitOfWork(BookDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Books> Books => _books ??= new GenericRepository<Books>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
