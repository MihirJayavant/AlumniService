using System;

namespace Core.Contracts.Response
{
    public class StudentResponse
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public long MobileNo { get; set; }
        public string Extension { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Branch { get; set; }
        public AddressResponse CurrentAddress { get; set; }
        public AddressResponse CorrespondanceAddress { get; set; }
        public int AdmissionYear { get; set; }
        public int PassingYear { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastModified { get; set; }

    }
}
