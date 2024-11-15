using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Interface;
using System;

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
        public async Task<List<MappingModel>> GetImbuildAppsAndUrls()
        {
            //var query = "SELECT I.Type, I.Name " +
            //            "FROM ImbuildAppsAndUrls I " +
            //            "LEFT JOIN Categories C ON C.ProductivityId = I.CategoryId";

            // You can include the filtering logic here
            var query = "Select * From ImbuildAppsAndUrls";
            return await _dapper.GetAllAsync<MappingModel>(query);
        }
        public async Task<List<MappingModel>> GetByIdImbuildAppsAndUrls(int id)
        {
            var query = @"
        SELECT [id], [type], [name], [categoryid]
        FROM [EMP6].[dbo].[ImbuildAppsAndUrls] 
        WHERE [id] = @Id";

            var parameters = new { Id = id };
            var result = await _dapper.GetAllAsync<MappingModel>(query, parameters);

            return result;
        }

        public async Task<bool> InsertImbuildAppsAndUrls(int id, MappingModel model)
        {
            var query = @"
                UPDATE [EMP4].[dbo].[ImbuildAppsAndUrls]
                SET [CategoryId] = @NewCategoryId
                WHERE [id] = @Id";

            var parameters = new { Id = id, NewCategoryId = model.CategoryId };
            var affectedRows = await _dapper.ExecuteAsync(query, parameters);

            return affectedRows > 0;
        }
    }
}
