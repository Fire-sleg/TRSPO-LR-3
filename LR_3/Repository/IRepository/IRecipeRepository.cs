using LR_3.Models;
using System.Linq.Expressions;

namespace LR_3.Repository.IRepository
{
    public interface IRecipeRepository : IRepository<Recipe>
    {   
        Task<Recipe> UpdateAsync(Recipe entity);
    }
}
