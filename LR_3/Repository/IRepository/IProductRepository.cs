using LR_3.Models;
using System.Linq.Expressions;

namespace LR_3.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> UpdateAsync(Product entity);
    }
}
