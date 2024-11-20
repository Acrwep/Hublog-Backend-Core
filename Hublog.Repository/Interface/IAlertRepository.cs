using Hublog.Repository.Entities.Model.AlertModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hublog.Repository.Interface
{
    public interface IAlertRepository
    {
        Task<Alert> InsertAlert(Alert alert);
        Task<List<Alert>> GetAlert(int id, int userId, string triggered, DateTime triggeredTime);
    }
}

