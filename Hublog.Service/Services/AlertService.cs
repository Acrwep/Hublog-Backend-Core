using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
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

        public async Task InsertAlert(List<Alert> alert)
        {
            foreach (var model in alert)
            {
                await _alertRepository.InsertAlert(model);
            }
        }
        public async Task<List<Alert>> GetAlert(int organizationId, int? userId,  DateTime triggeredTime)
        {
            var alerts = await _alertRepository.GetAlert(organizationId, userId, triggeredTime);
            return alerts;
        }
        public async Task<Alert_Rule> InsertAlertRule(Alert_Rule alert_Rule)
        {
            return await _alertRepository.InsertAlertRule(alert_Rule);
        }
        public async Task<Alert_Rule> UpdateAlertRule(Alert_Rule alert_Rule)
        {
            return await _alertRepository.UpdateAlertRule(alert_Rule);
        }
        public async Task<List<Alert_Rule>> GetAlertRule(string searchQuery)
        {
            return await _alertRepository.GetAlertRule(searchQuery);
        }
    }
}
