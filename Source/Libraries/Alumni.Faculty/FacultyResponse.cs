namespace Alumni.Faculty;

[RecordView(typeof(Faculty), nameof(Faculty.Id), nameof(Faculty.IsDeleted))]
public sealed partial record FacultyResponse
{

}

public static class FacultyResponseMapper
{
    public static FacultyResponse ToFacultyResponse(this Faculty faculty) =>
        new()
        {
            FacultyId = faculty.FacultyId,
            FirstName = faculty.FirstName,
            LastName = faculty.LastName,
            MobileNo = faculty.MobileNo,
            Extension = faculty.Extension,
            Email = faculty.Email,
            CreatedAt = faculty.CreatedAt,
            UpdatedAt = faculty.UpdatedAt,
        };
}
