using Hublog.Repository.Entities.Model.Productivity;

namespace Hublog.Repository.Interface
{
    public interface IProductivityRepository
    {
        Task<List<CategoryModel>> GetCategoryProductivity(string categoryName);

        Task<int> UpdateProductivityId(int categoryId, int? productivityId);    
    }
}
