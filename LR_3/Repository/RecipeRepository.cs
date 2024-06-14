using AutoMapper;
using LR_3.Data;
using LR_3.Models;
using LR_3.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace LR_3.Repository
{
    public class RecipeRepository : Repository<Recipe>, IRecipeRepository
    {
        private readonly ApplicationDbContext _db;
        public RecipeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Recipe> UpdateAsync(Recipe entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Recipes.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

       
    }
}
