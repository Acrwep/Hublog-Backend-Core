using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.UserModels;

namespace Hublog.Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(Users users);
    }
}
