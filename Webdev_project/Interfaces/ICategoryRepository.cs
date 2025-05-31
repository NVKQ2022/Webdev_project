namespace Webdev_project.Interfaces
{
    public interface ICategoryRepository
    {
        Task InsertCategoryAsync(string categoryName);
    }
}
