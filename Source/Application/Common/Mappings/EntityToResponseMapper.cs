using Application.Companies;
using Application.Exams;
using Application.Faculties;
using Application.FurtherStudies;
using Application.Students;

namespace Application.Common.Mappings;

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
