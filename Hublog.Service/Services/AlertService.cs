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

        public async Task<List<Alert>> GetAlert(int organizationId, int? userId,  DateTime triggeredTime)
        {
            var alerts = await _alertRepository.GetAlert(organizationId, userId, triggeredTime);
            return alerts;
        }
    }
}
