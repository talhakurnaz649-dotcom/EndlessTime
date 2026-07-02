using System;
using System.Threading.Tasks;
using EndlessTime.Model.Entities; 

namespace EndlessTime.Data.Repositories.Abstract;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<Order> Orders { get; }
    IGenericRepository<Log> Logs { get; }
    IGenericRepository<User> Users { get; }
    IGenericRepository<ContactMessage> ContactMessages { get; }
    Task<int> SaveAsync();
}