using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Attendance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hublog.Repository.Interface
{
    public interface IAlertRepository
    {
        Task<int> InsertAlert(Alert Alert);
        Task<List<Alert>> GetAlert(int organizationId,int? userId, DateTime triggeredTime);
    }
}

