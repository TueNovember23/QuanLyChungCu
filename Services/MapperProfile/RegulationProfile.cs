using AutoMapper;
using Repositories.Repositories.Entities;
using Services.DTOs.RegulationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MapperProfile
{
    public class RegulationProfile : Profile
    {
        public RegulationProfile()
        {
            CreateMap<Regulation, RegulationResponseDTO>()
                .ForMember(dest => dest.CreatedDate,
                        opt => opt.MapFrom(src => src.CreatedDate.ToDateTime(TimeOnly.MinValue)));

            CreateMap<CreateRegulationDTO, Regulation>();

            CreateMap<RegulationResponseDTO, Regulation>()
                .ForMember(dest => dest.CreatedDate,
                        opt => opt.MapFrom(src => DateOnly.FromDateTime(src.CreatedDate)));
        }
    }
}
