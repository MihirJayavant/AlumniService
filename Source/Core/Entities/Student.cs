using Core.ValueObjects;
namespace Core.Entities;

public class Student
{
    public int StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public long MobileNo { get; set; }
    public string Extension { get; set; }
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }

    public string Email { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateLastModified { get; set; }

    public Branch Branch { get; set; }
    public Address CurrentAddress { get; set; }
    public Address CorrespondanceAddress { get; set; }
    public int AdmissionYear { get; set; }
    public int PassingYear { get; set; }

    public Guid StudentAccount { get; set; }
    public StudentAccount Account { get; set; }

    public List<Company> Companies { get; set; }
    public List<Exam> Exams { get; set; }
    public List<FurtherStudy> FurtherStudies { get; set; }

}
