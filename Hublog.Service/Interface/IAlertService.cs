using Hublog.Repository.Entities.Model.AlertModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hublog.Service.Interface
{
    public interface IAlertService
    {
        Task<bool> InsertAlert(Alert model);
        Task<List<Alert>> GetAlert(int organizationId, int? userId, DateTime triggeredTime);
    }
}
