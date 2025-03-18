using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Common;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class ForgotPasswordRepository : IForgotPasswordRepository
    {
        private readonly Dapperr _dapper;

        public ForgotPasswordRepository(Dapperr dapper)
        {
            _dapper = dapper;

        }
        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            int count = await _dapper.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }
    }
}
