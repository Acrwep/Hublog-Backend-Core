using Hublog.Repository.Entities.Model.Productivity;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Repository.Interface
{
    public interface IProductivityRepository
    {
        Task<List<CategoryModel>> GetCategoryProductivity(string categoryName);

        Task<int> UpdateProductivityId(int categoryId, int? productivityId);
        Task<List<MappingModel>> GetImbuildAppsAndUrls();
        Task<List<MappingModel>> GetByIdImbuildAppsAndUrls(int id);
        Task<bool> InsertImbuildAppsAndUrls(int id, [FromBody] MappingModel model);
        Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate);
        Task<ProductivityDurations> GetProductivityDurations(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate);
        Task<List<TeamProductivity>> TeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);
    }
}
