using Microsoft.EntityFrameworkCore;
using EndlessTime.Model.Entities;
using EndlessTime.Data.Repositories.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EndlessTime.Data.Repositories.Concrete;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetProductsWithCategoriesAsync()
    {
        return await _context.Products.Include(p => p.Category).ToListAsync();
    }
}