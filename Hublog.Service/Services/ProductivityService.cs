using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class ProductivityService : IProductivityService
    {
        private readonly IProductivityRepository _productivityRepository;
        public ProductivityService(IProductivityRepository productivityRepository)
        {
            _productivityRepository = productivityRepository;
        }
        public async Task<List<CategoryModel>> GetCategoryProductivity(string categoryName)
        {
            return await _productivityRepository.GetCategoryProductivity(categoryName);
        }

        public async Task<bool> UpdateProductivityId(int categoryId, int? productivityId)
        {
            var rowsAffected = await _productivityRepository.UpdateProductivityId(categoryId, productivityId);
            return rowsAffected > 0;
        }
        public async Task<List<MappingModel>> GetImbuildAppsAndUrls()
        {
            return await _productivityRepository.GetImbuildAppsAndUrls(); 
        }
        public async Task<List<MappingModel>> GetByIdImbuildAppsAndUrls(int id)
        {
            return await _productivityRepository.GetByIdImbuildAppsAndUrls(id);
        }
        public async Task<bool> InsertImbuildAppsAndUrls(int id, MappingModel model)
        {
            return await _productivityRepository.InsertImbuildAppsAndUrls(id, model);
        }
        public async Task<ProductivityDurations> GetProductivityDurations(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate) 
        {
            return await _productivityRepository.GetProductivityDurations( organizationId,  teamId, userId, fromDate, toDate);
        }
        public async Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        { 
            return await _productivityRepository.GetAppUsages( organizationId,  teamId, userId, fromDate, toDate);
        }
    }
}
