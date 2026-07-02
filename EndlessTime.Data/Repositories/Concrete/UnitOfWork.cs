using EndlessTime.Data.Repositories.Abstract;
using EndlessTime.Model.Entities;
using System;
using System.Threading.Tasks;

namespace EndlessTime.Data.Repositories.Concrete;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IProductRepository? _products;
    private IGenericRepository<Category>? _categories;
    private IGenericRepository<Order>? _orders;
    private IGenericRepository<Log>? _logs;
    private IGenericRepository<User>? _users;
    private IGenericRepository<ContactMessage>? _contactMessages; 

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IGenericRepository<Category> Categories => _categories ??= new GenericRepository<Category>(_context);
    public IGenericRepository<Order> Orders => _orders ??= new GenericRepository<Order>(_context);
    public IGenericRepository<Log> Logs => _logs ??= new GenericRepository<Log>(_context);
    public IGenericRepository<User> Users => _users ??= new GenericRepository<User>(_context);
    public IGenericRepository<ContactMessage> ContactMessages => _contactMessages ??= new GenericRepository<ContactMessage>(_context); // EKLENDİ 🚨

    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
    public void Dispose() => _context.Dispose();
}