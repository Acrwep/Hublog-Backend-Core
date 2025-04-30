using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Hublog.Repository.Common
{
    public class Dapperr : IDisposable
    {
        private bool _disposed = false;
        private readonly SqlConnection _connection;

        public Dapperr(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DataBaseConnectionString");
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        #region Dapp
        public int Execute(string query, object parameters = null)
        {
            return _connection.Execute(query, parameters);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string query, object parameters = null, CommandType commandType = CommandType.Text)
        {
            var result = await _connection.QueryAsync<T>(query, parameters, commandType: commandType);
            return result.FirstOrDefault();
        }



        public object ExecuteScalar(string query)
        {
            return _connection.ExecuteScalar(query);
        }

        public T Get<T>(string query)
        {
            return _connection.Query<T>(query).FirstOrDefault();
        }

        public List<T> GetAll<T>(string query, object parameters = null)
        {
            return _connection.Query<T>(query, parameters).ToList();
        }

        public async Task<int> ExecuteAsync(string query, object parameters = null)
        {
            return await _connection.ExecuteAsync(query, parameters);
        }

        public async Task<int> ExecuteAsync(string query, object parameters, CommandType commandType)
        {
            return await _connection.ExecuteAsync(query, parameters, commandType: commandType);
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CommandType? commandType = null)
        {
            // Use the connection to perform the query asynchronously
            return await _connection.QueryAsync<T>(sql, parameters, commandType: commandType);
        }

        public async Task<List<T>> GetAllAsync<T>(string query, object parameters = null)
        {
            var result = await _connection.QueryAsync<T>(query, parameters);
            return result.ToList();
        }

        public async Task<List<T>> GetAllAsyncs<T>(string query, object parameters = null, CommandType commandType = CommandType.Text)
        {
            var result = await _connection.QueryAsync<T>(query, parameters, commandType: commandType);
            return result.ToList();
        }

        public async Task<T> ExecuteScalarAsync<T>(string query, object parameters = null, CommandType commandType = CommandType.Text)
        {
            return await _connection.ExecuteScalarAsync<T>(query, parameters, commandType: commandType);
        }

        public async Task<T> GetAsync<T>(string query, object parameters = null, CommandType commandType = CommandType.Text)
        {
            var result = await _connection.QueryAsync<T>(query, parameters, commandType: commandType);
            return result.FirstOrDefault();
        }

        public async Task<T> GetSingleAsync<T>(string query, object parameters = null, CommandType commandType = CommandType.Text)
        {
            return await _connection.QueryFirstOrDefaultAsync<T>(query, parameters, commandType: commandType);
        }

        public async Task<T> GetSingleValueAsycn<T>(string query, object parameters = null)
        {
            return await _connection.ExecuteScalarAsync<T>(query, parameters);
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection?.Dispose();
                }

                _disposed = true;
            }
        }

       
    }
}
