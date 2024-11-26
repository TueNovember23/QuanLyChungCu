using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DTOs.ApartmentDTO;
using Repositories.Repositories.Entities;
namespace Services.MapperProfile
{
    public class ApartmentProfile : Profile
    {
        public ApartmentProfile()
        {
            CreateMap<Apartment, ResponseApartmentDTO>()
                .ForMember(dest => dest.Block, opt => opt.MapFrom(src => src.Floor.Block != null ? src.Floor.Block.BlockCode : string.Empty))
                .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Floor != null ? src.Floor.FloorNumber : 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? string.Empty));
        }
    }


}
