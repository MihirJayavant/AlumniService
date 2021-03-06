using System;
using MediatR;

namespace Infrastructure.Commands
{
    public class AddFacultyCommand : IRequest
    {
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Admin { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long MobileNo { get; set; }
        public string Extension { get; set; }
    }
}
