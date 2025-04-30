using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Service.Services
{
    public class ProductivityService : IProductivityService
    {
        private readonly IProductivityRepository _productivityRepository;
        public ProductivityService(IProductivityRepository productivityRepository)
        {
            _productivityRepository = productivityRepository;
        }
        public async Task<List<CategoryModel>> GetCategoryProductivity(string categoryName, int organizationId)
        {
            return await _productivityRepository.GetCategoryProductivity(categoryName, organizationId);
        }

        public async Task<bool> UpdateProductivityId(int categoryId, int? productivityId)
        {
            var rowsAffected = await _productivityRepository.UpdateProductivityId(categoryId, productivityId);
            return rowsAffected > 0;
        }
        public async Task<List<MappingModel>> GetImbuildAppsAndUrls(int OrganizationId,string userSearchQuery, string type, string category)
        {
            return await _productivityRepository.GetImbuildAppsAndUrls(OrganizationId,userSearchQuery, type, category); 
        }
        public async Task<List<MappingModel>> GetByIdImbuildAppsAndUrls(int id)
        {
            return await _productivityRepository.GetByIdImbuildAppsAndUrls(id);
        }
        public async Task<bool> InsertImbuildAppsAndUrls(int id, MappingModel model)
        {
            return await _productivityRepository.InsertImbuildAppsAndUrls(id, model);
        }
        public async Task<bool> AddImbuildAppsAndUrls(MappingModel mappingModel)
        {
            return await _productivityRepository.AddImbuildAppsAndUrls(mappingModel);
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            return await _productivityRepository.DeleteByIdAsync(id);
        }

        public async Task<ProductivityDurations> GetProductivityDurations(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate) 
        {
            return await _productivityRepository.GetProductivityDurations( organizationId,  teamId, userId, fromDate, toDate);
        }
        public async Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        { 
            return await _productivityRepository.GetAppUsages( organizationId,  teamId, userId, fromDate, toDate);
        }
        public async Task<List<TeamProductivity>> TeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return await _productivityRepository.TeamwiseProductivity(organizationId, teamId,fromDate, toDate);
        }

        public async Task<dynamic> MostTeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return await _productivityRepository.MostTeamwiseProductivity(organizationId, teamId, fromDate, toDate);
        }
        public async Task<dynamic> GetTotal_Working_Time(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return await _productivityRepository.GetTotal_Working_Time(organizationId, teamId, userId, fromDate, toDate);
        }
        public async Task<dynamic> GetProductivity_Trend(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return await _productivityRepository.GetProductivity_Trend(organizationId, teamId, userId, fromDate, toDate);
        }
        public async Task<dynamic> GetEmployeeList(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return await _productivityRepository.GetEmployeeList(organizationId, teamId, userId, fromDate, toDate);
        }

        public async Task InsertDefaultCategoryRecordsAsync(int organizationId)
        {
            try
            {
                await _productivityRepository.InsertDefaultCategoryRecordsAsync(organizationId);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task InsertDefaultRecordsAsync(int organizationId)
        {
            try
            {
                await _productivityRepository.InsertDefaultRecordsAsync(organizationId);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }

        public async Task<dynamic> GetCategoryUsagePercentage(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            return await _productivityRepository.GetCategoryUsagePercentage(organizationId, teamId, userId, fromDate, toDate);
        }
    }

}
