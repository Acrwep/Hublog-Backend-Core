using Hublog.Repository.Entities.Model.Productivity;

namespace Hublog.Service.Interface
{
    public interface IProductivityService
    {
        Task<List<CategoryModel>> GetCategoryProductivity(string categoryName);

        Task<bool> UpdateProductivityId(int categoryId, int? productivityId);   
    }
}
