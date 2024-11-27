using Hublog.Repository.Entities.Model.Productivity;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Service.Interface
{
    public interface IProductivityService
    {
        Task<List<MappingModel>> GetImbuildAppsAndUrls(); 
        Task<List<MappingModel>> GetByIdImbuildAppsAndUrls( int id);
        Task<bool> InsertImbuildAppsAndUrls(int id, MappingModel model);
        Task<List<CategoryModel>> GetCategoryProductivity(string categoryName);

        Task<bool> UpdateProductivityId(int categoryId, int? productivityId);
        Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate);
        Task<ProductivityDurations> GetProductivityDurations(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate);
        Task<List<TeamProductivity>> TeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);
        Task<dynamic> MostTeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);
    }
}
