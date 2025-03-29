using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;
        public EmailService(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }
        public async Task SendEmailAsync(Users users)
        {
            await _emailRepository.SendEmailAsync(users);
        }

        public async Task SendOrganizationEmailAsync(Organizations organizations)
        {
            await _emailRepository.SendOrganizationEmailAsync(organizations);
        }
    }
}
