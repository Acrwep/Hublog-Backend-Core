using AutoMapper;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Common
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {

            CreateMap<TeamDTO, Team>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

        }
    }
}
