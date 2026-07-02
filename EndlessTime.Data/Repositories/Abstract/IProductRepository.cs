using EndlessTime.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EndlessTime.Data.Repositories.Abstract;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsWithCategoriesAsync();
}