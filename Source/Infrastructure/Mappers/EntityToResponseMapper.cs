using AutoMapper;
using Core.Entities;
using Core.Contracts.Response;
using Core.ValueObjects;

namespace Infrastructure.Mappers
{
    public class EntityToResponseMapper : Profile
    {
        public EntityToResponseMapper()
        {
            CreateMap<Student, StudentResponse>()
                    .ForMember(
                        res => res.DateCreated,
                        opt => opt.MapFrom(src => src.DateCreated.ToUniversalTime())
                    )
                    .ForMember(
                        res => res.DateLastModified,
                        opt => opt.MapFrom(src => src.DateLastModified.ToUniversalTime())
                    );

            CreateMap<Address, AddressResponse>();

            CreateMap<Company, CompanyResponse>();

            CreateMap<Exam, ExamResponse>();

            CreateMap<FurtherStudy, FurtherStudyResponse>();

            CreateMap<Faculty, FacultyResponse>();

        }
    }
}
