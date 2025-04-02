using Hublog.Repository.Entities.Model.Productivity;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Service.Interface
{
    public interface IProductivityService
    {
        Task<List<MappingModel>> GetImbuildAppsAndUrls(int OrganizationId,string userSearchQuery, string type, string category); 
        Task<List<MappingModel>> GetByIdImbuildAppsAndUrls( int id);
        Task<bool> InsertImbuildAppsAndUrls(int id, MappingModel model);
        Task<bool> AddImbuildAppsAndUrls(MappingModel mappingModel);
        Task<List<CategoryModel>> GetCategoryProductivity(string categoryName, int organizationId);

        Task<bool> UpdateProductivityId(int categoryId, int? productivityId);
        Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate);
        Task<ProductivityDurations> GetProductivityDurations(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate);
        Task<List<TeamProductivity>> TeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);
        Task<dynamic> MostTeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);
        Task<dynamic> GetTotal_Working_Time(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);
        Task<dynamic> GetProductivity_Trend(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);
        Task<dynamic> GetEmployeeList(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);
    }
}
