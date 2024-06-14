using AutoMapper;
using LR_3.Data;
using LR_3.Models;
using LR_3.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace LR_3.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Product> UpdateAsync(Product entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Products.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

       
    }
}
