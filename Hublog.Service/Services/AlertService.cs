using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Attendance;
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
    }
}
