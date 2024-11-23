using AutoMapper;
using Repositories.Repositories.Entities;
using Services.DTOs.GeneralInfo.AreaDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MapperProfile
{
    public class GeneralInfoProfile : Profile
    {
        public GeneralInfoProfile()
        {
            CreateMap<Area, AreaResponseDTO>()
                .ForMember(dest => dest.BlockCount,
                          opt => opt.MapFrom(src => src.Blocks.Count(b => (bool)!b.IsDeleted)));
            CreateMap<CreateAreaDTO, Area>();
            CreateMap<UpdateAreaDTO, Area>();
        }
    }
}
