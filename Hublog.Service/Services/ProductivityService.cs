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
    }
}
