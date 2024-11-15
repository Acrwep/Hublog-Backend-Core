using Hublog.Repository.Entities.Model.Productivity;

namespace Hublog.Service.Interface
{
    public interface IProductivityService
    {
        Task<List<MappingModel>> GetImbuildAppsAndUrls(); 
        Task<List<MappingModel>> GetByIdImbuildAppsAndUrls( int id);
        Task<bool> InsertImbuildAppsAndUrls(int id, MappingModel model);
        Task<List<CategoryModel>> GetCategoryProductivity(string categoryName);

        Task<bool> UpdateProductivityId(int categoryId, int? productivityId);   
    }
}
