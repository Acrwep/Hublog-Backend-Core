using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Manual_Time;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Repository.Interface
{
    public interface IManual_TimeRepository
    {
        Task<Manual_Time> InsertManualTime(Manual_Time manual_Time);
        Task<IEnumerable<GetManualList>> GetManualTime(int organizationId, int? teamid, int? userId);
    }
}
