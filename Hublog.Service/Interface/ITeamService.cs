using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface ITeamService
    {
        Task<List<Team>> GetTeams(int organizationId);  

        Task<Team> CreateTeam (Team team);

        Task<Team> UpdateTeam (int id, TeamDTO teamDto);

        Task<string> DeleteTeam (int id);
    }
}
