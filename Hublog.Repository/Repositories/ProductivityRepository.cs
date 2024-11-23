using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
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
        public async Task<List<AppUsage>> GetAppUsages(int organizationId,int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            string appUsageQuery = @"
           SELECT 
               A.UserId, 
               A.ApplicationName, 
               A.Details, 
               SUM(DATEDIFF(SECOND, '00:00:00', A.TotalUsage)) AS TotalSeconds, 
               A.UsageDate
           FROM  
               ApplicationUsage A
           INNER JOIN 
               Users U ON A.UserId = U.Id
           INNER JOIN 
               Organization O ON U.OrganizationId = O.Id
           WHERE  
                  O.Id = @OrganizationId 
                  AND A.UsageDate BETWEEN @FromDate AND @ToDate
                  AND (@TeamId IS NULL OR U.TeamId = @TeamId)
                  AND (@UserId IS NULL OR A.UserId = @UserId)
           GROUP BY 
               A.UserId, 
               A.ApplicationName, 
               A.Details, 
               A.UsageDate;
        ";

            var urlUsageQuery = @"
             SELECT U.UserId,
                 U.Url AS ApplicationName,
                 U.Details,
                 SUM(DATEDIFF(SECOND, '00:00:00', U.TotalUsage)) AS TotalSeconds,
                 U.UsageDate
             FROM UrlUsage U
             INNER JOIN 
               Users Us ON U.UserId = Us.Id
             INNER JOIN 
               Organization O ON Us.OrganizationId = O.Id
             WHERE 
               O.Id = @OrganizationId 
               AND U.UsageDate BETWEEN @FromDate AND @ToDate
               AND (@TeamId IS NULL OR US.TeamId = @TeamId)
               AND (@UserId IS NULL OR U.UserId = @UserId)
             GROUP BY 
               U.UserId,
               U.Url,
               U.Details, 
               U.UsageDate;"
            ;
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate
            };
            
            var appUsages = await _dapper.GetAllAsync<AppUsage>(appUsageQuery, parameters);
            var urlUsages = await _dapper.GetAllAsync<AppUsage>(urlUsageQuery, parameters);

            var allUsages = appUsages.Concat(urlUsages).ToList();

            // Return the merged list return allUsages;
            return allUsages;
            //var urlUsageQuery1 = @"select * imImbuildAppsAndUrls where name Like '' ",;


        }
        public async Task<ProductivityDurations> GetProductivityDurations(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            var usages = await GetAppUsages(organizationId,teamId,userId, fromDate, toDate);

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
            var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

            var dateDifferenceInDays = (toDate - fromDate).TotalDays;

            if (dateDifferenceInDays <= 0)
            {
                dateDifferenceInDays = 1; 
            }
            var averageDurationInSeconds = totalProductiveDuration / dateDifferenceInDays;


            var result = new ProductivityDurations
            {
                TotalProductiveDuration = TimeSpan.FromSeconds(totalProductiveDuration).ToString(@"hh\:mm\:ss"),
                TotalUnproductiveDuration = TimeSpan.FromSeconds(totalUnproductiveDuration).ToString(@"hh\:mm\:ss"),
                TotalNeutralDuration = TimeSpan.FromSeconds(totalNeutralDuration).ToString(@"hh\:mm\:ss"),
                TotalDuration = TimeSpan.FromSeconds(totalDurationInSeconds).ToString(@"hh\:mm\:ss"),
                AverageDuratiopn = TimeSpan.FromSeconds(averageDurationInSeconds).ToString(@"hh\:mm\:ss")
            };

            return result;

        }

    }
}
