using Webdev_project.Models;

namespace Webdev_project.Interfaces
{
    public interface ICategoryRepository
    {
        Task InsertCategoryAsync(string categoryName);
        Task<List<Category>> GetCategoriesSortedByBuyTimeAsync();
    }
}
