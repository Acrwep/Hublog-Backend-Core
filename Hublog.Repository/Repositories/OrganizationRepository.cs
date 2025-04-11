using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Entities.Model.Shift;
using Hublog.Repository.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Hublog.Repository.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly Dapperr _dapper;

        public OrganizationRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }
        public async Task<List<Organizations>> GetAllAsync()
        {
            string query = "SELECT * FROM Organization";
            return await _dapper.GetAllAsync<Organizations>(query);
        }


        public async Task<bool> IsNameExistsAsync(string email)
        {
            const string query = "SELECT COUNT(1) FROM Organization WHERE Email = @Email";
            int count = await _dapper.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }
        public async Task<bool> IsUserEmailExistsAsync(string email)
        {
            const string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            int count = await _dapper.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }

        public async Task<Organizations> InsertAsync(Organizations organization)
        {
            try
            {
                // Check if the name already exists
                bool emailExists = await IsNameExistsAsync(organization.Email);
                if (emailExists)
                {
                    return null; // Returning null to indicate a duplicate
                }

                bool userEmailExists = await IsUserEmailExistsAsync(organization.Email);
                if (userEmailExists)
                {
                    return null; // Returning null to indicate a duplicate
                }

                string insertQuery = @"
            INSERT INTO Organization 
            (Organization_Name, Country, FirstName, LastName, Email, Mobile, Domain, Licence, PlanName, 
            PlanStartDate, PlanEndDate, WebsiteUrl, LinkdinUrl, PaidAmount) 
            VALUES 
            (@Organization_Name, @Country, @FirstName, @LastName, @Email, @Mobile, @Domain, @Licence, 
            @PlanName, @PlanStartDate, @PlanEndDate, @WebsiteUrl, @LinkdinUrl, @PaidAmount);
            SELECT SCOPE_IDENTITY();";

                //int newId = await _dapper.ExecuteScalarAsync<int>(insertQuery, organization);

                //if (newId > 0)
                //{
                //    string selectQuery = "SELECT * FROM Organization WHERE Id = @Id";
                //    return await _dapper.GetAsync<Organizations>(selectQuery, new { Id = newId });
                //}
                //return null;
                int newId = await _dapper.GetSingleAsync<int>(insertQuery, organization);
                organization.Id = newId;
                return organization;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating shiftmaster", ex);
            }

        }

        public async Task<bool> UpdateNameExistsAsync(string email, int id)
        {
            const string query = @"SELECT COUNT(1) FROM Organization WHERE Email = @Email AND Id <> @Id";
            int count = await _dapper.ExecuteScalarAsync<int>(query, new { Email = email, Id = id });
            return count > 0;
        }

        public async Task<object> UpdateAsync(Organizations organization)
        {
            try
            {

                // Check if the name already exists
                bool nameExists = await UpdateNameExistsAsync(organization.Email, organization.Id);
                if (nameExists)
                {
                    return null; // Returning null to indicate a duplicate
                }



                string updateQuery = @"
            UPDATE Organization 
            SET Organization_Name = @Organization_Name, Country = @Country, FirstName = @FirstName, 
                LastName = @LastName, Email = @Email, Mobile = @Mobile, Domain = @Domain, 
                Licence = @Licence, PlanName = @PlanName, PlanStartDate = @PlanStartDate, 
                PlanEndDate = @PlanEndDate, WebsiteUrl = @WebsiteUrl, LinkdinUrl = @LinkdinUrl, 
                PaidAmount = @PaidAmount 
            WHERE Id = @Id";

                // int rowsAffected = await _dapper.ExecuteAsync(updateQuery, organization);
                //return rowsAffected > 0;

                var result = await _dapper.ExecuteAsync(updateQuery, organization);

                if (result > 0)
                {
                    string selectQuery = @"
                                           SELECT Id, Organization_Name, Country, FirstName, LastName, Email,
                                           Mobile, Domain, Licence, PlanName, 
                                           PlanStartDate, PlanEndDate, WebsiteUrl, LinkdinUrl, PaidAmount
                                           FROM Organization
                                           WHERE Id = @Id";

                    return await _dapper.GetAsync<Organizations>(selectQuery, new { Id = organization.Id });
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating Organization", ex);
            }

        }

        public async Task<bool> CheckDomainAvailabilityAsync(string domain)
        {
            string query = "SELECT COUNT(1) FROM Organization WHERE Domain = @Domain";
            int count = await _dapper.ExecuteScalarAsync<int>(query, new { Domain = domain });
            return count > 0;
        }

        public async Task<DateTime?> GetPlanEndDateAsync(int organizationId)
        {
            var query = @"select PlanEndDate from Organization where id=@Id";
            var parameter = new { Id = organizationId };
            var result = await _dapper.ExecuteScalarAsync<DateTime?>(query,parameter);
            return result;
        }

       
    }
}
