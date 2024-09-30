using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class ProductivityRepository : IProductivityRepository
    {
        private readonly Dapperr _dapper;
        public ProductivityRepository(Dapperr dapper)   
        {
            _dapper = dapper;
        }
        public async Task<List<CategoryModel>> GetCategoryProductivity(string categoryName)
        {
            var query = @"
            SELECT 
                C.Id AS CategoryId,
                C.CategoryName,
                PA.Id AS ProductivityId,
                PA.Name AS ProductivityName
            FROM 
                Categories C
            LEFT JOIN 
                ProductivityAssign PA ON C.ProductivityId = PA.Id";

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                query += " WHERE C.CategoryName LIKE @CategoryName";
                var parameters = new { CategoryName = $"%{categoryName}%" };
                return await _dapper.GetAllAsync<CategoryModel>(query, parameters);
            }

            return await _dapper.GetAllAsync<CategoryModel>(query);
        }

        public async Task<int> UpdateProductivityId(int categoryId, int? productivityId)
        {
            var query = @"
            UPDATE Categories
            SET ProductivityId = @ProductivityId
            WHERE Id = @CategoryId";

            var parameters = new { CategoryId = categoryId, ProductivityId = productivityId };

            return await _dapper.ExecuteAsync(query, parameters);
        }
    }
}
