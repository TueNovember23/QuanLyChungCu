using AutoMapper;
using Repositories.Repositories.Entities;
using Services.DTOs.ViolationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MapperProfile
{
    public class ViolationProfile : Profile
    {
        public ViolationProfile()
        {
            CreateMap<Violation, ViolationResponseDTO>()
                .ForMember(dest => dest.ApartmentCode,
                    opt => opt.MapFrom(src => src.Apartment.ApartmentCode))
                .ForMember(dest => dest.RegulationTitle,
                    opt => opt.MapFrom(src => src.Regulation.Title))
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.MapFrom(src => src.CreatedDate.ToDateTime(TimeOnly.MinValue)))
                .ForMember(dest => dest.LatestProcessingStatus,
                    opt => opt.MapFrom(src => src.ViolationPenalties
                        .OrderByDescending(p => p.ProcessedDate)
                        .Select(p => p.ProcessingStatus)
                        .FirstOrDefault() ?? "Chờ xử lý"));

            CreateMap<CreateViolationDTO, Violation>()
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.MapFrom(src => DateOnly.FromDateTime(src.CreatedDate)));

            CreateMap<ViolationPenalty, ViolationPenaltyDTO>();
            CreateMap<ViolationPenaltyDTO, ViolationPenalty>();
        }
    }
}
