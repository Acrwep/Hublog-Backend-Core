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
            var query = "InsertManualTime1";

            var parameters = new
            {
                OrganizationId = manual_Time.OrganizationId,
                UserId = manual_Time.UserId,
                Date = manual_Time.Date,
                StartTime = manual_Time.StartTime,
                EndTime = manual_Time.EndTime,
                Summary = manual_Time.Summary,
                Attachment = manual_Time.Attachment
            };

            await _dapper.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            var selectQuery = @"
        SELECT TOP 1 *
        FROM Manual_Time
        WHERE OrganizationId = @OrganizationId
          AND UserId = @UserId
          AND Date = @Date
          AND StartTime = @StartTime
          AND EndTime = @EndTime
          AND Summary = @Summary
          AND Attachment = @Attachment
        ORDER BY Id DESC";

            var insertedRecord = await _dapper.QueryFirstOrDefaultAsync<Manual_Time>(selectQuery, parameters);
            return insertedRecord;
        }
        public async Task<IEnumerable<Manual_Time>> GetManualTime(int organizationId, int? teamid, int? userId)
        {
            var query = "GetManualTimeEntries"; 

            var parameters = new
            {             
                OrganizationId = organizationId,
                TeamId=teamid,
                UserId = userId
            };

            return await _dapper.QueryAsync<Manual_Time>(query, parameters, commandType: CommandType.StoredProcedure);
        }

    }

}
