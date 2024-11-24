using AutoMapper;
using Repositories.Repositories.Entities;
using Services.DTOs.GeneralInfo.AreaDTO;
using Services.DTOs.GeneralInfo.BlockDTO;
using Services.DTOs.GeneralInfo.FloorDTO;
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

            CreateMap<Block, BlockResponseDTO>()
                .ForMember(dest => dest.BlockId, opt => opt.MapFrom(src => src.BlockId))
                .ForMember(dest => dest.BlockCode, opt => opt.MapFrom(src => src.BlockCode))
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.AreaId))
                .ForMember(dest => dest.NumberOfFloor, opt => opt.MapFrom(src => src.NumberOfFloor));

            CreateMap<Floor, FloorResponseDTO>()
                .ForMember(dest => dest.FloorId, opt => opt.MapFrom(src => src.FloorId))
                .ForMember(dest => dest.FloorNumber, opt => opt.MapFrom(src => src.FloorNumber))
                .ForMember(dest => dest.NumberOfApartment, opt => opt.MapFrom(src => src.NumberOfApartment));
        }
    }
}
