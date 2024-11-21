using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Attendance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hublog.Service.Interface
{
    public interface IAlertService
    {
        Task InsertAlert(List<Alert>  alert);
        Task<List<Alert>> GetAlert(int organizationId, int? userId, DateTime triggeredTime);
    }
}
