using Core.ValueObjects;

namespace Infrastructure.Mappers;

public class CommandToEntityMapper : Profile
{
    public CommandToEntityMapper()
    {

        CreateMap<AddStudentCommand, Student>()
                .ForMember(
                    s => s.Gender,
                    opt => opt.MapFrom(sc => Enum.Parse(typeof(Gender), sc.Gender))
                )
                .ForMember(
                    s => s.Branch,
                    opt => opt.MapFrom(sc => Enum.Parse(typeof(Branch), sc.Branch))
                );

        CreateMap<AddCompanyCommand, Company>();

        CreateMap<AddExamCommand, Exam>();

        CreateMap<AddFurtherStudyCommand, FurtherStudy>();

        CreateMap<AddFacultyCommand, Faculty>();

        CreateMap<AddressResponse, Address>();
    }
}
