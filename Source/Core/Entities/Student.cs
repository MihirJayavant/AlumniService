using Core.ValueObjects;
namespace Core.Entities;

public class Student
{
    public int StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public long MobileNo { get; set; }
    public string Extension { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }

    public string Email { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public DateTime DateLastModified { get; set; }

    public Branch Branch { get; set; }
    public Address CurrentAddress { get; set; } = default!;
    public Address CorrespondanceAddress { get; set; } = default!;
    public int AdmissionYear { get; set; }
    public int PassingYear { get; set; }

    public Guid StudentAccount { get; set; }
    public StudentAccount Account { get; set; } = default!;

    public List<Company> Companies { get; set; } = default!;
    public List<Exam> Exams { get; set; } = default!;
    public List<FurtherStudy> FurtherStudies { get; set; } = default!;

}
