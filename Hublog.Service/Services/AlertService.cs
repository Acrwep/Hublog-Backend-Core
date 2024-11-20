using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hublog.Service.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepository;

        public AlertService(IAlertRepository alertRepository)
        {
            _alertRepository = alertRepository;
        }

        public async Task<bool> InsertAlert(Alert model)
        {
            var alert = await _alertRepository.InsertAlert(model);
            return alert != null;
        }

        public async Task<List<Alert>> GetAlert(int id, int userId, string triggered, DateTime triggeredTime)
        {
            var alerts = await _alertRepository.GetAlert(id, userId, triggered, triggeredTime);
            return alerts;
        }
    }
}
