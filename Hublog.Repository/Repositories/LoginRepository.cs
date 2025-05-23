﻿using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Microsoft.AspNetCore.Rewrite;
using Org.BouncyCastle.Crypto.Generators;


namespace Hublog.Repository.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly Dapperr _dapper;
        public LoginRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        #region AdminLogin
        public async Task<Users> AdminLogin(string email, string password) 
        {
            var query = "SELECT A.*, B.Name as RoleName, B.AccessLevel, " +
                        "C.Name as DesignationName, D.Name as TeamName " +
                        "FROM Users A WITH(NOLOCK) " +
                        "INNER JOIN Role B WITH(NOLOCK) ON A.RoleId = B.Id " +
                        "INNER JOIN Designation C WITH(NOLOCK) ON A.DesignationId = C.Id " +
                        "INNER JOIN Team D WITH(NOLOCK) ON A.TeamId = D.Id " +
                        "WHERE B.AccessLevel = 1 AND A.Email = @Email AND A.Password = @Password AND A.Active = 1";

            var result= (await _dapper.GetAllAsync<Users>(query, new { Email = email, Password = password })).FirstOrDefault();

            if (result == null || result.Password!=password)
            {
                return null;
            }

            return result;
        }
        #endregion  

        #region Login
        public async Task<Users> Login(string email, string password)
        {
            var query = @"SELECT A.*, B.Name as RoleName, B.AccessLevel, 
                             C.Name as DesignationName, D.Name as TeamName 
                      FROM Users A
                      INNER JOIN Role B ON A.RoleId = B.Id
                      LEFT JOIN Designation C ON A.DesignationId = C.Id
                      LEFT JOIN Team D ON A.TeamId = D.Id
                      WHERE A.Email = @Email AND A.Password = @Password AND A.Active = 1";

            var user = await _dapper.GetAsync<Users>(query, new { Email = email, Password = password });

            if (user == null)
            {
                return null;
            }
            if (user == null || password != user.Password)
            {
                return null; // Incorrect password
            }

            return user;
        }
        #endregion

        #region UserLogin
        public async Task<Users> UserLogin(string email, string password)
        {
            string query = "SELECT A.*, B.Name AS RoleName, B.AccessLevel, C.Name AS DesignationName, D.Name AS TeamName " +
               "FROM Users A WITH (NOLOCK) " +
               "INNER JOIN Role B WITH (NOLOCK) ON A.RoleId = B.Id " +
               "LEFT JOIN Designation C WITH (NOLOCK) ON A.DesignationId = C.Id " +
               "LEFT JOIN Team D WITH (NOLOCK) ON A.TeamId = D.Id " +
               "WHERE B.AccessLevel = 2 AND A.Email = @Email AND A.Password = @Password AND A.Active = 1";

            var parameters = new { Email = email, Password = password };
            //return await _dapper.GetAsync<Users>(query, parameters);
            var result = await _dapper.GetAsync<Users>(query, parameters);

            if (result == null||result.Password != password)
            {
                return null;
            }
            return result;
        }
        #endregion
    }
}
