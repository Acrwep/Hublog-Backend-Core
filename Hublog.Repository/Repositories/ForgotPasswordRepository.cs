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

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            string query = "UPDATE Users SET Password = @Password WHERE Email = @Email";

            var parameters = new { Email = email, Password = newPassword };

            int rowsAffected = await _dapper.ExecuteAsync(query, parameters);

            return rowsAffected > 0;
        }
    }
}
