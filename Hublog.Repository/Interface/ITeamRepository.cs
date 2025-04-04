﻿using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface ITeamRepository
    {
        Task<List<Team>> GetTeams(int organizationId);

        //Task<Team> CreateTeam(Team team);

        Task<(bool IsSuccessful, string Message, Team CreatedTeam)> CreateTeam(Team team);

        Task<(string, Team)> UpdateTeam(int id, Team team);
        //mycode
        Task<(int RowsAffected, string Message)> UpdateTeams(Team team);

        Task<string> DeleteTeam(int id);
    }
}
