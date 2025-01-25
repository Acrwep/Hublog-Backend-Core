using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Manual_Time;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class Manual_TimeRepository : IManual_TimeRepository
    {
        private readonly Dapperr _dapper;

        public Manual_TimeRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        public async Task<Manual_Time> InsertManualTime(Manual_Time manual_Time)
        {
            // Check if the Attachment is not null, and convert it to byte array
            if (manual_Time.Attachment != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await manual_Time.Attachment.CopyToAsync(memoryStream);
                    manual_Time.AttachmentData = memoryStream.ToArray(); // Convert to byte array
                    manual_Time.FileName = manual_Time.Attachment.FileName; // Store the file name
                }
            }

            var query = "InsertManualTime";

            var parameters = new
            {
                OrganizationId = manual_Time.OrganizationId,
                UserId = manual_Time.UserId,
                Date = manual_Time.Date,
                StartTime = manual_Time.StartTime,
                EndTime = manual_Time.EndTime,
                Summary = manual_Time.Summary,
                FileName = manual_Time.FileName,
                AttachmentData = manual_Time.AttachmentData // This will now be passed as byte[]
            };

            await _dapper.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);

            var selectQuery = @"
    SELECT TOP (1) [id]
      ,[OrganizationId]
      ,[UserId]
      ,[Date]
      ,[StartTime]
      ,[EndTime]
      ,[Summary]
      ,[FileName]
    FROM Manual_Time
    WHERE OrganizationId = @OrganizationId
      AND UserId = @UserId
      AND Date = @Date
      AND StartTime = @StartTime
      AND EndTime = @EndTime
      AND Summary = @Summary
      AND FileName = @FileName
    ORDER BY Id DESC";
            var insertedRecord = await _dapper.QueryFirstOrDefaultAsync<Manual_Time>(selectQuery, parameters);

            return insertedRecord;
        }

        public async Task<IEnumerable<GetManualList>> GetManualTime(int organizationId, int? teamid, int? userId, DateTime? startDate, DateTime? endDate)
        {
            var query = "GetManualTimeEntries"; 

            var parameters = new
            {             
                OrganizationId = organizationId,
                TeamId=teamid,
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            return await _dapper.QueryAsync<GetManualList>(query, parameters, commandType: CommandType.StoredProcedure);
        }

    }

}
