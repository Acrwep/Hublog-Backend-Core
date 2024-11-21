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
        public async Task<List<AppUsage>> GetAppUsages(int userId, DateTime fromDate, DateTime toDate)
        {
            var appUsageQuery = @"
            SELECT UserId, ApplicationName, Details, SUM(DATEDIFF(SECOND, '00:00:00', TotalUsage)) AS TotalSeconds, UsageDate
            FROM ApplicationUsage
            WHERE UserId = @UserId AND UsageDate BETWEEN @FromDate AND @ToDate
            GROUP BY UserId, ApplicationName, Details, UsageDate";

            var urlUsageQuery = @"
          SELECT UserId, Url AS ApplicationName, Details, SUM(DATEDIFF(SECOND, '00:00:00', TotalUsage)) AS TotalSeconds, UsageDate
           FROM UrlUsage 
           WHERE UserId = @UserId AND UsageDate BETWEEN @FromDate AND @ToDate 
           GROUP BY UserId, Url, Details, UsageDate";

            var parameters = new { UserId = userId, FromDate = fromDate, ToDate = toDate };
            // Fetch data from both tables
            var appUsages = await _dapper.GetAllAsync<AppUsage>(appUsageQuery, parameters);
            var urlUsages = await _dapper.GetAllAsync<AppUsage>(urlUsageQuery, parameters);
            // Merge both lists
            var allUsages = appUsages.Concat(urlUsages).ToList();

            // Return the merged list return allUsages;
            return allUsages;
            //var urlUsageQuery1 = @"select * imImbuildAppsAndUrls where name Like '' ",;


        }
        public async Task<ProductivityDurations> GetProductivityDurations(int userId, DateTime fromDate, DateTime toDate)
        {
            var usages = await GetAppUsages(userId, fromDate, toDate);

            // Calculate TotalUsage for each ApplicationName
            var totalUsages = usages
                .GroupBy(u => u.ApplicationName)
                .Select(g => new { ApplicationName = g.Key, TotalSeconds = g.Sum(u => u.TotalSeconds) })
                .ToDictionary(t => t.ApplicationName, t => t.TotalSeconds);

            // Initialize duration variables
            int totalProductiveDuration = 0;
            int totalUnproductiveDuration = 0;
            int totalNeutralDuration = 0;



            // Check against ImbuildAppsAndUrls and Categories tables
            foreach (var usage in usages)
            {
                usage.ApplicationName = usage.ApplicationName.ToLower();

                if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                {
                    if (totalUsages.TryGetValue(usage.ApplicationName, out var totalSeconds))
                    {
                        usage.TotalSeconds = totalSeconds;
                        usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
                    }

                    // Query for category and productivity details
                    var imbuildAppQuery = @"
                        SELECT CategoryId 
                         FROM ImbuildAppsAndUrls 
                        WHERE Name LIKE '%' + @ApplicationName + '%'";
                    var categoryId = await _dapper.QueryFirstOrDefaultAsync<int?>(imbuildAppQuery, new { ApplicationName = usage.ApplicationName });

                    if (categoryId.HasValue)
                    {
                        usage.CategoryId = categoryId.Value;

                        var categoryQuery = @"
                        SELECT CategoryName, ProductivityId 
                        FROM Categories 
                         WHERE Id = @CategoryId";

                        var category = await _dapper.QueryFirstOrDefaultAsync<(string CategoryName, int ProductivityId)>(categoryQuery, new { CategoryId = categoryId.Value });

                        if (category != default)
                        {
                            usage.CategoryName = category.CategoryName;

                            // Fetch ProductivityName from ProductivityAssign
                            var productivityQuery = @"
                            SELECT Name FROM ProductivityAssign
                            WHERE Id = @ProductivityId";

                            var productivityName = await _dapper.QueryFirstOrDefaultAsync<string>(productivityQuery, new { ProductivityId = category.ProductivityId });
                            usage.ProductivityName = productivityName;

                            // Add to the corresponding duration
                            switch (usage.ProductivityName)
                            {
                                case "Productive":
                                    totalProductiveDuration += usage.TotalSeconds;
                                    break;
                                case "Unproductive":
                                    totalUnproductiveDuration += usage.TotalSeconds;
                                    break;
                                case "Neutral":
                                    totalNeutralDuration += usage.TotalSeconds;
                                    break;
                            }
                        }
                    }
                    //else
                    //{
                    //    // Assign to Neutral if no category is found
                    //    totalNeutralDuration += usage.TotalSeconds;
                    //}
                }
            }

                // Create the result object
                var result = new ProductivityDurations
                {
                    TotalProductiveDuration = TimeSpan.FromSeconds(totalProductiveDuration).ToString(@"hh\:mm\:ss"),
                    TotalUnproductiveDuration = TimeSpan.FromSeconds(totalUnproductiveDuration).ToString(@"hh\:mm\:ss"),
                    TotalNeutralDuration = TimeSpan.FromSeconds(totalNeutralDuration).ToString(@"hh\:mm\:ss")
                };

                return result;
        }

    }
}
