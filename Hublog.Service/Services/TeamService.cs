using AutoMapper;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        public TeamService(ITeamRepository teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccessful, string Message, Team CreatedTeam)> CreateTeam(Team team)
        {
            return await _teamRepository.CreateTeam(team);
        }

        public async Task<string> DeleteTeam(int id)
        {
            return await _teamRepository.DeleteTeam(id);
        }

        public async Task<List<Team>> GetTeams(int organizationId)
        {
            return await _teamRepository.GetTeams(organizationId);
        }

        public async Task<(string, Team)> UpdateTeam(int id, TeamDTO teamDto)
        {
            var team = _mapper.Map<Team>(teamDto);
            return await _teamRepository.UpdateTeam(id, team);
        }

    }
}
